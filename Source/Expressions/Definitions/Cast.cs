using LLVMSharp.Interop;
using WARD.Exceptions;
using WARD.Generics;
using WARD.Scoping;
using WARD.Statements;
using WARD.Types;

namespace WARD.Expressions;

// Expression that is a cast to another type of expression.
public class ExpressionCast : Expression {
    public Expression Operand { get; } // Operand of the expression.
    public VarType DestType { get; } // Desired destination type of the cast.
    public bool Implicit { get; } // If the cast does not require user interaction.

    // Create a cast expression.
    public ExpressionCast(Expression operand, VarType destType, bool isImplicit) {
        Operand = operand;
        DestType = destType;
        Implicit = isImplicit;
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
    }

    protected override VarType ReturnType() {
        return DestType.GetVarType(Scope);
    }

    public override bool Constant() => Operand.Constant();

    public override void CompileDeclarations(LLVMModuleRef mod, LLVMBuilderRef builder) {
        Operand.CompileDeclarations(mod, builder);
    }

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder, CompilationContext ctx) {
        if (Operand.GetReturnType().Equals(DestType, Scope)) {
            return Operand.Compile(mod, builder, ctx);
        } else {
            var expr = ctx.ConversionSequence.Convert(Operand.GetReturnType(), DestType.GetVarType(Scope), Operand, Scope, Implicit);
            expr.ResolveVariables();
            expr.ResolveTypes();
            return expr.Compile(mod, builder, ctx);
        }
    }

    public override string ToString() => "(" + DestType.ToString() + ")" + Operand.ToString();

    public override Statement Instantiate(InstantiationInfo info) => new ExpressionCast(Operand.Instantiate(info) as Expression, DestType.Instantiate(info), Implicit);

}