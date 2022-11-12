using LLVMSharp.Interop;
using WARD.Exceptions;
using WARD.Generics;
using WARD.Scoping;

namespace WARD.Statements;

// Statement for continuing a loop.
public class StatementContinue : Statement {
    public uint NumContinues { get; } // How many loops to continue.

    // Create a new continue statement.
    public StatementContinue(uint numContinues = 1) {
        NumContinues = numContinues;
        if (NumContinues == 0) Error.ThrowInternal("Must continue at least 1 loop.");
    }

    public override void SetScopes(Scope parent) {}

    public override void ResolveVariables() {}

    public override void ResolveTypes() {}

    public override bool ReturnsType() => false;

    public override bool EndsBlock() => true;

    public override void CompileDeclarations(LLVMModuleRef mod, LLVMBuilderRef builder) {}

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder, CompilationContext ctx) {

        // Make sure we are the necessary loops in.
        if (ctx.Loops.Count < NumContinues) {
            Error.ThrowInternal("Can not continue \"" + NumContinues + "\" loops when only \"" + ctx.Loops.Count + "\" loops are present.");
            return null;
        }

        //End block with a continue.
        builder.BuildBr(ctx.Loops.ElementAt(new Index((int)NumContinues - 1)).Cont);
        return null;

    }

    public override Statement Instantiate(InstantiationInfo info) => this;

    public override string ToString() => NumContinues == 1 ? "continue;" : ("continue " + NumContinues + ";");

}