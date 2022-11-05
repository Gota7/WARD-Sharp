using LLVMSharp.Interop;
using WARD.Common;
using WARD.Expressions;
using WARD.Scoping;

namespace WARD.Types;

// Alias to another type.
public class VarTypeAlias : VarType {
    public string Referenced { get; } // Referenced variable type.

    // Create a new alias type.
    public VarTypeAlias(string referenced, DataAccessFlags accessFlags = DataAccessFlags.RW) : base(accessFlags) {
        Referenced = referenced;
    }

    public override VarType GetVarType(Scope scope) => scope.Table.ResolveType(Referenced);
    protected override LLVMTypeRef LLVMType(Scope scope) => GetVarType(scope).GetLLVMType(scope);
    public override string Mangled() {
        string ret = "B";
        foreach (var item in Referenced.Split(".")) {
            ret += item.Length + item;
        }
        return ret + "E";
    }

    protected override bool Equals(VarType other, Scope scope) {
        return GetVarType(scope).Equals(other, scope);
    }

    public override string ToString() {
        return Referenced;
    }

    public override Expression DefaultValue(Scope scope) => GetVarType(scope).DefaultValue(scope);

}