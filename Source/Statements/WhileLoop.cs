using LLVMSharp.Interop;
using WARD.Expressions;
using WARD.Generics;
using WARD.Scoping;

namespace WARD.Statements;

// A standard while loop.
public class StatementWhileLoop : Statement {
    public StatementLoop CreatedLoop { get; } // Finished loop.
    private Statement Body { get; } // Statement body.
    private Expression Condition { get; } // Condition.

    // Create a new while loop.
    public StatementWhileLoop(Statement body, Expression condition = null) {

        // This is equivalent to:
        /*
            loop {
                if (condition) {} else break; // If condition is not null.
                [Body here]
            }
        */
        CreatedLoop = new StatementLoop(
            condition == null ? body : new StatementBlock(
                new StatementIf(condition, new StatementBlock(), new StatementBreak()),
                body
            )
        );
        Body = body;
        Condition = condition;

    }

    public override void SetScopes(Scope parent) {
        CreatedLoop.SetScopes(parent);
    }

    public override void ResolveVariables() {
        CreatedLoop.ResolveVariables();
    }

    public override void ResolveTypes() {
        CreatedLoop.ResolveTypes();
    }

    public override bool ReturnsType() => CreatedLoop.ReturnsType();

    public override bool EndsBlock() => CreatedLoop.EndsBlock();

    public override void CompileDeclarations(LLVMModuleRef mod, LLVMBuilderRef builder) {
        CreatedLoop.CompileDeclarations(mod, builder);
    }

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder, CompilationContext ctx) => CreatedLoop.Compile(mod, builder, ctx);

    public override Statement Instantiate(InstantiationInfo info) => new StatementWhileLoop(Body.Instantiate(info), Condition == null ? null : Condition.Instantiate(info) as Expression);

    public override string ToString() => CreatedLoop.ToString();

}