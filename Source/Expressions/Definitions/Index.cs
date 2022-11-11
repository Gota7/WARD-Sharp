using LLVMSharp.Interop;
using WARD.Common;
using WARD.Exceptions;
using WARD.Generics;
using WARD.Scoping;
using WARD.Statements;
using WARD.Types;

namespace WARD.Expressions;

// Expression for an index of a struct or array.
public class ExpressionIndex : Expression {
    public Expression Operand { get; } // Operand to get the element of.
    private string Member = null; // Struct member to get.
    private uint Index = 0; // Actual index to get.
    private VarType RetType; // Gotten return type.
    private bool IsStruct; // If we are indexing a struct or not.

    // Create an index expression for a member.
    public ExpressionIndex(Expression operand, string member) {
        Operand = operand;
        Member = member;
    }

    // Create an index expression using just an index.
    public ExpressionIndex(Expression operand, uint index) {
        Operand = operand;
        Index = index;
    }

    public override void ResolveVariables() {
        Operand.ResolveVariables();
    }

    public override void ResolveTypes(VarType preferredReturnType, List<VarType> parameterTypes) {
        Operand.ResolveTypes();
        if (Member != null) { // We are accessing a member of a struct.
            IsStruct = true;
            var structType = Operand.GetReturnType() as VarTypeStruct;
            if (structType == null) { // Have to make sure we are working with a struct for member accesses.
                Error.ThrowInternal("Index operator accessing with a member must operate on a struct type.");
                return;
            }
            bool foundMember = false;
            for (uint i = 0; i < structType.Members.Length; i++) { // Look for the member.
                if (structType.Members[i].Name.Equals(Member)) {
                    Index = i;
                    foundMember = true;
                    break;
                }
            }
            if (!foundMember) {
                Error.ThrowInternal("Struct operand does not contain member \"" + Member + "\".");
                return;
            }
            RetType = new VarTypeLValueReference(structType.Members[Index].Type.GetVarType(Scope)); // GEP returns an L-Value reference.
        } else { // We are indexing either a struct or an array.
            throw new System.NotImplementedException();
        }
    }

    protected override VarType ReturnType() => RetType;

    public override bool Constant() => false;

    public override void CompileDeclarations(LLVMModuleRef mod, LLVMBuilderRef builder) {
        Operand.CompileDeclarations(mod, builder);
    }

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder) {
        if (IsStruct) {
            return builder.BuildStructGEP2(GetReturnType().GetLLVMType(Scope), Operand.Compile(mod, builder), Index);
        } else {
            return builder.BuildGEP2(GetReturnType().GetLLVMType(Scope), Operand.Compile(mod, builder), new LLVMValueRef[] { LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, Index) });
        }
    }

    public override string ToString() => Operand.ToString() + (Member != null ? Member : ("[" + Index + "]"));

    public override Statement Instantiate(InstantiationInfo info) {
        if (Member == null) {
            return new ExpressionIndex(Operand.Instantiate(info) as Expression, Index);
        } else {
            return new ExpressionIndex(Operand.Instantiate(info) as Expression, Member);
        }
    }

}