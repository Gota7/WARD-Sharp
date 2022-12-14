using LLVMSharp.Interop;
using WARD.Common;
using WARD.Expressions;
using WARD.Generics;
using WARD.Scoping;

namespace WARD.Types;

// Predefined types.
public partial class VarType {
    public static VarTypeInteger Int8_t { get; } = new VarTypeInteger(true, 8);
    public static VarTypeInteger UInt8_t { get; } = new VarTypeInteger(false, 8);
    public static VarTypeInteger Int16_t { get; } = new VarTypeInteger(true, 16);
    public static VarTypeInteger UInt16_t { get; } = new VarTypeInteger(false, 16);
    public static VarTypeInteger Int32_t { get; } = new VarTypeInteger(true, 32);
    public static VarTypeInteger UInt32_t { get; } = new VarTypeInteger(false, 32);
    public static VarTypeInteger Int64_t { get; } = new VarTypeInteger(true, 64);
    public static VarTypeInteger UInt64_t { get; } = new VarTypeInteger(false, 64);
    public static VarTypeInteger SByte { get; } = Int8_t;
    public static VarTypeInteger Byte { get; } = UInt8_t;
    public static VarTypeInteger Short { get; } = Int16_t;
    public static VarTypeInteger UShort { get; } = UInt16_t;
    public static VarTypeInteger Int { get; } = Int32_t;
    public static VarTypeInteger UInt { get; } = UInt32_t;
    public static VarTypeInteger Long { get; } = Int64_t;
    public static VarTypeInteger ULong { get; } = UInt64_t;

}

// Integer type.
public class VarTypeInteger : VarType {
    public bool Signed { get; } // If the integer is signed or unsigned.
    public uint BitWidth { get; } // How many bits the integer has.

    // Create a new integer (signed for negative values, bitwidth for how many bits including signed bit).
    public VarTypeInteger(bool signed, uint bitWidth, DataAccessFlags accessFlags = DataAccessFlags.RW) : base(accessFlags) {
        Signed = signed;
        BitWidth = bitWidth;
    }

    public override VarType GetVarType(Scope scope) => this;
    protected override LLVMTypeRef LLVMType(Scope scope) => LLVMTypeRef.CreateInt(BitWidth);
    public override string Mangled() => (Signed ? "s" : "u") + BitWidth.ToString() + "E";

    protected override bool Equals(VarType other, Scope scope) {
        var o = other.GetVarType(scope) as VarTypeInteger;
        if (o != null) {
            return Signed == o.Signed && BitWidth == o.BitWidth;
        }
        return false;
    }

    public override string ToString() {
        return (Signed ? "s" : "u") + BitWidth.ToString();
    }

    public override Expression DefaultValue(Scope scope) => new ExpressionConstInt(this, 0);

    public override VarType Instantiate(InstantiationInfo info) => this;

}