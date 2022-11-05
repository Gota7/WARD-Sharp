using LLVMSharp.Interop;
using WARD.Exceptions;
using WARD.Scoping;
using WARD.Statements;
using WARD.Types;

namespace WARD.Expressions;

// Low level operation that can be compiled directly to LLVM bitcode.
public abstract class Expression : Statement {
    public Scope Scope { get; protected set; } // Scope for this item.

    public override void SetScopes(Scope parent) {
        Scope = parent; // Expression doesn't change scope.
    }

    // Resolve types, type check, add casts, and solidify all function references. The parameters are so calls can have expressions resolve the correct function.
    public virtual void ResolveTypes(VarType preferredReturnType, List<VarType> parameterTypes) {}

    // Resolve variables.
    public override void ResolveVariables() {}

    // Resolve types.
    public override void ResolveTypes() => ResolveTypes(null, null);

    // Get the return type. Use this instead of ReturnType.
    public VarType GetReturnType() => ReturnType().GetVarType(Scope);

    // Get the return type of an expression.
    protected abstract VarType ReturnType();

    // If the expression type is constant.
    public abstract bool Constant();

    // Compile any variable definitions.
    public override void CompileDeclarations(LLVMModuleRef mod, LLVMBuilderRef builder) {}

    // Statements are not return values.
    public override bool ReturnsType(VarType type, out bool outExplicitVoidReturn) {
        outExplicitVoidReturn = false;
        return type.Equals(VarType.Void, Scope);
    }

}