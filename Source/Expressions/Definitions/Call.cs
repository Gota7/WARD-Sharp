using LLVMSharp.Interop;
using WARD.Exceptions;
using WARD.Scoping;
using WARD.Statements;
using WARD.Types;

namespace WARD.Expressions;

// Expression for a function call. TODO: CASTING ARGUMENTS!!!
public class ExpressionCall : Expression {
    public Expression Callee { get; } // What to call.
    public Expression[] Args { get; } // Arguments to the function call.
    private VarTypeFunction FuncType; // Type of the function being called.

    // Create a new function call.
    public ExpressionCall(Expression callee, params Expression[] args) {
        //Callee = new ExpressionImplicitCast(callee, new ConceptCompliment(new ConceptType(VarTypeEnum.LValueReference))); // Can only call R-values.
        Callee = callee;
        Args = args;
    }

    public override void SetScopes(Scope parent) {
        Callee.SetScopes(parent);
        foreach (var a in Args) {
            a.SetScopes(parent);
        }
    }

    public override void ResolveVariables() {
        Callee.ResolveVariables();
        foreach (var a in Args) {
            a.ResolveVariables();
        }
    }

    public override void ResolveTypes(VarType preferredReturnType, List<VarType> parameterTypes) {
        foreach (var a in Args) {
            a.ResolveTypes();
        }
        Callee.ResolveTypes(preferredReturnType, Args.Select(x => x.GetReturnType()).ToList());
        VarType calleeType = Callee.GetReturnType();
        if (calleeType as VarTypeFunction == null) {
            Error.ThrowInternal("Call expression expects a function type, but instead got \"" + calleeType.ToString() + "\".");
        }
        FuncType = calleeType as VarTypeFunction;
    }

    protected override VarType ReturnType() => FuncType.ReturnType.GetVarType();

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder) {
        var callee = Callee.Compile(mod, builder); // This is important for function pointers where we must get the function value.
        LLVMValueRef[] args = new LLVMValueRef[Args.Length];
        for (int i = 0; i < args.Length; i++) {
            args[i] = Args[i].Compile(mod, builder);
        }
        if (callee == null) {

            // If we have a null callee, this means we are calling an inline function.
            // The only case that the callee ends up null is if we are doing a variable.
            //return ((Callee as ExpressionVariable).Resolved as Function).CompileInline(mod, builder, param, args);
            throw new System.NotImplementedException();

        }
        return builder.BuildCall2(FuncType.GetLLVMType(), callee, args);
    }

    public override string ToString() {
        string ret = "(" + ReturnType().ToString() + ")" + Callee.ToString() + "(";
        for (int i = 0; i < Args.Length; i++) {
            ret += Args[i].ToString();
            if (i != Args.Length - 1) {
                ret += ", ";
            }
        }
        return ret + ")";
    }

}