using LLVMSharp.Interop;
using WARD.Exceptions;
using WARD.Generics;
using WARD.Statements;
using WARD.Types;

namespace WARD.Expressions;

// Expression for a constant integer.
public class ExpressionConstInt : Expression {
    public VarType IntType { get; } // Type of integer that is constant.
    public ulong Value { get; } // Value of the constant integer.

    // Create a new constant int expression.
    public ExpressionConstInt(VarType intType, ulong value) {
        IntType = intType;
        Value = value;
    }

    protected override VarType ReturnType() => IntType.GetVarType(Scope);

    public override bool Constant() => true;

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder, CompilationContext ctx) {
        var intType = IntType.GetVarType(Scope) as VarTypeInteger;
        if (intType == null) {
            Error.ThrowInternal("Type \"" + IntType.ToString() + "\" is not a valid int type for constant number \"" + Value + "\".");
        }
        return LLVMValueRef.CreateConstInt(IntType.GetLLVMType(Scope), Value, intType.Signed);
    }

    public override string ToString() {
        var intType = IntType.GetVarType(Scope) as VarTypeInteger;
        if (intType == null) {
            Error.ThrowInternal("Type \"" + IntType.ToString() + "\" is not a valid int type for constant number \"" + Value + "\".");
        }
        return "((" + IntType.ToString() + ")" + (intType.Signed ? ((long)Value).ToString() : Value.ToString()) + ")";
    }

    public override Statement Instantiate(InstantiationInfo info) => new ExpressionConstInt(IntType.Instantiate(info), Value);

}