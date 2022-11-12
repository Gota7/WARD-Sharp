using LLVMSharp.Interop;
using WARD.Exceptions;
using WARD.Expressions;
using WARD.Generics;
using WARD.Scoping;
using WARD.Types;

namespace WARD.Statements;

// An if statement. TODO: IMPLICIT CAST TO BOOL!!!
public class StatementIf : Statement {
    public Scope Scope { get; private set; } // Statement scope.
    public Expression Condition { get; } // Condition for a branch to occur.
    public Statement ThenStatement { get; } // Code to execute if condition is true.
    public Statement ElseStatement { get; } // Else statement to execute if the condition is not met.

    // Create a new if statement.
    public StatementIf(Expression condition, Statement thenStatement, Statement elseStatement = null) {
        Condition = new ExpressionRValue(condition); // A condition must always be an R-Value.
        ThenStatement = thenStatement;
        ElseStatement = elseStatement;
    }

    public override void SetScopes(Scope parent) {
        Scope = parent;
        Condition.SetScopes(parent);
        ThenStatement.SetScopes(parent);
        if (ElseStatement != null) ElseStatement.SetScopes(parent);
    }

    public override void ResolveVariables() {
        Condition.ResolveVariables();
        ThenStatement.ResolveVariables();
        if (ElseStatement != null) ElseStatement.ResolveVariables();
    }

    public override void ResolveTypes() {
        Condition.ResolveTypes();
        if (!Condition.GetReturnType().Equals(VarType.Bool, Scope)) {
            Error.ThrowInternal("If statement return type must be a boolean expression.");
            return;
        }
        ThenStatement.ResolveTypes();
        if (ElseStatement != null) ElseStatement.ResolveTypes();
    }

    public override bool ReturnsType() {
        if (!ThenStatement.ReturnsType()) return false; // Both statements need to return, so quit now.
        if (ElseStatement != null && ElseStatement.ReturnsType()) return true; // Both statements do return.
        else return false;
    }

    public override bool EndsBlock() {
        if (!ThenStatement.EndsBlock()) return false; // Both statements need to end, so quit now.
        if (ElseStatement != null && ElseStatement.EndsBlock()) return true; // Both statements end the block.
        else return false;
    }

    public override void CompileDeclarations(LLVMModuleRef mod, LLVMBuilderRef builder) {
        Condition.CompileDeclarations(mod, builder);
        ThenStatement.CompileDeclarations(mod, builder);
        if (ElseStatement != null) ElseStatement.CompileDeclarations(mod, builder);
    }

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder, CompilationContext ctx) {

        // Add blocks.
        LLVMBasicBlockRef thenBlock = LLVMBasicBlockRef.AppendInContext(mod.Context, ctx.Func, "ifThen");
        LLVMBasicBlockRef elseBlock = null;
        if (ElseStatement != null) elseBlock = LLVMBasicBlockRef.AppendInContext(mod.Context, ctx.Func, "ifElse");
        LLVMBasicBlockRef contBlock = null;
        if (!EndsBlock()) contBlock = LLVMBasicBlockRef.AppendInContext(mod.Context, ctx.Func, "ifCont"); // Only define continuation if no returns.

        // Compile the condition.
        LLVMValueRef condition = Condition.Compile(mod, builder, ctx);
        builder.BuildCondBr(condition, thenBlock, elseBlock == null ? contBlock : elseBlock);

        // Compile the then statement.
        builder.PositionAtEnd(thenBlock);
        ThenStatement.Compile(mod, builder, ctx);
        if (!ThenStatement.EndsBlock()) builder.BuildBr(contBlock); // Will only occur if both then and else end the block.

        // Compile the else statement.
        if (ElseStatement != null) {
            builder.PositionAtEnd(elseBlock);
            ElseStatement.Compile(mod, builder, ctx);
            if (!ElseStatement.EndsBlock()) builder.BuildBr(contBlock); // Will only occur if both then and else end a block.
        }

        // Place at new continuation. Statement return value isn't really used so just null it.
        if (contBlock != null) builder.PositionAtEnd(contBlock);
        return null;

    }

    public override Statement Instantiate(InstantiationInfo info) => new StatementIf(Condition.Instantiate(info) as Expression, ThenStatement.Instantiate(info), ElseStatement == null ? null : ElseStatement.Instantiate(info));

    public override string ToString() => "if (" + Condition.ToString() + ")" + ThenStatement.ToString() + (ElseStatement != null ? ("else " + ElseStatement.ToString()) : "");

}