using LLVMSharp.Interop;
using WARD.Generics;
using WARD.Scoping;
using WARD.Statements;
using WARD.Types;

namespace WARD.Expressions;

// An operator is really just a function that is global among compilation units. They can be called by name for convenience.
public class ExpressionOperator : Expression {
    private ExpressionCall Call; // What really gets used.

    // Create a new operator expression.
    public ExpressionOperator(string op, params Expression[] operands) {
        Call = new ExpressionCall(new ExpressionVariable("%OP%_" + op), operands);
    }

    public override void SetScopes(Scope parent) {
        Scope = parent;
        Call.SetScopes(parent);
    }

    public override void ResolveVariables() {
        Call.ResolveVariables();
    }

    public override void ResolveTypes(VarType preferredReturnType, List<VarType> parameterTypes) {
        Call.ResolveTypes();
    }

    protected override VarType ReturnType() => Call.GetReturnType();

    public override bool Constant() => Call.Constant();

    public override void CompileDeclarations(LLVMModuleRef mod, LLVMBuilderRef builder) {
        Call.CompileDeclarations(mod, builder);
    }

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder, CompilationContext ctx) => Call.Compile(mod, builder, ctx);

    public override string ToString() => Call.ToString();

    public override Statement Instantiate(InstantiationInfo info) => Call.Instantiate(info);

}