using LLVMSharp.Interop;
using WARD.Exceptions;
using WARD.Generics;
using WARD.Scoping;
using WARD.Types;

namespace WARD.Statements;

// Context for a loop.
public class LoopContext {
    public LLVMBasicBlockRef Start { get; } // Block that contains the actual loop (what repeats every time).
    public LLVMBasicBlockRef Cont { get; } // Block that contains code that is ran on every loop, even if continue is used.
    public LLVMBasicBlockRef End { get; } // Block that exits the loop.

    // Create a new loop context.
    public LoopContext(LLVMBasicBlockRef start, LLVMBasicBlockRef cont, LLVMBasicBlockRef end) {
        Start = start;
        Cont = cont;
        End = end;
    }

};

// Context for compiling.
public class CompilationContext {
    public LLVMValueRef Func { get; } // LLVM function.
    public Stack<LoopContext> Loops { get; } = new Stack<LoopContext>(); // Loops currently contained.

    // Create a new compilation context.
    public CompilationContext(LLVMValueRef func) {
        Func = func;
    }

}

// A statement that counts as a function definition.
public abstract class Statement {

    // Set scope for the statement.
    public abstract void SetScopes(Scope parent);

    // Resolve variables in the statement.
    public abstract void ResolveVariables();

    // Resolve types in the statement.
    public abstract void ResolveTypes();

    // If the statement returns a type (if it does itself or all its children do).
    public abstract bool ReturnsType();

    // If the statement is the end of a basic block.
    public abstract bool EndsBlock();

    // Compile any variable definitions.
    public abstract void CompileDeclarations(LLVMModuleRef mod, LLVMBuilderRef builder);

    // Compile the statement. Note that the returned value is only needed for expressions.
    public abstract LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder, CompilationContext ctx);

    // Instantiate this statement if it is generic or has generic components.
    public abstract Statement Instantiate(InstantiationInfo info);

    // Get the statement as a string.
    public override abstract string ToString();

}