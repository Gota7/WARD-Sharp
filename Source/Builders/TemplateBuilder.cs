using WARD.Common;
using WARD.Exceptions;
using WARD.Expressions;
using WARD.Generics;
using WARD.Statements;
using WARD.Types;

namespace WARD.Builders;

// For instantiating templates.
public class TemplateBuilder {
    public UnitManager UnitManager { get; } // Unit manager to instantiate templates to.
    public string CurrCompilationUnit; // The current compilation unit to instantiate with.
    public TemplateParameter[] Parameters { get; } // Template parameters used to instance functions or classes.
    private List<FunctionDeclarationInfo> Functions = new List<FunctionDeclarationInfo>(); // Functions to instantiate.

    // Info for declaring a function.
    private class FunctionDeclarationInfo {
        public string Name;
        public VarTypeFunction Signature;
        public Statement Definition;
        public string Scope;
        public ItemAttribute[] Attributes;
        public FunctionDeclarationInfo(string name, VarTypeFunction signature, Statement definition, string scope = "", params ItemAttribute[] attributes) {
            Name = name;
            Signature = signature;
            Definition = definition;
            Scope = scope;
            Attributes = attributes;
        }
    }

    // Create a new template builder.
    public TemplateBuilder(UnitManager unitManager, string currCompilationUnit, params TemplateParameter[] parameters) {
        UnitManager = unitManager;
        CurrCompilationUnit = currCompilationUnit;
        Parameters = parameters;
    }

    // Add a new function to the template scope.
    public void AddFunction(string name, VarTypeFunction signature, Statement definition, string scope = "", params ItemAttribute[] attributes) {
        Functions.Add(new FunctionDeclarationInfo(name, signature, definition, scope, attributes));
    }

    // Create an instantiation of the template with definitions in the current compilation unit.
    public void Instantiate(params object[] parameters) {

        // Make sure all the types are correct.
        if (Parameters.Length != parameters.Length) {
            Error.ThrowInternal("Can not instantiate template with wrong number of parameters. Expected \"" + Parameters.Length + "\" but got \"" + parameters.Length + "\".");
            return;
        }
        for (int i = 0; i < parameters.Length; i++) {
            if (Parameters[i].Type == TemplateParameterType.Value) {
                var expr = parameters[i] as Expression;
                if (expr == null || !expr.Constant()) { // Make sure we got a constant expression parameter.
                    Error.ThrowInternal("Expected constant expression for value type parameter.");
                }
                if (!Parameters[i].Concept.TypeFitsConcept(expr.GetReturnType(), UnitManager.Units[CurrCompilationUnit].Scope)) {
                    Error.ThrowInternal("Expected constant expression does not fit expected concept.");
                }
            } else { // Typename.
                var type = parameters[i] as VarType;
                if (type == null || !Parameters[i].Concept.TypeFitsConcept(type, UnitManager.Units[CurrCompilationUnit].Scope)) {
                    Error.ThrowInternal("Expected type, but it does not fit the given concept parameter.");
                }
            }
        }

        // Create instantiation info.
        InstantiationInfo info = new InstantiationInfo(Parameters, parameters);

        // Instantiate functions.
        foreach (var function in Functions) {
            var func = UnitManager.AddFunction(CurrCompilationUnit, function.Name, function.Signature.Instantiate(info) as VarTypeFunction, function.Scope, function.Attributes);
            func.Define(function.Definition.Instantiate(info));
        }

    }

}