using LLVMSharp.Interop;
using WARD.Common;
using WARD.Exceptions;
using WARD.Expressions;
using WARD.Generics;
using WARD.Scoping;

namespace WARD.Types;

// A reference to an L-value. This is basically a pointer but acts like the type it points to. TODO: HAVE INSTANTIATION WORK FOR NONREFERENCES?
public class VarTypeLValueReference : VarType {
    public VarType Referenced { get; } // Type that is referenced to.

    // Create a new L-value reference type.
    public VarTypeLValueReference(VarType referenced, DataAccessFlags accessFlags = DataAccessFlags.RW) : base(accessFlags) {
        Referenced = referenced;
        if (referenced as VarTypeLValueReference != null)
        {
            Error.ThrowInternal("An L-value reference can not reference another L-value reference.");
        }
    }

    public override VarType GetVarType(Scope scope) => this;
    protected override LLVMTypeRef LLVMType(Scope scope) => LLVMTypeRef.CreatePointer(Referenced.GetLLVMType(scope), 0);
    public override string Mangled() => "L" + Referenced.Mangled();

    protected override bool Equals(VarType other, Scope scope) {
        var o = other.GetVarType(scope) as VarTypeLValueReference;
        if (o != null) {
            return Referenced.Equals(o.Referenced, scope);
        }
        return false;
    }

    public override string ToString() {
        return Referenced.ToString() + "&";
    }

    public override Expression DefaultValue(Scope scope) {
        Error.ThrowInternal("A reference type can not have a default value.");
        return null;
    }

    public override VarType Instantiate(InstantiationInfo info) => new VarTypeLValueReference(Referenced.Instantiate(info), AccessFlags);

}