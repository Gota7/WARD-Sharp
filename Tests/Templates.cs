using System.Runtime.InteropServices;
using WARD.Builders;
using WARD.Common;
using WARD.Expressions;
using WARD.Generics;
using WARD.Statements;
using WARD.Types;
using Xunit;

// For testing templates.
public class TemplateTests {

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate int IntReturn();

    // Test creating and executing a simple generic.
    [Fact]
    public void SimpleValueTest() {
        ProgramBuilder pb = new ProgramBuilder();
        UnitManager um = new UnitManager();
        um.AddUnit("Templates");
        TemplateBuilder tb = new TemplateBuilder(um, "Templates", new TemplateParameter("T", TemplateParameterType.Value, Concept.ArithmeticInteger));

        // Define the addition function to test.
        tb.AddFunction(
            "returnConst",
            new VarTypeFunction(VarType.Int),
            new StatementReturn(new ExpressionGeneric("T")),
            "",
            new ItemAttribute("NoMangle")
        );
        tb.Instantiate(new ExpressionConstInt(VarType.Int, 31));
        pb.AddUnitManager(um);

        // Compile, export modules, and test.
        pb.Compile();
        pb.ExportLLVMAssembly("Templates", "templatesValue.ll");
        IntReturn func = pb.GetFunctionExecuterFromUnmangledName<IntReturn>("Templates", "returnConst");
        Assert.Equal(31, func());

    }

    // Test creating and executing a simple generic.
    [Fact]
    public void SimpleTypenameTest() {
        ProgramBuilder pb = new ProgramBuilder();
        UnitManager um = new UnitManager();
        um.AddUnit("Templates");
        TemplateBuilder tb = new TemplateBuilder(um, "Templates", new TemplateParameter("T", TemplateParameterType.Typename, Concept.ArithmeticInteger));

        // Define the addition function to test.
        tb.AddFunction(
            "add",
            new VarTypeFunction(new VarTypeGeneric("T")),
            new StatementReturn(new ExpressionLLVM("add", new VarTypeGeneric("T"),
                new ExpressionConstInt(new VarTypeGeneric("T"), 5),
                new ExpressionConstInt(new VarTypeGeneric("T"), 3)
            )),
            "",
            new ItemAttribute("NoMangle")
        );
        tb.Instantiate(VarType.Int);
        pb.AddUnitManager(um);

        // Compile, export modules, and test.
        pb.Compile();
        pb.ExportLLVMAssembly("Templates", "templatesType.ll");
        IntReturn func = pb.GetFunctionExecuterFromUnmangledName<IntReturn>("Templates", "add");
        Assert.Equal(8, func());

    }

}