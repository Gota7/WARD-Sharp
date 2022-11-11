using WARD.Common;
using WARD.Generics;
using WARD.Statements;
using WARD.Types;

namespace WARD.Builders;

// For creating operators that interact on types.
public class OperatorBuilder {
    public TemplateManager TemplateManager { get; } // Manager of templates.

    // Create a new operator builder.
    public OperatorBuilder(TemplateManager templateManager) {
        TemplateManager = templateManager;
    }

    // Add a new operator.
    public void AddOperator(string name, VarTypeFunction signature, Statement definition, bool inline = false, params TemplateParameter[] parameters) {
        var tb = TemplateManager.GetOrAddBuilder(parameters);
        if (inline) tb.AddFunction("%OP%_" + name, signature, definition, "", new ItemAttribute("Inline"));
        else tb.AddFunction("%OP%_" + name, signature, definition);
    }

}