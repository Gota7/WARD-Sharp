using LLVMSharp.Interop;
using WARD.Common;
using WARD.Expressions;
using WARD.Scoping;
using WARD.Statements;
using WARD.Types;

namespace WARD.Builders;

// For building individual compilation units.
public class UnitBuilder {
    private List<Function> Functions = new List<Function>(); // Functions that are managed by this unit builder.
    private List<Variable> Globals = new List<Variable>(); // Global variables managed by this unit builder.
    public Scope Scope { get; } = new Scope();
    public string Path { get; } // Path of the unit.
    internal LLVMModuleRef LLVMModule { get; } // Stored LLVM module.

    // Create a new unit builder.
    public UnitBuilder(string path) {
        Path = path;
        LLVMModule = LLVMModuleRef.CreateWithName(path);
    }

    // Add a new function to the scope table.
    public Function AddFunction(string name, VarTypeFunction signature, params ItemAttribute[] attributes) {
        var func = new Function(this, name, signature, attributes);
        Functions.Add(func);
        return func;
    }

    // Add a global variable. TODO: ALLOW CONSTANT EXPRESSIONS?
    public LLVMValueRef AddGlobal(string name, VarType type, Expression value = null) {
        if (value != null) throw new System.NotImplementedException();
        var variable = new Variable(name, type);
        Scope.Table.AddVariable(variable);
        Globals.Add(variable);
        return Globals.Last().Value;
    }

    // Add a type alias.
    public void AddAlias(string name, VarType type) {
        Scope.Table.AddType(name, type);
    }

    // Compile the unit.
    public LLVMModuleRef Compile(LLVMPassManagerRef fpm) {

        // Go through global definitions.
        foreach (var global in Globals) {
            global.Value = LLVMModule.AddGlobal(global.Type.GetLLVMType(Scope), global.Name);
        }

        // Go through function declarations first.
        foreach (var function in Functions) {
            if (!function.Inline) function.Value = LLVMModule.AddFunction(function.Name, function.Type.GetLLVMType(Scope));
        }

        // Define each function now.
        LLVMBuilderRef builder = LLVMBuilderRef.Create(LLVMModule.Context);
        foreach (var function in Functions) {
            if (!function.Inline) function.Compile(LLVMModule, builder, fpm);
        }

        // Return build module.
        return LLVMModule;

    }

}