using LLVMSharp.Interop;
using WARD.Exceptions;
using WARD.Generics;
using WARD.Statements;
using WARD.Types;

namespace WARD.Expressions;

// Expression to be instantiated with a template.
public class ExpressionGeneric : Expression {
    public string Template { get; } // Name of the template.

    // Create a new generic expression.
    public ExpressionGeneric(string template) {
        Template = template;
    }

    public override void ResolveVariables() {
        Error.ThrowInternal("Can not resolve variables for uninstantiated generic \"" + Template + "\".");
    }

    public override void ResolveTypes(VarType preferredReturnType, List<VarType> parameterTypes) {
        Error.ThrowInternal("Can not resolve type for uninstantiated generic \"" + Template + "\".");
    }

    protected override VarType ReturnType() {
        Error.ThrowInternal("Can not get return type for uninstantiated generic \"" + Template + "\".");
        return null;
    }

    public override bool Constant() {
        Error.ThrowInternal("Can not resolve variables for uninstantiated generic \"" + Template + "\".");
        return true;
    }

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder, CompilationContext ctx) {
        Error.ThrowInternal("Can not compile uninstantiated generic \"" + Template + "\".");
        return null;
    }

    public override string ToString() => Template;

    public override Statement Instantiate(InstantiationInfo info) {
        int? indexN = info.TemplateIndex(Template);
        if (indexN == null) return this;
        int index = indexN.Value;
        if (info.Parameters[index].Type != TemplateParameterType.Value) {
            Error.ThrowInternal("Can not instantiate generic \"" + Template + "\" with a non-expression.");
            return null;
        }
        return info.Instantiations[index] as Expression;
    }

}