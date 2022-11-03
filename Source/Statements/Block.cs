using LLVMSharp.Interop;
using WARD.Expressions;
using WARD.Scoping;
using WARD.Types;

namespace WARD.Statements;

// A block of statements. A block has its own new scope and multiple statements.
public class StatementBlock : Statement {
    private static int BlockId = 0; // Block ID.
    public List<Statement> Statements; // Statements in the block.

    // Create a new return statement.
    public StatementBlock(params Statement[] statements) {
        Statements = statements.ToList();
    }
    
    public override void SetScopes(Scope parent) {
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

    public override bool ReturnsType(VarType type, out bool outExplicitVoidReturn) {
        
        // This one is more interesting.
        // If we come across a statement that returns false, we should return false since the return type doesn't match ours.
        // However, there is an exception to this. It's possible the statement is just an expression that doesn't do anything (no return).
        // Thus, we first need to check if the return is a void, and ignore it if it is unless it is explicit.
        // If we are not ignoring it, we then check if the type matches ours and return the result.
        // If we make it to the end, then we are implicitly returning void.

        // Go through each statement.
        foreach (var statement in Statements) {
            bool returnsVoid = statement.ReturnsType(VarType.Void, out outExplicitVoidReturn);
            if (returnsVoid && outExplicitVoidReturn || !returnsVoid) {
                return statement.ReturnsType(type, out outExplicitVoidReturn);
            }
        }

        // Made it through the end, error unless type is void.
        outExplicitVoidReturn = false;
        return type.Equals(VarType.Void);

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

}