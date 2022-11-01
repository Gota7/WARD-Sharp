using LLVMSharp.Interop;
using WARD.Common;
using WARD.Exceptions;
using WARD.Expressions;

namespace WARD.Types;

// A general type of a variable. Things like constants and access are more of a variable thing.
public abstract partial class VarType : IEqualityComparer<VarType> {
    public DataAccessFlags AccessFlags { get; protected set; } // How the type can be accessed.
    private bool TypeNotGotten = true; // If LLVM type cache clear.
    private LLVMTypeRef GottenType = null; // Cache LLVM type if already fetched.

    // Create a new variable type.
    public VarType(DataAccessFlags accessFlags) {
        AccessFlags = accessFlags;
    }

    // Get the LLVM type.
    public LLVMTypeRef GetLLVMType() {
        if (TypeNotGotten) {
            GottenType = LLVMType();
        }
        if (GottenType == null) Error.ThrowInternal(ToString() + " was not able to be resolved to an LLVM type.");
        return GottenType;
    }

    // If this type is equal to another one.
    public bool Equals(VarType a, VarType b) {
        return a.Equals(b);
    }

    // If this type is equal to another one.
    public override bool Equals(object other) {
        VarType t = other as VarType;
        if (t != null) {
            if (AccessFlags != t.AccessFlags) return false;
            return Equals(t);
        }
        return false;
    }

    // Get hash code.
    public int GetHashCode(VarType v) => v.GetHashCode();

    // Get true underlying type.
    public abstract VarType GetVarType();

    // For each member to get the LLVM type.
    protected abstract LLVMTypeRef LLVMType();

    // Get the mangled version.
    public abstract string Mangled();

    // Equals.
    protected abstract bool Equals(VarType other);

    // Hash code.
    public abstract override int GetHashCode();

    // Convert to string.
    public abstract override string ToString();

    // Get the default value of the type.
    public abstract Expression DefaultValue();

}