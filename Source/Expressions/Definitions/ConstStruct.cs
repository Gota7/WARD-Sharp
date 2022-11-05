using LLVMSharp.Interop;
using WARD.Exceptions;
using WARD.Statements;
using WARD.Types;

namespace WARD.Expressions;

// Expression for a constant struct.
public class ExpressionConstStruct : Expression {
    public VarTypeStruct StructType { get; } // Type of struct that is constant.
    public Expression[] Expressions { get; } // Expressions that are constants.

    // Create a new constant int expression.
    public ExpressionConstStruct(VarTypeStruct structType, params Expression[] expressions) {
        StructType = structType;
        Expressions = expressions;
        if (structType.Members.Length != expressions.Length) {
            Error.ThrowInternal("Constant struct has the wrong number of member assignments.");
            return;
        }
        foreach (var expression in Expressions) {
            if (!expression.Constant()) {
                Error.ThrowInternal("Constant struct has non-constant expression \"" + expression.ToString() + "\".");
            }
        }
    }

    protected override VarType ReturnType() => StructType;

    public override bool Constant() => true;

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder) {
        return LLVMValueRef.CreateConstStruct(Expressions.Select(x => x.Compile(mod, builder)).ToArray(), false);
    }

    public override string ToString() {
        string ret = "{\n";
        foreach (var expression in Expressions) {
            ret += "\t" + expression.ToString() + "\n";
        }
        return ret + "}";
    }

}