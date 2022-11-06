using WARD.Builders;
using WARD.Common;
using WARD.Expressions;
using WARD.Statements;
using WARD.Types;
using Xunit;

// Test a basic hello world program.
public class HelloWorld {

    [Fact]
    public void Run() {

        // Create necessary builders.
        ProgramBuilder pb = new ProgramBuilder();
        UnitBuilder ub = new UnitBuilder("HelloWorld");

        // Add print function.
        ub.AddFunction("puts", new VarTypeFunction(
            VarType.Int,
            null,
            new Variable("str", VarType.String)
        ), "", new ItemAttribute("NoMangle"));

        // Add main function and define it.
        var main = ub.AddFunction("main", new VarTypeFunction(VarType.Int));
        var body = new StatementBlock();
        body.Statements.Add(new ExpressionCall(new ExpressionVariable("puts"), new ExpressionConstString("Hello World!")));
        body.Statements.Add(new StatementReturn(new ExpressionConstInt(VarType.Int, 0)));
        main.Define(body);

        // Compile program.
        pb.AddUnitBuilder(ub);
        pb.Compile();

        // Exporting of different types and execution example.
        pb.ExportLLVMAssembly("HelloWorld", "HelloWorld.ll");
        pb.ExportLLVMBitcode("HelloWorld", "HelloWorld.bc");
        pb.ExportAssembly("HelloWorld", "HelloWorld.s");
        pb.ExportObject("HelloWorld", "HelloWorld.o");
        Assert.Equal(0, pb.Execute());

    }

}