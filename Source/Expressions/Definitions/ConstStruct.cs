using LLVMSharp.Interop;
using WARD.Exceptions;
using WARD.Generics;
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
    }

    protected override VarType ReturnType() => StructType;

    public override bool Constant() => true;

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder) {
        foreach (var expression in Expressions) {
            if (!expression.Constant()) {
                Error.ThrowInternal("Constant struct has non-constant expression \"" + expression.ToString() + "\".");
            }
        }
        return LLVMValueRef.CreateConstStruct(Expressions.Select(x => x.Compile(mod, builder)).ToArray(), false);
    }

    public override string ToString() {
        string ret = "{\n";
        foreach (var expression in Expressions) {
            ret += "\t" + expression.ToString() + "\n";
        }
        return ret + "}";
    }

     public override Statement Instantiate(InstantiationInfo info) {
        Expression[] parameters = new Expression[Expressions.Length];
        for (int i = 0; i < parameters.Length; i++) {
            parameters[i] = Expressions[i].Instantiate(info) as Expression;
        }
        return new ExpressionConstStruct(StructType.Instantiate(info) as VarTypeStruct, parameters);
    }

}