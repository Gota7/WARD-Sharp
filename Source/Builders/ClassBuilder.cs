using WARD.Common;
using WARD.Statements;
using WARD.Types;

namespace WARD.Builders;

// For building classes spread across multiple compilation units. TODO: INHERITANCE AND STUFF!!!
public class ClassBuilder {
    public UnitManager UnitManager { get; } = new UnitManager(); // To share with other compilation units.
    public string ClassName { get; } // Name of the class to build.
    public string CurrCompilationUnit; // The current compilation unit to build.
    public VarTypeStruct Members { get; } // Contained members of the class.
    public string Scope { get; } // Scope of the defined class.

    // Create a new class builder. Scope is the scope that will contain the new class.
    public ClassBuilder(UnitManager unitManager, string className, string defaultCompilationUnit, VarTypeStruct members, string scope = "") {
        UnitManager = unitManager;
        ClassName = className;
        CurrCompilationUnit = defaultCompilationUnit;
        Members = members;
        if (scope.Equals("")) {
            Scope = "%CLASS%_" + className;
        } else {
            Scope = scope + ".%CLASS%_" + className;
        }
        unitManager.AddAlias(className, members, scope);
    }

    // Add a compilation unit. Do these before any functions are added.
    public void AddCompilationUnit(string compilationUnit) {
        UnitManager.AddUnit(compilationUnit);
    }

    // Create a static function. It's really just a standard function within the class's scope table.
    public Function AddStaticFunction(string name, VarTypeFunction signature, params ItemAttribute[] attributes) {
        return UnitManager.AddFunction(CurrCompilationUnit, name, signature, Scope, attributes);
    }

    // Create a standard function.
    public Function AddThisCallFunction(string name, VarTypeFunction signature, params ItemAttribute[] attributes) {

        // We need to create a new signature that uses __thiscall. We do this by adding a new parameter that represents "this".
        List<Variable> parameters = new List<Variable>();
        parameters.Add(new Variable("this", new VarTypePointer(Members, DataAccessFlags.Read)));
        parameters.AddRange(signature.Parameters);
        return AddStaticFunction(name, new VarTypeFunction(signature.ReturnType, signature.VariadicType, parameters.ToArray()));

    }

}