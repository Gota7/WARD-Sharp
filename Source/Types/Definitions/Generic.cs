using LLVMSharp.Interop;
using WARD.Common;
using WARD.Exceptions;
using WARD.Expressions;
using WARD.Generics;
using WARD.Scoping;

namespace WARD.Types;

// Type to be instantiated with a template.
public class VarTypeGeneric : VarType {
    public string Template { get; } // Name of the template.

    // Create a new generic type.
    public VarTypeGeneric(string template, DataAccessFlags accessFlags = DataAccessFlags.RW) : base(accessFlags) {
        Template = template;
    }

    public override VarType GetVarType(Scope scope) => this;

    protected override LLVMTypeRef LLVMType(Scope scope) {
        Error.ThrowInternal("Can not get definitive type of uninstantiated generic \"" + Template + "\".");
        return null;
    }

    public override string Mangled() => "t" + Template.Length + Template;

    protected override bool Equals(VarType other, Scope scope) {
        var otherTemplate = other as VarTypeGeneric;
        if (otherTemplate != null) return otherTemplate.Template.Equals(Template);
        return false;
    }

    public override string ToString() {
        return Template;
    }

    public override Expression DefaultValue(Scope scope) {
        Error.ThrowInternal("Can not get default type of uninstantiated generic \"" + Template + "\".");
        return null;
    }

    public override VarType Instantiate(InstantiationInfo info) {
        int? indexN = info.TemplateIndex(Template);
        if (indexN == null) return this;
        int index = indexN.Value;
        if (info.Parameters[index].Type != TemplateParameterType.Typename) {
            Error.ThrowInternal("Can not instantiate generic \"" + Template + "\" with a non-type.");
            return null;
        }
        return info.Instantiations[index] as VarType;
    }

}