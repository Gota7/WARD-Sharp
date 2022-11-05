using LLVMSharp.Interop;
using WARD.Statements;
using WARD.Types;

namespace WARD.Expressions;

// Expression for a constant integer.
public class ExpressionConstInt : Expression {
    public VarTypeInteger IntType { get; } // Type of integer that is constant.
    public ulong Value { get; } // Value of the constant integer.

    // Create a new constant int expression.
    public ExpressionConstInt(VarTypeInteger intType, ulong value) {
        IntType = intType;
        Value = value;
    }

    protected override VarType ReturnType() => IntType;

    public override bool Constant() => true;

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder) {
        return LLVMValueRef.CreateConstInt(IntType.GetLLVMType(Scope), Value, IntType.Signed);
    }

    public override string ToString() => "((" + IntType.ToString() + ")" + (IntType.Signed ? ((long)Value).ToString() : Value.ToString()) + ")";

}