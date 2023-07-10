using WARD.Common;
using WARD.Expressions;
using WARD.Statements;
using WARD.Types;

namespace WARD.Builders;

// Addition operator.
public static class OperatorAdd {

    public static ProgramBuilder.Operator GetInt() {
        return new ProgramBuilder.Operator(
            "Add",
            new VarTypeFunction(VarType.Int, null, new Variable("a", VarType.Int), new Variable("b", VarType.Int)),
            new StatementReturn(
                new ExpressionLLVM(
                    "add",
                    VarType.Int,
                    new ExpressionVariable("a"),
                    new ExpressionVariable("b")
                )
            )
        );
    }

}