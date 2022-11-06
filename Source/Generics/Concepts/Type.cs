using WARD.Scoping;
using WARD.Types;

namespace WARD.Generics;

// Define some useful concepts.
public abstract partial class Concept {
    public static Concept LValue { get; } = new ConceptType<VarTypeLValueReference>();
    public static Concept Integer { get; } = new ConceptType<VarTypeInteger>();
}

// Represents a concept for a particular type.
public class ConceptType<T> : Concept where T : VarType {

    public override bool TypeFitsConcept(VarType type, Scope scope) {
        return type as T != null;
    }

}