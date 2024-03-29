using LLVMSharp.Interop;
using WARD.Generics;
using WARD.Scoping;
using WARD.Statements;
using WARD.Types;

namespace WARD.Expressions;

// Expression that is guaranteed to be an R-Value. While a cast will work, they are a lot slower and so this should be used when possible.
public class ExpressionRValue : Expression {
    public Expression Operand { get; } // Operand of the expression.
    private bool LValue; // If the operand is an L-Value.

    // Create a new R-Value expression.
    public ExpressionRValue(Expression operand) {
        Operand = operand;
    }

    public override void SetScopes(Scope parent) {
        Scope = parent;
        Operand.SetScopes(parent);
    }

    public override void ResolveVariables() {
        Operand.ResolveVariables();
    }

    public override void ResolveTypes(VarType preferredReturnType, List<VarType> parameterTypes) {
        Operand.ResolveTypes();
        LValue = Operand.GetReturnType() as VarTypeLValueReference != null;
    }

    protected override VarType ReturnType() {
        VarTypeLValueReference opType = Operand.GetReturnType() as VarTypeLValueReference;
        if (opType == null) return Operand.GetReturnType();
        else return opType.Referenced.GetVarType(Scope);
    }

    public override bool Constant() => Operand.Constant();

    public override void CompileDeclarations(LLVMModuleRef mod, LLVMBuilderRef builder) {
        Operand.CompileDeclarations(mod, builder);
    }

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder, CompilationContext ctx) {
        LLVMValueRef val = Operand.Compile(mod, builder, ctx);
        if (LValue) return builder.BuildLoad2((Operand.GetReturnType() as VarTypeLValueReference).Referenced.GetLLVMType(Scope), val);
        else return val;
    }

    public override string ToString() => "RValue(" + Operand.ToString() + ")";

    public override Statement Instantiate(InstantiationInfo info) => new ExpressionRValue(Operand.Instantiate(info) as Expression);

}