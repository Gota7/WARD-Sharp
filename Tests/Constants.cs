using System.Runtime.InteropServices;
using WARD.Builders;
using WARD.Common;
using WARD.Expressions;
using WARD.Statements;
using WARD.Types;
using Xunit;

// For testing constants.
public class ConstantTests
{

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate bool BoolReturn();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate int IntReturn();

    // Test an operation with two ints.
    public TDelegate BaseTest<TDelegate>(VarType retType, Statement definition) {
        ProgramBuilder pb = new ProgramBuilder();
        UnitBuilder ub = new UnitBuilder("TestMod");
        var test = ub.AddFunction("test", new VarTypeFunction(retType), "", new ItemAttribute("NoMangle"));
        test.Define(definition);
        pb.AddUnitBuilder(ub);
        pb.Compile();
        return pb.GetFunctionExecuter<TDelegate>("TestMod", test);
    }

    [Fact]
    public void ConstBool() {
        var func = BaseTest<BoolReturn>(VarType.Bool, new StatementReturn(new ExpressionConstBool(true)));
        Assert.True(func());
    }

    [Fact]
    public void ConstInt() {
        var func = BaseTest<IntReturn>(VarType.Int, new StatementReturn(new ExpressionConstInt(VarType.Int, (ulong)777)));
        Assert.Equal(777, func());
    }

    [Fact]
    public void ConstPointer() {
        VarTypePointer ptrType = new VarTypePointer(VarType.Int);
        var func = BaseTest<IntReturn>(ptrType, new StatementReturn(new ExpressionConstPointer(ptrType, 0xB800)));
        Assert.Equal(0xB800, func());
    }

    [Fact]
    public void ConstString() {
        var func = BaseTest<IntReturn>(VarType.String, new StatementReturn(new ExpressionConstString("Hi there :}\n")));
        Assert.NotEqual(0, func()); // No way to test this?
    }

}