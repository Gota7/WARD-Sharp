using LLVMSharp.Interop;
using WARD.Expressions;
using WARD.Generics;
using WARD.Scoping;

namespace WARD.Statements;

// A standard for loop.
public class StatementForLoop : Statement {
    public Expression Init { get; } // Initial expression before the loop.
    public StatementLoop CreatedLoop { get; } // Finished loop.
    private Statement Body { get; } // Body.
    private Expression Condition { get; } // Condition.
    private Expression ContinueExpression { get; } // Continue expression.

    // Create a new for loop.
    public StatementForLoop(Statement body, Expression init = null, Expression condition = null, Expression continueExpression = null) {

        // This is equivalent to:
        /*
            [Init here] // If not null.
            loop {
                if (condition) {} else break; // If oondition not null.
                [Body here]
                continue:
                    [Continue expression here] // If not null.
            }
        */
        Init = init;
        CreatedLoop = new StatementLoop(
            condition == null ? body : new StatementBlock(
                new StatementIf(condition, new StatementBlock(), new StatementBreak()),
                body
            ), continueExpression
        );
        Body = body;
        Condition = condition;
        ContinueExpression = continueExpression;

    }

    public override void SetScopes(Scope parent) {
        if (Init != null) Init.SetScopes(parent);
        CreatedLoop.SetScopes(parent);
    }

    public override void ResolveVariables() {
        if (Init != null) Init.ResolveVariables();
        CreatedLoop.ResolveVariables();
    }

    public override void ResolveTypes() {
        if (Init != null) Init.ResolveTypes();
        CreatedLoop.ResolveTypes();
    }

    public override bool ReturnsType() => (Init == null ? false : Init.ReturnsType()) || CreatedLoop.ReturnsType();

    public override bool EndsBlock() => (Init == null ? false : Init.EndsBlock()) || CreatedLoop.EndsBlock();

    public override void CompileDeclarations(LLVMModuleRef mod, LLVMBuilderRef builder) {
        if (Init != null) Init.CompileDeclarations(mod, builder);
        CreatedLoop.CompileDeclarations(mod, builder);
    }

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder, CompilationContext ctx) {
        if (Init != null) Init.Compile(mod, builder, ctx);
        if (Init != null && !Init.EndsBlock()) CreatedLoop.Compile(mod, builder, ctx);
        return null;
    }

    public override Statement Instantiate(InstantiationInfo info) => new StatementForLoop(Body.Instantiate(info), Init == null ? null : Init.Instantiate(info) as Expression, Condition == null ? null : Condition.Instantiate(info) as Expression, ContinueExpression == null ? null : ContinueExpression.Instantiate(info) as Expression);

    public override string ToString() => (Init != null ? (Init.ToString() + "\n") : "") + CreatedLoop.ToString();

}