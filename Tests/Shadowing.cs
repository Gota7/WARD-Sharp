using System.Runtime.InteropServices;
using WARD.Builders;
using WARD.Common;
using WARD.Expressions;
using WARD.Statements;
using WARD.Types;
using Xunit;

// For testing variable shadowing.
public class ShadowingTests
{

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate int FuncType(int param);

    // Test an operation with two ints.
    public int TestBase(Statement definition, int input) {
        ProgramBuilder pb = new ProgramBuilder();
        UnitBuilder ub = new UnitBuilder("TestMod");
        var test = ub.AddFunction("test", new VarTypeFunction(VarType.Int, null, new Variable("arg", VarType.Int)), "", new ItemAttribute("NoMangle"));
        test.Define(definition);
        pb.AddUnitBuilder(ub);
        pb.Compile();
        var func = pb.GetFunctionExecuter<FuncType>("TestMod", test);
        return func(input);
    }

    // Test if basic arguments work.
    // [Fact]
    // public void ArgumentTest() {
    //     Assert.Equal(404, TestBase(new StatementBlock(
    //         new StatementReturn(new ExpressionVariable("arg"))
    //     ), 404));
    // }

}