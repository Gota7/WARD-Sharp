using LLVMSharp.Interop;
using WARD.Common;
using WARD.Exceptions;
using WARD.Expressions;
using WARD.Statements;
using WARD.Types;

namespace WARD.Builders;

// It can be a pain to manage declarations across different compilation units. The Unit manager simplifies this.
public class UnitManager {
    internal Dictionary<string, UnitBuilder> Units = new Dictionary<string, UnitBuilder>(); // Compilation units.

    // Add a new compilation unit. Ideally you should add all your compilation units before you add functions.
    public void AddUnit(string path) {
        if (Units.ContainsKey(path)) {
            Error.ThrowInternal("Compilation unit \"" + path + "\" already exists.");
            return;
        }
        Units.Add(path, new UnitBuilder(path));
    }

    // Add a new function. This will declare it for all currently embedded compilation units but only return the function it is to be defined in.
    public Function AddFunction(string definingCompilationUnit, string name, VarTypeFunction signature, string scope = "", params ItemAttribute[] attributes) {
        if (!Units.ContainsKey(definingCompilationUnit)) {
            Error.ThrowInternal("Unit manager does not contain the \"" + definingCompilationUnit + "\" compilation unit.");
            return null;
        }
        Function func = null;
        foreach (var unit in Units) {
            var addedFunc = unit.Value.AddFunction(name, signature, scope, attributes);
            if (unit.Key.Equals(definingCompilationUnit)) func = addedFunc;
        }
        return func;
    }

    // Add a new global. This will declare it for all currently embedded compilation units but only define it for what is needed.
    public LLVMValueRef AddGlobal(string definingCompilationUnit, string name, VarType type, Expression value = null, string scope = "") {
        if (!Units.ContainsKey(definingCompilationUnit)) {
            Error.ThrowInternal("Unit manager does not contain the \"" + definingCompilationUnit + "\" compilation unit.");
            return null;
        }
        LLVMValueRef val = null;
        foreach (var unit in Units) {
            if (unit.Key.Equals(definingCompilationUnit)) {
                val = unit.Value.AddGlobal(name, type, value, scope);
            } else {
                var tmp = unit.Value.AddGlobal(name, type, null, scope);
                tmp.IsExternallyInitialized = true;
            }
        }
        return val;
    }

    // Add an alias.
    public void AddAlias(string name, VarType type, string scope = "") {
        foreach (var unit in Units) {
            unit.Value.AddAlias(name, type, scope);
        }
    }

    // Get the compilation units.
    public List<UnitBuilder> GetUnits() {
        var ret = new List<UnitBuilder>();
        foreach (var unit in Units) {
            ret.Add(unit.Value);
        }
        return ret;
    }

}