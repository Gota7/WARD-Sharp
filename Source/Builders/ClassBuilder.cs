using WARD.Statements;
using WARD.Types;

namespace WARD.Builders;

// For building classes spread across multiple compilation units. TODO: INHERITANCE AND STUFF!!!
public class ClassBuilder {
    private UnitManager UnitManager = new UnitManager(); // Contains compilation units.
    public string ClassName { get; } // Name of the class to build.
    public string CurrCompilationUnit { get; private set; } // The current compilation unit to build.

    // Create a new class builder.
    public ClassBuilder(string className, string defaultCompilationUnit, VarTypeStruct members) {
        ClassName = className;
        CurrCompilationUnit = defaultCompilationUnit;
    }

    // Add a compilation unit. Do these before any functions are added.
    public void AddCompilationUnit(string compilationUnit) {
        UnitManager.AddUnit(compilationUnit);
    }

    // Set the current compilation unit.
    public void SetCompilationUnit(string compilationUnit) {
        CurrCompilationUnit = compilationUnit;
    }

    // Create a static function.
    public Function AddStaticFunction() {
        return null;
    }

    // Create a standard function.
    public Function AddThisCallFunction() {
        return null;
    }

}