using LLVMSharp.Interop;
using WARD.Generics;
using WARD.Scoping;

namespace WARD.Statements;

// Statement for looping infinitely.
public class StatementLoop : Statement {
    private static int LoopId = 0; // ID of the current loop.
    public Statement Body { get; } // Statement to execute in the loop body.
    public Statement Cont { get; } // Statement to execute in the "continue" code (ran after body and every continue).
    private int Id { get; } // ID of the loop.

    // Create a new loop statement.
    public StatementLoop(Statement body, Statement cont = null) {
        Body = body;
        Cont = cont;
        Id = LoopId++;
    }

    public override void SetScopes(Scope parent) {
        Body.SetScopes(parent.EnterScope("%LOOP%_" + Id));
        if (Cont != null) Cont.SetScopes(parent.EnterScope("%LOOP%_" + Id));
    }

    public override void ResolveVariables() {
        Body.ResolveVariables();
        if (Cont != null) Cont.ResolveVariables();
    }

    public override void ResolveTypes() {
        Body.ResolveTypes();
        if (Cont != null) Cont.ResolveTypes();
    }

    public override bool ReturnsType() {
        return Body.ReturnsType() || (Cont == null ? false : Cont.ReturnsType()); // We are returning a type if the body ends or if the continue block does.
    }

    /*

        Consider the following:

            loop {
                return 0;
            }

        In this case, we don't create a loop end because the body or continue returns a value.
        Note that breaking or continuing do not effect this since that just gives us a reason to have an end of a block.

    */
    public override bool EndsBlock() {
        return Body.ReturnsType() || (Cont == null ? false : Cont.ReturnsType());
    }

    public override void CompileDeclarations(LLVMModuleRef mod, LLVMBuilderRef builder) {
        Body.CompileDeclarations(mod, builder);
        if (Cont != null) Cont.CompileDeclarations(mod, builder);
    }

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder, CompilationContext ctx) {

        // Create new blocks.
        LLVMBasicBlockRef startBlock = LLVMBasicBlockRef.AppendInContext(mod.Context, ctx.Func, "loopStart" + Id);
        LLVMBasicBlockRef contBlock = null;
        if (!Body.ReturnsType()) contBlock = LLVMBasicBlockRef.AppendInContext(mod.Context, ctx.Func, "loopCont" + Id); // Only build continue if needed.
        LLVMBasicBlockRef endBlock = null;
        if (!EndsBlock()) endBlock = LLVMBasicBlockRef.AppendInContext(mod.Context, ctx.Func, "loopEnd" + Id); // Only build end if we aren't ending.

        // Jump to the main loop block and add it to the context.
        builder.BuildBr(startBlock);
        ctx.Loops.Push(new LoopContext(startBlock, contBlock, endBlock));

        // Build the main loop.
        builder.PositionAtEnd(startBlock);
        Body.Compile(mod, builder, ctx);
        if (!Body.EndsBlock()) builder.BuildBr(contBlock); // Jump to the continuation block only if not already going somewhere.

        // Build the continuation block.
        if (contBlock != null) {
            builder.PositionAtEnd(contBlock);
            if (Cont != null) {
                Cont.Compile(mod, builder, ctx);
                if (!Cont.EndsBlock()) builder.BuildBr(startBlock); // Go back to start if we are not already going somewhere.
            } else builder.BuildBr(startBlock); // Always jump back to loop start if no continuation.
        }

        // Finish loop and continue.
        if (endBlock != null) builder.PositionAtEnd(endBlock);
        ctx.Loops.Pop();
        return null;

    }

    public override Statement Instantiate(InstantiationInfo info) => new StatementLoop(Body.Instantiate(info), Cont == null ? null : Cont.Instantiate(info));

    public override string ToString() {
        string ret = "loop " + Body.ToString();
        if (Cont != null) ret += " " + Cont.ToString();
        return ret;
    }

}