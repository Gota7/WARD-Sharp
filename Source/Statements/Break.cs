using LLVMSharp.Interop;
using WARD.Exceptions;
using WARD.Generics;
using WARD.Scoping;

namespace WARD.Statements;

// Statement for breaking out of a loop.
public class StatementBreak : Statement {
    public uint NumBreaks { get; } // How many loops to break out of.

    // Create a new break statement.
    public StatementBreak(uint numBreaks = 1) {
        NumBreaks = numBreaks;
        if (NumBreaks == 0) Error.ThrowInternal("Must break out of at least 1 loop.");
    }

    public override void SetScopes(Scope parent) {}

    public override void ResolveVariables() {}

    public override void ResolveTypes() {}

    public override bool ReturnsType() => false;

    public override bool EndsBlock() => true;

    public override void CompileDeclarations(LLVMModuleRef mod, LLVMBuilderRef builder) {}

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder, CompilationContext ctx) {

        // Make sure we are the necessary loops in.
        if (ctx.Loops.Count < NumBreaks) {
            Error.ThrowInternal("Can not break out of \"" + NumBreaks + "\" loops when only \"" + ctx.Loops.Count + "\" loops are present.");
            return null;
        }

        // End the block with a branch.
        builder.BuildBr(ctx.Loops.ElementAt(new Index((int)NumBreaks - 1)).End);
        return null;

    }

    public override Statement Instantiate(InstantiationInfo info) => this;

    public override string ToString() => NumBreaks == 1 ? "break;" : ("break " + NumBreaks + ";");

}