using LLVMSharp.Interop;
using WARD.Exceptions;
using WARD.Generics;
using WARD.Statements;
using WARD.Types;

namespace WARD.Expressions;

// Expression that is guaranteed to be an R-Value.
public class ExpressionRValue : Expression {
    public Expression Operand { get; } // Operand of the expression.
    private bool LValue; // If the operand is an L-Value.

    // Create a new R-Value expression.
    public ExpressionRValue(Expression operand) {
        Operand = operand;
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

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder) {
        LLVMValueRef val = Operand.Compile(mod, builder);
        if (LValue) return builder.BuildLoad2(Operand.GetReturnType().GetLLVMType(Scope), val);
        else return val;
    }

    public override string ToString() => "RValue(" + Operand.ToString() + ")";

    public override Statement Instantiate(InstantiationInfo info) => new ExpressionRValue(Operand.Instantiate(info) as Expression);

}