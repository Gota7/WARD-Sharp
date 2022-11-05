using WARD.Statements;

namespace WARD.Builders;

// For building classes spread across multiple compilation units. TODO!!!
public class ClassBuilder {
    public string ClassName { get; } // Name of the class to build.

    // Create a new class builder.
    public ClassBuilder(string className) {
        ClassName = className;
    }

    // Create a "this call" function.
    public Function AddThisCallFunction() {
        return null;
    }

}