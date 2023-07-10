using System.Runtime.InteropServices;
using WARD.Builders;
using WARD.Common;
using WARD.Expressions;
using WARD.Statements;
using WARD.Types;
using Xunit;

// Test how operators work.
public class OperatorTest {

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate int BinaryInt32Operation(int op1, int op2);

    // Add test using exact operands the operator expects.
    [Fact]
    public void AddExact() {
        ProgramBuilder pb = new ProgramBuilder();
        pb.AddStandardOperators();
        UnitBuilder ub = new UnitBuilder("TestMod");
        var test = ub.AddFunction("test", new VarTypeFunction(VarType.Int, null, new Variable("a", VarType.Int), new Variable("b", VarType.Int)), "", new ItemAttribute("NoMangle"));
        test.Define(new StatementReturn(new ExpressionOperator(
            "Add",
            new ExpressionRValue(new ExpressionVariable("a")),
            new ExpressionRValue(new ExpressionVariable("b"))
        )));
        pb.AddUnitBuilder(ub);
        pb.Compile();
        BinaryInt32Operation op = pb.GetFunctionExecuter<BinaryInt32Operation>("TestMod", test);
        Assert.Equal(7, op(3, 4));
    }

}