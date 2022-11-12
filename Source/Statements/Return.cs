using LLVMSharp.Interop;
using WARD.Expressions;
using WARD.Generics;
using WARD.Scoping;
using WARD.Types;

namespace WARD.Statements;

// A return statement to return a value. TODO: CAST RETURN EXPRESSION!!!
public class StatementReturn : Statement {
    private Scope Scope; // Scope.
    public Expression ReturnExpression { get; } // Return value for the return.

    // Create a new return statement. Leave null for void.
    public StatementReturn(Expression returnExpression = null) {
        ReturnExpression = returnExpression;
    }

    public override void SetScopes(Scope parent) {
        Scope = parent;
        if (ReturnExpression != null) ReturnExpression.SetScopes(parent);
    }

    public override void ResolveVariables() {
        if (ReturnExpression != null) ReturnExpression.ResolveVariables();
    }

    public override void ResolveTypes() {
        if (ReturnExpression != null) ReturnExpression.ResolveTypes();
    }

    public override bool ReturnsType() => true; // It's a return statement, what did you expect.

    public override bool EndsBlock() => true;

    public override void CompileDeclarations(LLVMModuleRef mod, LLVMBuilderRef builder) {
        if (ReturnExpression != null) ReturnExpression.CompileDeclarations(mod, builder);
    }

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder, CompilationContext ctx) {
        if (ReturnExpression != null) {
            return builder.BuildRet(ReturnExpression.Compile(mod, builder, ctx));
        } else {
            return builder.BuildRetVoid();
        }
    }

    public override Statement Instantiate(InstantiationInfo info) => new StatementReturn(ReturnExpression == null ? null : (ReturnExpression.Instantiate(info) as Expression));

    public override string ToString() => ReturnExpression == null ? "return;" : ("return " + ReturnExpression.ToString() + ";");

}