using LLVMSharp.Interop;
using WARD.Common;
using WARD.Exceptions;
using WARD.Expressions;
using WARD.Scoping;

namespace WARD.Types;

// A general type of a variable. Things like constants and access are more of a variable thing.
public abstract partial class VarType {
    public DataAccessFlags AccessFlags { get; protected set; } // How the type can be accessed.
    private bool TypeNotGotten = true; // If LLVM type cache clear.
    private LLVMTypeRef GottenType = null; // Cache LLVM type if already fetched.

    // Create a new variable type.
    public VarType(DataAccessFlags accessFlags) {
        AccessFlags = accessFlags;
    }

    // Get the LLVM type.
    public LLVMTypeRef GetLLVMType(Scope scope) {
        if (TypeNotGotten) {
            GottenType = LLVMType(scope);
        }
        if (GottenType == null) Error.ThrowInternal(ToString() + " was not able to be resolved to an LLVM type.");
        return GottenType;
    }

    // If this type is equal to another one.
    public bool Equals(object other, Scope scope) {
        VarType t = other as VarType;
        if (t != null) {
            if (AccessFlags != t.AccessFlags) return false;
            return Equals(t, scope);
        }
        return false;
    }

    // To prevent improper usage of equals.
    public override bool Equals(object obj) {
        throw new System.Exception("Do not use VarType.Equals(object)!");
    }

    // Get hash code.
    public override int GetHashCode() => 0; // We should never do this.

    // Get true underlying type.
    public abstract VarType GetVarType(Scope scope);

    // For each member to get the LLVM type.
    protected abstract LLVMTypeRef LLVMType(Scope scope);

    // Get the mangled version.
    public abstract string Mangled();

    // Equals.
    protected abstract bool Equals(VarType other, Scope scope);

    // Convert to string.
    public abstract override string ToString();

    // Get the default value of the type.
    public abstract Expression DefaultValue(Scope scope);

}