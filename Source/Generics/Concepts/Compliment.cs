using WARD.Scoping;
using WARD.Types;

namespace WARD.Generics;

// Define some useful concepts.
public abstract partial class Concept {
    public static Concept RValue { get; } = new ConceptCompliment(LValue);
}

// Represents a concept that is the compliment of an existing one.
public class ConceptCompliment : Concept {
    private Concept Concept; // Concept to modify.

    // Take the opposite of a concept.
    public ConceptCompliment(Concept concept) {
        Concept = concept;
    }

    public override bool TypeFitsConcept(VarType type, Scope scope) {
        return !Concept.TypeFitsConcept(type, scope);
    }

}