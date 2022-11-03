using LLVMSharp.Interop;
using WARD.Expressions;
using WARD.Scoping;
using WARD.Types;

namespace WARD.Statements;

// A return statement to return a value.
public class StatementReturn : Statement {
    public Expression ReturnExpression { get; } // Return value for the return.

    // Create a new return statement. Leave null for void.
    public StatementReturn(Expression returnExpression = null) {
        ReturnExpression = returnExpression;
    }
    
    public override void SetScopes(Scope parent) {
        if (ReturnExpression != null) ReturnExpression.SetScopes(parent);
    }

    public override void ResolveVariables() {
        if (ReturnExpression != null) ReturnExpression.ResolveVariables();
    }

    public override void ResolveTypes() {
        if (ReturnExpression != null) ReturnExpression.ResolveTypes();
    }

    public override bool ReturnsType(VarType type, out bool outExplicitVoidReturn) {
        outExplicitVoidReturn = true; // If we are returning anything, then it is explicit.
        if (ReturnExpression != null) {
            return type.Equals(ReturnExpression.GetReturnType());
        } else {
            return type.Equals(VarType.Void);
        }
    }

    public override void CompileDeclarations(LLVMModuleRef mod, LLVMBuilderRef builder) {
        if (ReturnExpression != null) ReturnExpression.CompileDeclarations(mod, builder);
    }

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder) {
        if (ReturnExpression != null) {
            return builder.BuildRet(ReturnExpression.Compile(mod, builder));
        } else {
            return builder.BuildRetVoid();
        }
    }

}