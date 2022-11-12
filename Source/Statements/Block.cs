using LLVMSharp.Interop;
using WARD.Expressions;
using WARD.Generics;
using WARD.Scoping;
using WARD.Types;

namespace WARD.Statements;

// A block of statements. A block has its own new scope and multiple statements.
public class StatementBlock : Statement {
    private Scope Scope; // Scope.
    private static int BlockId = 0; // Block ID.
    public List<Statement> Statements; // Statements in the block.
    private int Id { get; } // Id of this block.

    // Create a new return statement.
    public StatementBlock(params Statement[] statements) {
        Statements = statements.ToList();
        Id = BlockId++;
    }

    public override void SetScopes(Scope parent) {
        Scope = parent;
        foreach (var statement in Statements) {
            statement.SetScopes(parent.EnterScope("%CODESTATEMENT%_" + Id));
        }
    }

    public override void ResolveVariables() {
        foreach (var statement in Statements) {
            statement.ResolveVariables();
        }
    }

    public override void ResolveTypes() {
        foreach (var statement in Statements) {
            statement.ResolveTypes();
        }
    }

    public override bool ReturnsType() {
        foreach (var statement in Statements) {
            if (statement.EndsBlock()) return statement.ReturnsType(); // Block is ending, it either returns or not.
        }
        return false;
    }

    public override bool EndsBlock()
    {
        foreach (var statement in Statements) {
            if (statement.EndsBlock()) return true; // Exit early if found exit.
        }
        return false;
    }

    public override void CompileDeclarations(LLVMModuleRef mod, LLVMBuilderRef builder) {
        foreach (var statement in Statements) {
            if (statement.EndsBlock()) { statement.CompileDeclarations(mod, builder); return; } // Exit early.
            else statement.CompileDeclarations(mod, builder);
        }
    }

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder, CompilationContext ctx) {
        foreach (var statement in Statements) {
            if (statement.EndsBlock()) return statement.Compile(mod, builder, ctx); // Return value if needed.
            else statement.Compile(mod, builder, ctx);
        }
        return null;
    }

    public override Statement Instantiate(InstantiationInfo info) {
        Statement[] statements = new Statement[Statements.Count];
        for (int i = 0; i < statements.Length; i++) {
            statements[i] = Statements[i].Instantiate(info);
        }
        return new StatementBlock(statements);
    }

    public override string ToString() {
        string ret = "{\n";
        foreach (var statement in Statements) {
            ret += statement.ToString() + "\n";
        }
        return ret + "}";
    }

}