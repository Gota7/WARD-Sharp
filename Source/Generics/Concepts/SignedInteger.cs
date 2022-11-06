using WARD.Scoping;
using WARD.Types;

namespace WARD.Generics;

// Define some useful concepts.
public abstract partial class Concept {
    public static Concept SignedInteger { get; } = new ConceptSignedInteger();
}

// Represents a concept for signed integers.
public class ConceptSignedInteger : Concept {
    public override bool TypeFitsConcept(VarType type, Scope scope) {
        var t = type as VarTypeInteger;
        if (t != null) {
            return t.Signed;
        }
        return false;
    }
}