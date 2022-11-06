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
        //test.Define(new StatementReturn(new ExpressionConstPointer(new VarTypePointer(VarType.Int)))); // Should fail.
        pb.AddUnitBuilder(ub);
        pb.Compile();
        var func = pb.GetFunctionExecuter<ReturnInt>("TestMod", test);
        Assert.Equal(7, func());
    }

}