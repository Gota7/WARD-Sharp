using WARD.Exceptions;
using WARD.Expressions;
using WARD.Generics;
using WARD.Scoping;
using WARD.Types;

namespace WARD.Builders;

// Define some useful conversions.
public abstract partial class Conversion {
    public static Conversion Int2Bool { get; } = new ConversionInt2Bool();
}

// For converting an integer to a boolean.
public class ConversionInt2Bool : Conversion {

    // Make a new int to bool expression.
    public ConversionInt2Bool() : base(Concept.Integer) {}

    // Get the R-value type of an L-value.
    protected override VarType Convert(VarType type, Scope scope) {
        return VarType.Bool;
    }

    public override Expression GetExpression(Expression input, Scope scope) {
        return new ExpressionLLVM("icmpne", VarType.Bool, input, new ExpressionConstInt(input.GetReturnType(), 0));
    }

}