using WARD.Exceptions;
using WARD.Expressions;
using WARD.Generics;
using WARD.Scoping;
using WARD.Types;

namespace WARD.Builders;

// Define some useful conversions.
public abstract partial class Conversion {
    public static Conversion IntegerPromotion { get; } = new ConversionIntegerPromotion();
}

// For promoting integers.
public class ConversionIntegerPromotion : Conversion {

    // Make a integer promotion.
    public ConversionIntegerPromotion() : base(Concept.Integer) {}

    // Get the promotion.
    protected override VarType Convert(VarType type, Scope scope) {
        var oldType = type as VarTypeInteger;
        double power2 = Math.Log2(oldType.BitWidth);
        int newPower2;
        if (power2 - (int)power2 < 0.0000001) newPower2 = (int)power2 + 1; // If clean log, make next power one up.
        else newPower2 = (int)Math.Ceiling(power2); // Otherwise just round up.
        newPower2 = Math.Max((int)Math.Log2(VarTypeInteger.Int.BitWidth), newPower2); // Ensure >= int bitwidth.
        var ret = new VarTypeInteger(oldType.Signed, (uint)Math.Pow(2, newPower2), oldType.AccessFlags);
        if (!Concept.ArithmeticInteger.TypeFitsConcept(ret, scope)) {
            Error.ThrowInternal("Integer promotion did not result in a valid arithmetic integer.");
            return null;
        }
        else return ret;
    }

    public override Expression GetExpression(Expression input, Scope scope) {
        VarTypeInteger dest = Convert(input.GetReturnType(), scope) as VarTypeInteger;
        if (dest.Signed) {
            return new ExpressionLLVM("sext", dest, input);
        } else {
            return new ExpressionLLVM("zext", dest, input);
        }
    }

}