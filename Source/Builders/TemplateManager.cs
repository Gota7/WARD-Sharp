using WARD.Generics;

namespace WARD.Builders;

// Collection of template parameters.
public class TemplateParameterCollection {
    public TemplateParameter[] Parameters { get; } // Parameters contained.

    // Create a new collection of template parameters.
    public TemplateParameterCollection(params TemplateParameter[] parameters) {
        Parameters = parameters;
    }

    public override bool Equals(object obj) {
        var tpc = obj as TemplateParameterCollection;
        if (tpc != null) {
            if (Parameters.Length != tpc.Parameters.Length) return false;
            for (int i = 0; i < Parameters.Length; i++) {
                if (!Parameters[i].Equals(tpc.Parameters[i])) return false;
            }
            return true;
        }
        return false;
    }

    public override int GetHashCode() {
        HashCode hc = new HashCode();
        hc.Add(Parameters.Length);
        foreach (var parameter in Parameters) {
            hc.Add(parameter);
        }
        return hc.ToHashCode();
    }

}

// It can be tricky to manage different templates if some parameters are the same. The template manager simplifies this.
public class TemplateManager {
    public UnitManager UnitManager { get; } // Unit manager to build with,
    private string m_CurrCompilationUnit; // True parameter.
    public string CurrCompilationUnit {
        get => m_CurrCompilationUnit;
        set {
            m_CurrCompilationUnit = value;
            foreach (var tb in Templates.Values) {
                tb.CurrCompilationUnit = value;
            }
        }
    } // Current compilation unit to use.
    internal Dictionary<TemplateParameterCollection, TemplateBuilder> Templates = new Dictionary<TemplateParameterCollection, TemplateBuilder>(); // Templates that exist in the manager.

    // Create a new template manager.
    public TemplateManager(UnitManager unitManager, string currCompilationUnit) {
        UnitManager = unitManager;
        CurrCompilationUnit = currCompilationUnit;
    }

    // Get or add a builder based on template parameters.
    public TemplateBuilder GetOrAddBuilder(params TemplateParameter[] templateParameters) {
        TemplateParameterCollection tpc = new TemplateParameterCollection(templateParameters);
        if (Templates.ContainsKey(tpc)) return Templates[tpc];
        TemplateBuilder tb = new TemplateBuilder(UnitManager, CurrCompilationUnit, templateParameters);
        Templates.Add(tpc, tb);
        return tb;
    }

}