using System.Runtime.InteropServices;
using WARD.Builders;
using WARD.Common;
using WARD.Expressions;
using WARD.Statements;
using WARD.Types;
using Xunit;

// For testing the unit manager.
public class UnitManagerTests {

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate int IntReturn();

    // Test calling a function from another compilation unit.
    [Fact]
    public void SpreadFunctionTests() {
        ProgramBuilder pb = new ProgramBuilder();
        UnitManager um = new UnitManager();
        um.AddUnit("Const5");
        um.AddUnit("Const7");
        um.AddUnit("AddConsts");

        // Define the addition function to test.
        var add = um.AddFunction("AddConsts", "add", new VarTypeFunction(VarType.Int), new ItemAttribute("NoMangle"));
        add.Define(new StatementReturn(new ExpressionLLVM("add", VarType.Int,
            new ExpressionCall(new ExpressionVariable("return5")),
            new ExpressionCall(new ExpressionVariable("return7"))
        )));

        // Define constant functions.
        var return5 = um.AddFunction("Const5", "return5", new VarTypeFunction(VarType.Int), new ItemAttribute("NoMangle"));
        return5.Define(new StatementReturn(new ExpressionConstInt(VarType.Int, 5)));
        var return7 = um.AddFunction("Const7", "return7", new VarTypeFunction(VarType.Int), new ItemAttribute("NoMangle"));
        return7.Define(new StatementReturn(new ExpressionConstInt(VarType.Int, 7)));
        pb.AddUnitManager(um);

        // Compile, export modules, and test.
        pb.Compile();
        pb.ExportLLVMAssembly("Const5", "const5.ll");
        pb.ExportLLVMAssembly("Const7", "const7.ll");
        pb.ExportLLVMAssembly("AddConsts", "addConsts.ll");
        IntReturn func = pb.GetFunctionExecuter<IntReturn>("AddConsts", add);
        Assert.Equal(12, func());

    }
    
}