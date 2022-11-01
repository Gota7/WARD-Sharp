using LLVMSharp.Interop;
using WARD.Types;

namespace WARD.Common;

// Contains information about a variable.
public class Variable {
    public string Name { get; internal set; } // Name of the variable.
    public VarType Type { get; } // Type of the variable.
    public LLVMValueRef Value; // Actual value to modify. This is a stack allocated L-value.

    // Create a new variable. WARNING: This does not append it to the scope table!
    public Variable(string name, VarType type) {
        Name = name;
        Type = type;
    }

}