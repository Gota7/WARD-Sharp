using LLVMSharp.Interop;
using WARD.Exceptions;
using WARD.Generics;
using WARD.Scoping;
using WARD.Types;

namespace WARD.Statements;

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

    // Compile any variable definitions.
    public abstract void CompileDeclarations(LLVMModuleRef mod, LLVMBuilderRef builder);

    // Compile the expression.
    public abstract LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder);

    // Instantiate this statement if it is generic or has generic components.
    public abstract Statement Instantiate(InstantiationInfo info);

}