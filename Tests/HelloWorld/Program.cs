using WARD.Builders;
using WARD.Common;
using WARD.Expressions;
using WARD.Types;

// Create necessary builders.
ProgramBuilder pb = new ProgramBuilder();
UnitBuilder ub = new UnitBuilder("HelloWorld");

// Add print function.
ub.AddFunction("puts", new VarTypeFunction(
    VarType.Int,
    null,
    new Variable("str", VarType.String)
), new ItemAttribute("NoMangle"));

// Add main function and define it.
var main = ub.AddFunction("main", new VarTypeFunction(VarType.Void));
main.Define(new ExpressionCall(new ExpressionVariable("puts"), new ExpressionConstString("Hello World!")));

// Compile program.
pb.AddCompilationUnit(ub);
pb.Compile();

// Exporting of different types and execution example.
pb.ExportLLVMAssembly("HelloWorld", "HelloWorld.ll");
pb.ExportLLVMBitcode("HelloWorld", "HelloWorld.bc");
pb.ExportAssembly("HelloWorld", "HelloWorld.s");
pb.ExportObject("HelloWorld", "HelloWorld.o");
pb.Execute();