using WARD.Generics;
using WARD.Types;
using WARD.Scoping;
using WARD.Exceptions;
using WARD.Expressions;

namespace WARD.Builders;

// A single conversion that takes an input of a particular concept, an input type, and outputs a new type.
public abstract partial class Conversion {
    public Concept InputConcept { get; } // Types that are allowed to be converted.

    // Create a new conversion.
    public Conversion(Concept inputConcept) {
        InputConcept = inputConcept;
    }

    // Given a type that the conversion covers, get its output type.
    protected abstract VarType Convert(VarType type, Scope scope);

    // Given an input type to convert, get its output type.
    public VarType ConvertType(VarType type, Scope scope) {
        if (InputConcept.TypeFitsConcept(type, scope)) return Convert(type, scope);
        else {
            Error.ThrowInternal("Type " + type + " does not fit concept " + InputConcept + " as expected.");
            return null;
        }
    }

    // Create an expression used to convert the type to output type. Note that this can only be done after types have been resolved for the input.
    public abstract Expression GetExpression(Expression input, Scope scope);

}