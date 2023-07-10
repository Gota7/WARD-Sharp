using WARD.Scoping;
using WARD.Types;

namespace WARD.Generics;

// Define some useful concepts.
public abstract partial class Concept {
    public static Concept ArithmeticInteger { get; } = new ConceptArithmeticInteger();
}

// Represents a concept for whole numbers that you can do math on. Ensures that math is done on a clean power of 2 bitsize and the bitwidth is at least an int.
public class ConceptArithmeticInteger : Concept {
    public override bool TypeFitsConcept(VarType type, Scope scope) {
        var t = type as VarTypeInteger;
        if (t != null) {
            int intBase = (int)Math.Log2(VarType.Int.BitWidth); // For 32-bit int, 2^5 = 32 so 5.
            double currIntBase = Math.Log2(t.BitWidth);
            double rounded = Math.Round(currIntBase);
            if (Math.Abs(rounded - currIntBase) < 0.0000001) { // Must be clean log.
                return (int)rounded >= currIntBase; // Ensure either 2^5, 2^6, 2^7, etc.
            }
        }
        return false;
    }
}