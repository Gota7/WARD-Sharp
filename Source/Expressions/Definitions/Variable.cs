using LLVMSharp.Interop;
using WARD.Common;
using WARD.Exceptions;
using WARD.Scoping;
using WARD.Statements;
using WARD.Types;

namespace WARD.Expressions;

// Expression for a variable.
public class ExpressionVariable : Expression {
    public string Path { get; } // Path to the variable.
    private Variable PossibleVariable; // Possible variable that may be the resolved one.
    private List<Function> PossibleFunctions; // Possible functions that may be the resolved one.
    public Variable Resolved { get; private set; } // Resolved variable.
    private bool LValue; // If we are returning an L-value or an R-value.

    // Create a variable reference.
    public ExpressionVariable(string path) {
        Path = path;
    }

    public override void ResolveVariables() {
        PossibleVariable = Scope.Table.ResolveVariable(Path);
        PossibleFunctions = Scope.Table.ResolveFunctions(Path);
    }

    public override void ResolveTypes(VarType preferredReturnType, List<VarType> parameterTypes) {

        // Parameters are not null, this means we are resolving a function.
        // What if we are resolving a function pointer?
        // This won't happen, as we must *dereference* the function pointer first.
        // The dereference operator is calling this function, thus it won't give us populated type info for resolution.
        if (parameterTypes != null) {
            LValue = false; // Functions don't get loaded as they are really R-values.

            // An exact match has been found, function is probably unmangled.
            if (PossibleVariable != null) {
                Resolved = PossibleVariable;
                return;
            }

            // Time for some function overload resolution.
            List<Tuple<Function, int>> possibleFunctions = new List<Tuple<Function, int>>();
            foreach (var p in PossibleFunctions) {
                int distance;
                if (p.CallSatisfiesOverload(parameterTypes.ToArray(), out distance)) {
                    possibleFunctions.Add(new Tuple<Function, int>(p, distance));
                }
            }

            // No results.
            if (possibleFunctions.Count == 0) {
                Error.ThrowInternal("No valid function overload found for path \"" + Path + "\".");
                return;
            }

            // Select function with least distance.
            var sorted = possibleFunctions.OrderBy(x => x.Item2);
            if (sorted.Where(x => x.Item2 == sorted.First().Item2).Count() > 1) {
                Error.ThrowInternal("Function overload call for path \"" + Path + "\" is ambiguous.");
                return;
            }
            Resolved = sorted.First().Item1;

        }

        // Otherwise, we are simply just fetching a variable.
        else {
            LValue = true; // Variables are L-values.
            if (PossibleVariable != null) {
                Resolved = PossibleVariable;
            } else {
                Error.ThrowInternal("Can not resolve variable with path \"" + Path + "\".");
            }
        }

    }

    protected override VarType ReturnType() => LValue ? new VarTypeLValueReference(Resolved.Type.GetVarType(), DataAccessFlags.Read) : Resolved.Type.GetVarType();

    public override LLVMValueRef Compile(LLVMModuleRef mod, LLVMBuilderRef builder) {
        return Resolved.Value;
    }

    public override string ToString() => Path;

}