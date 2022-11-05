using LLVMSharp.Interop;
using WARD.Common;
using WARD.Exceptions;
using WARD.Expressions;
using WARD.Scoping;

namespace WARD.Types;

// Simple variable types.
public enum VarTypeSimpleEnum {
    Bool, // For data that is either true or false.
    Object, // Any type of data.
    Void // No data.
}

// Simple type declarations.
public partial class VarType {
    public static VarTypeSimple Bool = new VarTypeSimple(VarTypeSimpleEnum.Bool);
    public static VarTypeSimple Object = new VarTypeSimple(VarTypeSimpleEnum.Object);
    public static VarTypeSimple Void = new VarTypeSimple(VarTypeSimpleEnum.Void);

    // If a type is void.
    public bool IsVoid() {
        var simple = this as VarTypeSimple;
        if (simple != null && simple.SimpleType == VarTypeSimpleEnum.Void) return true;
        return false;
    }

}

// Simple type.
public class VarTypeSimple : VarType {
    public VarTypeSimpleEnum SimpleType { get; } // Type of simple primitive to compile.

    // Create a new simple primitive.
    public VarTypeSimple(VarTypeSimpleEnum simpleType, DataAccessFlags accessFlags = DataAccessFlags.RW) : base(accessFlags) {
        SimpleType = simpleType;
    }

    public override VarType GetVarType(Scope scope) => this;
    protected override LLVMTypeRef LLVMType(Scope scope) {
        switch (SimpleType) {
            case VarTypeSimpleEnum.Bool: return LLVMTypeRef.Int1;
            case VarTypeSimpleEnum.Object: return LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0);
            case VarTypeSimpleEnum.Void: return LLVMTypeRef.Void;
            default:
                Error.ThrowInternal("Unknown simple primitive variable type.");
                return null;
        }
    }
    public override string Mangled() {
        switch (SimpleType) {
            case VarTypeSimpleEnum.Bool: return "b";
            case VarTypeSimpleEnum.Object: return "o";
            case VarTypeSimpleEnum.Void: return "v";
            default:
                Error.ThrowInternal("Unknown simple primitive variable type.");
                return null;
        }
    }

    protected override bool Equals(VarType other, Scope scope) {
        var o = other.GetVarType(scope) as VarTypeSimple;
        if (o != null) {
            return SimpleType == o.SimpleType;
        }
        return false;
    }

    public override string ToString() {
        switch (SimpleType) {
            case VarTypeSimpleEnum.Bool: return "bool";
            case VarTypeSimpleEnum.Object: return "object";
            case VarTypeSimpleEnum.Void: return "void";
            default:
                Error.ThrowInternal("Unknown simple primitive variable type.");
                return null;
        }
    }

    public override Expression DefaultValue(Scope scope) {
        switch (SimpleType) {
            case VarTypeSimpleEnum.Bool: return new ExpressionConstBool(false);
            default:
                Error.ThrowInternal("Simple type\"" + SimpleType.ToString() + "\" can not have a default value.");
                return null;
        }
    }

}