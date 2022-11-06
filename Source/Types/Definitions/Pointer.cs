using LLVMSharp.Interop;
using WARD.Common;
using WARD.Expressions;
using WARD.Generics;
using WARD.Scoping;

namespace WARD.Types;

// Predefined types.
public partial class VarType {
    public static VarTypePointer String = new VarTypePointer(VarType.Int8_t, DataAccessFlags.Read); // Standard string.
}

// Pointer type.
public class VarTypePointer : VarType {
    public VarType PointedTo { get; } // Type that is pointed to.

    // Null pointer value.
    public ExpressionConstPointer NullPointer => new ExpressionConstPointer(this);

    // Create a new pointer type.
    public VarTypePointer(VarType pointedTo, DataAccessFlags accessFlags = DataAccessFlags.RW) : base(accessFlags) {
        PointedTo = pointedTo;
    }

    public override VarType GetVarType(Scope scope) => this;
    protected override LLVMTypeRef LLVMType(Scope scope) => LLVMTypeRef.CreatePointer(PointedTo.GetLLVMType(scope), 0);
    public override string Mangled() => "p" + PointedTo.Mangled();

    protected override bool Equals(VarType other, Scope scope) {
        var o = other.GetVarType(scope) as VarTypePointer;
        if (o != null) {
            return PointedTo.Equals(o.PointedTo, scope);
        }
        return false;
    }

    public override string ToString() {
        return PointedTo.ToString() + "*";
    }

    public override Expression DefaultValue(Scope scope) => NullPointer;

    public override VarType Instantiate(InstantiationInfo info) => new VarTypePointer(PointedTo.Instantiate(info), AccessFlags);

}