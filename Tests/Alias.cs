using System.Runtime.InteropServices;
using WARD.Builders;
using WARD.Common;
using WARD.Expressions;
using WARD.Statements;
using WARD.Types;
using Xunit;

// For testing type aliasing.
public class AliasTests
{

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate int ReturnInt();

    // Test type aliasing.
    [Fact]
    public void AliasTest() {
        ProgramBuilder pb = new ProgramBuilder();
        UnitBuilder ub = new UnitBuilder("TestMod");
        ub.AddAlias("Number", VarType.Int);
        var test = ub.AddFunction("test", new VarTypeFunction(new VarTypeAlias("Number")), "", new ItemAttribute("NoMangle"));
        test.Define(new StatementReturn(new ExpressionConstInt(new VarTypeAlias("Number"), 7)));
        pb.AddUnitBuilder(ub);
        pb.Compile();
        var func = pb.GetFunctionExecuter<ReturnInt>("TestMod", test);
        Assert.Equal(7, func());
    }

    // Test type aliasing to a wrong type.
    private void AliasTestCrashBody() {
        ProgramBuilder pb = new ProgramBuilder();
        UnitBuilder ub = new UnitBuilder("TestMod");
        ub.AddAlias("Number", VarType.Int);
        var test = ub.AddFunction("test", new VarTypeFunction(new VarTypeAlias("Number")), "", new ItemAttribute("NoMangle"));
        test.Define(new StatementReturn(new ExpressionConstPointer(new VarTypePointer(VarType.Int)))); // Should fail.
        pb.AddUnitBuilder(ub);
        pb.Compile();
        var func = pb.GetFunctionExecuter<ReturnInt>("TestMod", test);
        Assert.Equal(0, func());
    }

    // Test type aliasing to a wrong type. TODO: IMPLICIT CASTING HAS TO BE ADDED BEFORE THIS FAILS!!!
    // [Fact]
    // public void AliasTestCrash() {
    //     Assert.ThrowsAny<Exception>(AliasTestCrashBody);
    // }

}