using WARD.Expressions;
using WARD.Scoping;
using WARD.Types;

namespace WARD.Builders;

// For managing what can be casted to what. Heavily based off of: https://en.cppreference.com/w/cpp/language/implicit_conversion.
public class ConversionSequence {
    public Conversion[] System { get; } // Sequences that are "built-in" and not user defined.
    public List<Conversion> User { get; } = new List<Conversion>(); // Conversions that are added by the programmer.

    // Create a new conversion sequence.
    public ConversionSequence(params Conversion[] systemConversions) {
        System = systemConversions;
    }

    // Convert a source type into a desired type. This is done with one or more system conversions, then one or more user conversions, then one or more system conversions. Input expression must have types resolved!
    public Expression Convert(VarType src, VarType dest, Expression input, Scope scope, bool isImplicit) {

        // // Given a list of conversions, 
        // bool ExpandConversions(VarType src, Expression input, List<VarType> outTypes, List<Expression> outExpressions, IEnumerable<Conversion> conversions) {
        //     foreach (var conversion in conversions) {
        //         if (conversion.InputConcept.TypeFitsConcept(src, scope)) {
        //             outTypes.Add(conversion.ConvertType(src, scope));
        //             outExpressions.Add(conversion.GetExpression(input, scope));
        //             if (outTypes.Last().Equals(dest, scope)) return true;
        //         }
        //     }
        //     return false;
        // }

        // // Get lists of current types an expressions.
        // List<VarType> types = new List<VarType>();
        // List<Expression> expressions = new List<Expression>();
        // foreach (var conversion in System) {
        //     if (conversion.InputConcept.TypeFitsConcept(src, scope)) {
        //         types.Add(conversion.ConvertType(src, scope));
        //         expressions.Add(conversion.GetExpression(input, scope));
        //         if (types.Last().Equals(dest, scope)) return expressions.Last();
        //     }
        // }
        throw new System.NotImplementedException();

    }

}