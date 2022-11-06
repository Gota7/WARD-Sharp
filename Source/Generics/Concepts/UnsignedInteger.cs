using WARD.Scoping;
using WARD.Types;

namespace WARD.Generics;

// Define some useful concepts.
public abstract partial class Concept {
    public static Concept UnsignedInteger { get; } = new ConceptUnsignedInteger();
}

// Represents a concept for unsigned integers.
public class ConceptUnsignedInteger : Concept {
    public override bool TypeFitsConcept(VarType type, Scope scope) {
        var t = type as VarTypeInteger;
        if (t != null) {
            return !t.Signed;
        }
        return false;
    }
}