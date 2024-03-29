using LLVMSharp.Interop;
using WARD.Builders;
using WARD.Common;
using WARD.Exceptions;
using WARD.Scoping;
using WARD.Types;

namespace WARD.Statements;

// Create a new function.
public class Function : Variable {
    private LLVMValueRef CompiledVal = null; // If function has already been compiled.
    public Scope Scope { get; internal set; } // Scope for items within the function.
    public string FuncName { get; } // Actual name of the function, Name is reserved for the mangled version.
    public ItemAttribute[] Attributes { get; } // Function attributes for additional rules for handling.
    public bool Inline => Attributes.Where(x => x.Name.Equals("Inline")).Count() > 0; // If the function serves as a macro rather than being defined.
    public Statement Definition { get; internal set; } = null; // Function definition if one exists.
    public bool NameMangled => !FuncName.Equals("main") && Attributes.Where(x => x.Name.Equals("NoMangle")).Count() < 1; // If the function's name is mangled.
    public override string ToString() => NameMangled ? ("_W" + Scope.Parent.Mangled() + FuncName.Length + FuncName + "E" + Type.Mangled()) : Name;

    // Create a new function. Automatically adds it to the unit builder's scope table.
    public Function(UnitBuilder builder, string name, VarTypeFunction signature, string scope = "", params ItemAttribute[] attributes) : base(name.Length + name + signature.Mangled(), signature) {
        FuncName = name;
        Attributes = attributes;
        if (!NameMangled) Name = name;
        builder.Scope.Table.AddFunction(this);
        Scope = builder.Scope.EnterScope(scope).EnterScope(name);
        foreach (var param in (Type as VarTypeFunction).Parameters) {
            Scope.Table.AddVariable(new Variable(param.Name, param.Type));
        }
    }

    // Set the function definition.
    public void Define(Statement definition) {
        if (Definition == null) Definition = definition;
        else Error.ThrowInternal("Function \"" + Name + "\" already has a definition.");
    }

    // Compile the function.
    public void Compile(LLVMModuleRef mod, LLVMBuilderRef builder, LLVMPassManagerRef fpm, ConversionSequence conversionSequence) {

        // Check if inline.
        if (Inline) {
            Error.ThrowInternal("Can not compile inlined function \"" + Name + "\".");
            return;
        }

        // Set parameter names.
        for (uint i = 0; i < Value.ParamsCount; i++) {
            Value.Params[i].Name = (Type as VarTypeFunction).Parameters[i].Name;
        }

        // Compile only if has a definition.
        if (Definition == null) return;

        // Create basic block.
        var block = LLVMBasicBlockRef.AppendInContext(mod.Context, Value, "entry");
        builder.PositionAtEnd(block);

        // Shadow parameters and define the rest of the variables.
        uint paramIndex = 0;
        foreach (var param in (Type as VarTypeFunction).Parameters) {
            var value = builder.BuildAlloca(param.Type.GetLLVMType(Scope));
            builder.BuildStore(Value.Params[paramIndex++], value);
            Scope.Table.ResolveVariable(param.Name).Value = value;
        }
        Definition.SetScopes(Scope);
        Definition.CompileDeclarations(mod, builder);

        // Resolve variables and types.
        Definition.ResolveVariables();
        Definition.ResolveTypes();

        // Finally compile the function, and add a return void if needed.
        Definition.Compile(mod, builder, new CompilationContext(Value, conversionSequence));
        if (!Definition.ReturnsType()) {
            if (!(Type as VarTypeFunction).ReturnType.Equals(VarType.Void, Scope)) {
                Error.ThrowInternal("Function \"" + Name + "\" does not return \"" + (Type as VarTypeFunction).ReturnType.ToString() + "\" as expected.");
                return;
            }
            builder.BuildRetVoid();
        }

        // Optimize and verify function.
        Value.VerifyFunction(LLVMVerifierFailureAction.LLVMPrintMessageAction);
        fpm.RunFunctionPassManager(Value);

    }

    // Compile the function inline.
    public LLVMValueRef CompileInline(LLVMModuleRef mod, LLVMBuilderRef builder, params LLVMValueRef[] parameters) {
        throw new System.NotImplementedException();
    }

    // See if a function fits the overload. Distance is how many implicit casts need to be done.
    public bool CallSatisfiesOverload(VarType[] args, Scope scope, out int distance) {
        distance = 0;
        var sig = Type as VarTypeFunction;

        // Variadic check.
        if (args.Length != sig.Parameters.Length) {
            throw new System.NotImplementedException();
        }

        // Check parameters if they match.
        for (int i = 0; i < args.Length; i++) {

            // Case one, type matches.
            if (args[i].Equals(sig.Parameters[i].Type, scope)) {
                continue; // All good!
            }

            // Case two implicit cast possible. Case three, not possible to call.
            else {
                throw new System.NotImplementedException();
            }

        }

        // No incompatibility found.
        return true;

    }

}