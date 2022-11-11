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

    // Create a new return statement.
    public StatementBlock(params Statement[] statements) {
        Statements = statements.ToList();
    }

    public override void SetScopes(Scope parent) {
        Scope = parent;
        foreach (var statement in Statements) {
            statement.SetScopes(parent.EnterScope("%CODESTATEMENT%_" + BlockId++));
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
            if (statement.ReturnsType()) return true;
        }
        return false;
    }

    public override void CompileDeclarations(LLVMModuleRef mod, LLVMBuilderRef builder) {
        foreach (var statement in Statements) {
            statement.CompileDeclarations(mod, builder);
            if (statement as StatementReturn != null) return; // Return if needed.
        }
    }

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder) {
        foreach (var statement in Statements) {
            if (statement as StatementReturn != null) return statement.Compile(mod, builder); // Return value if needed.
            else statement.Compile(mod, builder);
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

}