using WARD.Exceptions;
using WARD.Expressions;
using WARD.Generics;
using WARD.Scoping;
using WARD.Types;

namespace WARD.Builders;

// Define some useful conversions.
public abstract partial class Conversion {
    public static Conversion LValue2RValue { get; } = new ConversionLValue2RValue();
}

// For converting an L-value to an R-value automatically.
public class ConversionLValue2RValue : Conversion {

    // Make a new L-value to R-value conversion.
    public ConversionLValue2RValue() : base(Concept.LValue) {}

    // Get the R-value type of an L-value.
    protected override VarType Convert(VarType type, Scope scope) {
        var ret = (type as VarTypeLValueReference).Referenced;
        if (!Concept.RValue.TypeFitsConcept(ret, scope)) {
            Error.ThrowInternal("L-value to R-value conversion did not work.");
            return null;
        }
        else return ret;
    }

    public override Expression GetExpression(Expression input, Scope scope) {
        return new ExpressionRValue(input);
    }

}