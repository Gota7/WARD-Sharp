using LLVMSharp.Interop;
using WARD.Exceptions;
using WARD.Generics;
using WARD.Scoping;
using WARD.Statements;
using WARD.Types;

namespace WARD.Expressions;

// Assign an L-Value a value. TODO: CAST AS NEEDED!!!
public class ExpressionAssign : Expression {
    public Expression Left { get; } // Expression being assigned a value on the left.
    public Expression Right { get; } // Value to assign on the right.

    // Create a new assignment.
    public ExpressionAssign(Expression left, Expression right) {
        Left = left;
        Right = right;
    }

    public override void SetScopes(Scope parent) {
        Scope = parent;
        Left.SetScopes(Scope);
        Right.SetScopes(Scope);
    }

    public override void ResolveVariables() {
        Left.ResolveVariables();
        Right.ResolveVariables();
    }

    public override void ResolveTypes(VarType preferredReturnType, List<VarType> parameterTypes) {
        Left.ResolveTypes();
        Right.ResolveTypes();
        var lValueRef = Left.GetReturnType() as VarTypeLValueReference;
        if (lValueRef == null) {
            Error.ThrowInternal("Can not assign to a non L-Value.");
            return;
        }
        if (!Right.GetReturnType().Equals(lValueRef.Referenced, Scope)) {
            Error.ThrowInternal("Can not assign type \"" + Right.GetReturnType().ToString() + "\" somewhere expecting type \"" + lValueRef.Referenced.ToString() + "\".");
        }
    }

    protected override VarType ReturnType() => Left.GetReturnType(); // x = y will always return x.

    public override bool Constant() => false;

    public override void CompileDeclarations(LLVMModuleRef mod, LLVMBuilderRef builder) {
       Left.CompileDeclarations(mod, builder);
       Right.CompileDeclarations(mod, builder);
    }

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder, CompilationContext ctx) {
        var left = Left.Compile(mod, builder, ctx);
        var right = Right.Compile(mod, builder, ctx);
        builder.BuildStore(right, left);
        return left;
    }

    public override string ToString() => Left.ToString() + " = " + Right.ToString();

    public override Statement Instantiate(InstantiationInfo info) => new ExpressionAssign(Left.Instantiate(info) as Expression, Right.Instantiate(info) as Expression);

}