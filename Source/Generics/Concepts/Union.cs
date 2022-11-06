using WARD.Scoping;
using WARD.Types;

namespace WARD.Generics;

// Represents a concept that is a union of existing ones.
public class ConceptUnion : Concept {
    private Concept[] Concepts; // Concepts to modify.

    // Take the union of concepts.
    public ConceptUnion(params Concept[] concepts) {
        Concepts = concepts;
    }

    public override bool TypeFitsConcept(VarType type, Scope scope) {
        foreach (var c in Concepts) {
            if (c.TypeFitsConcept(type, scope)) return true;
        }
        return false;
    }

}