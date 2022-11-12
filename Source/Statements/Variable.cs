using LLVMSharp.Interop;
using WARD.Common;
using WARD.Exceptions;
using WARD.Expressions;
using WARD.Generics;
using WARD.Scoping;
using WARD.Types;

namespace WARD.Statements;

// Declares and possibly defines variables of the same type. TODO: CAST DEFINITION TO VARIABLE TYPE!
public class StatementVariable : Statement {
    public Scope Scope { get; private set; } // Scope.
    public Variable[] Vars { get; } // Created variables.
    public Expression Definition { get; private set; } // Definition of the variables.

    // Create a new variable statement.
    public StatementVariable(VarType type, Expression definition = null, params string[] names) {
        if (names.Length <= 0) {
            Error.ThrowInternal("Variable statement must declare at least one variable.");
            return;
        }
        Vars = new Variable[names.Length];
        Definition = definition;
        for (int i = 0; i < names.Length; i++) {
            Vars[i] = new Variable(names[i], type);
        }
    }

    public override void SetScopes(Scope parent) {
        Scope = parent;
        foreach (var var in Vars) {
            Scope.Table.AddVariable(var);
        }
        if (Definition == null) Definition = Vars[0].Type.GetVarType(Scope).DefaultValue(Scope);
        Definition.SetScopes(Scope);
    }

    public override void ResolveVariables() {
        Definition.ResolveVariables();
    }

    public override void ResolveTypes() {
        Definition.ResolveTypes();
        if (!Definition.GetReturnType().Equals(Vars[0].Type, Scope)) {
            Error.ThrowInternal("Variable definition return type \"" + Definition.GetReturnType().ToString() + "\" does not equal variable return type \"" + Vars[0].Type.ToString() + "\".");
        }
    }

    public override bool ReturnsType() => false;

    public override bool EndsBlock() => false;

    public override void CompileDeclarations(LLVMModuleRef mod, LLVMBuilderRef builder) {
        foreach (var var in Vars) {
            var.Value = builder.BuildAlloca(var.Type.GetVarType(Scope).GetLLVMType(Scope));
        }
        Definition.CompileDeclarations(mod, builder);
    }

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder, CompilationContext ctx) {
        LLVMValueRef defaultVal = Definition.Compile(mod, builder, ctx);
        foreach (var var in Vars) {
            builder.BuildStore(defaultVal, var.Value);
        }
        return defaultVal;
    }

    public override Statement Instantiate(InstantiationInfo info) => new StatementVariable(Vars[0].Type.Instantiate(info), Definition.Instantiate(info) as Expression, Vars.Select(x => x.Name).ToArray());

    public override string ToString() {
        string ret = Vars[0].Type.ToString() + " ";
        foreach (var var in Vars) {
            ret += var.Name + (var == Vars.Last() ? " " : ", ");
        }
        return ret + "= " + Definition.ToString();
    }

}