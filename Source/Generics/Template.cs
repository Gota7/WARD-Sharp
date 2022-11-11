using System.Diagnostics.CodeAnalysis;

namespace WARD.Generics;

// Type of template parameter.
public enum TemplateParameterType {
    Value, // An expression/value that results in a type permitted by the concept is passed.
    Typename // A typename that fits a concept.
}

// A template parameter.
public class TemplateParameter {
    public string Name { get; } // Name of the parameter.
    public TemplateParameterType Type { get; } // Type of the parameter.
    public Concept Concept { get; } // What must be followed for the parameter.

    // Create a new template parameter.
    public TemplateParameter(string name, TemplateParameterType type, Concept concept) {
        Name = name;
        Type = type;
        Concept = concept;
    }

    public override bool Equals(object obj) {
        var tp = obj as TemplateParameter;
        if (tp != null) {
            if (!Name.Equals(tp.Name)) return false;
            if (Type != tp.Type) return false;
            return Concept.Equals(tp.Concept);
        }
        return false;
    }

    public override int GetHashCode() {
        HashCode hc = new HashCode();
        hc.Add(Name);
        hc.Add(Type);
        hc.Add(Concept);
        return hc.ToHashCode();
    }

}

// Information needed to instantiate.
public class InstantiationInfo {
    public TemplateParameter[] Parameters { get; } // Template parameters.
    public object[] Instantiations { get; } // Items used to instantiate.

    // Create new instantiation info.
    public InstantiationInfo(TemplateParameter[] parameters, object[] instantiations) {
        Parameters = parameters;
        Instantiations = instantiations;
    }

    // Get a template index from a name.
    public int? TemplateIndex(string name) {
        for (int i = 0; i < Parameters.Length; i++) {
            if (Parameters[i].Name.Equals(name)) return i;
        }
        return null;
    }

}