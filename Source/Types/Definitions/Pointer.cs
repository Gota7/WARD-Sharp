using LLVMSharp.Interop;
using WARD.Common;
using WARD.Expressions;

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

    public override VarType GetVarType() => this;
    protected override LLVMTypeRef LLVMType() => LLVMTypeRef.CreatePointer(PointedTo.GetLLVMType(), 0);
    public override string Mangled() => "p" + PointedTo.Mangled();

    protected override bool Equals(VarType other) {
        var o = other as VarTypePointer;
        if (o != null) {
            return PointedTo.Equals(o.PointedTo);
        }
        return false;
    }

    public override int GetHashCode() {
        HashCode ret = new HashCode();
        ret.Add(PointedTo);
        return ret.ToHashCode();
    }

    public override string ToString() {
        return PointedTo.ToString() + "*";
    }

    public override Expression DefaultValue() => NullPointer;

}