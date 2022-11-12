using LLVMSharp.Interop;
using WARD.Expressions;
using WARD.Generics;
using WARD.Scoping;

namespace WARD.Statements;

// A standard do while loop.
public class StatementDoWhileLoop : Statement {
    public StatementLoop CreatedLoop { get; } // Finished loop.
    private bool ConditionNull { get; } // If the condition is null.

    // Create a new do while loop.
    public StatementDoWhileLoop(Statement body, Expression condition = null) {

        // This is equivalent to:
        /*
            loop {
                [Body here]
                continue: // Only present if condition is not null.
                    if (condition) {} else break;
            }
        */
        ConditionNull = condition == null;
        CreatedLoop = new StatementLoop(
            body,
            ConditionNull ? null : new StatementIf(condition, new StatementBlock(), new StatementBreak())
        );

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

    public override Statement Instantiate(InstantiationInfo info) => new StatementDoWhileLoop(CreatedLoop.Body.Instantiate(info), ConditionNull ? null : (CreatedLoop.Cont as StatementIf).Condition.Instantiate(info) as Expression);

    public override string ToString() => CreatedLoop.ToString();

}