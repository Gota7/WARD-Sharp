using LLVMSharp.Interop;
using WARD.Common;
using WARD.Exceptions;
using WARD.Expressions;
using WARD.Generics;
using WARD.Scoping;

namespace WARD.Types;

// Structure type.
public class VarTypeStruct : VarType {
    public Variable[] Members { get; } // Embedded members.

    // Create a new simple primitive.
    public VarTypeStruct(DataAccessFlags accessFlags = DataAccessFlags.RW, params Variable[] members) : base(accessFlags) {
        Members = members;
    }

    public override VarType GetVarType(Scope scope) => this;
    protected override LLVMTypeRef LLVMType(Scope scope) {
        return LLVMTypeRef.CreateStruct(Members.Select(x => x.Type.GetLLVMType(scope)).ToArray(), false);
    }
    public override string Mangled() {
        string ret = "s";
        foreach (var m in Members) {
            ret += m.Type.Mangled();
        }
        return ret + "E";
    }

    protected override bool Equals(VarType other, Scope scope) {
        var o = other.GetVarType(scope) as VarTypeStruct;
        if (o != null) {
            if (Members.Length != o.Members.Length) return false;
            for (int i = 0; i < Members.Length; i++) {
                if (!Members[i].Type.Equals(o.Members[i].Type, scope)) return false;
            }
            return true;
        }
        return false;
    }

    public override string ToString() {
        string ret = "struct {\n";
        foreach (var m in Members) {
            ret += "\t" + m.Type.ToString() + " " + m.Name + ";\n";
        }
        return ret + "};";
    }

    public override Expression DefaultValue(Scope scope) {
        Expression[] expressions = new Expression[Members.Length];
        for (int i = 0; i < Members.Length; i++) {
            expressions[i] = Members[i].Type.GetVarType(scope).DefaultValue(scope);
        }
        return new ExpressionConstStruct(this, expressions);
    }

    public override VarType Instantiate(InstantiationInfo info) {
        Variable[] members = new Variable[Members.Length];
        for (int i = 0; i < members.Length; i++) {
            members[i] = new Variable(Members[i].Name, Members[i].Type.Instantiate(info));
        }
        return new VarTypeStruct(AccessFlags, members);
    }

}