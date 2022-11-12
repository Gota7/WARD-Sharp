using LLVMSharp.Interop;
using WARD.Generics;
using WARD.Statements;
using WARD.Types;

namespace WARD.Expressions;

// Expression for a constant pointer.
public class ExpressionConstPointer : Expression {
    public VarTypePointer PointerType { get; } // Type of the pointer to be constant.
    public ulong? Value { get; } // Value for the constant pointer to be.

    // Create a new constant pointer expression. Can have a null value.
    public ExpressionConstPointer(VarTypePointer pointerType, ulong? value = null) {
        PointerType = pointerType;
        Value = value;
    }

    protected override VarType ReturnType() => PointerType;

    public override bool Constant() => true;

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder, CompilationContext ctx) {
        if (Value != null) {
            return builder.BuildIntToPtr(LLVMValueRef.CreateConstInt(LLVMTypeRef.Int64, Value.Value, false), PointerType.GetLLVMType(Scope));
        } else {
            return LLVMValueRef.CreateConstPointerNull(PointerType.GetLLVMType(Scope));
        }
    }

    public override string ToString() => Value == null ? "NULL" : ("0x" + Value.Value.ToString("X"));

    public override Statement Instantiate(InstantiationInfo info) => new ExpressionConstPointer(PointerType.Instantiate(info) as VarTypePointer, Value);

}