using LLVMSharp.Interop;
using WARD.Generics;
using WARD.Statements;
using WARD.Types;

namespace WARD.Expressions;

// Expression for a constant boolean value.
public class ExpressionConstBool : Expression {
    public bool Value { get; } // Boolean value.

    // Create a new boolean expression.
    public ExpressionConstBool(bool value) {
        Value = value;
    }

    protected override VarType ReturnType() => VarType.Bool;

    public override bool Constant() => true;

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder, CompilationContext ctx) {
        return LLVMValueRef.CreateConstInt(ReturnType().GetLLVMType(Scope), Value ? 1u : 0u, false);
    }

    public override string ToString() => Value ? "true" : "false";

    public override Statement Instantiate(InstantiationInfo info) => this;

}