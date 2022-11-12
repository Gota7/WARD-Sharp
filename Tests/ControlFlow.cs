using System.Runtime.InteropServices;
using WARD.Builders;
using WARD.Common;
using WARD.Expressions;
using WARD.Statements;
using WARD.Types;
using Xunit;

// For testing control flow operations.
public class ControlFlowTests {

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate int ReturnInt();

    // Test base.
    public int TestBase(Statement body) {
        ProgramBuilder pb = new ProgramBuilder();
        UnitBuilder ub = new UnitBuilder("TestMod");
        var test = ub.AddFunction("test", new VarTypeFunction(VarType.Int), "", new ItemAttribute("NoMangle"));
        test.Define(body);
        pb.AddUnitBuilder(ub);
        pb.Compile();
        var func = pb.GetFunctionExecuter<ReturnInt>("TestMod", test);
        return func();
    }

    // Test basic if statement.
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void IfTest(bool passes) {
        int val = TestBase(new StatementBlock(
            new StatementIf(new ExpressionConstBool(passes), new StatementReturn(new ExpressionConstInt(VarType.Int, 5))),
            new StatementReturn(new ExpressionConstInt(VarType.Int, 7))
        ));
        Assert.Equal(passes ? 5 : 7, val);
    }

    // Test an if/else statement.
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void IfElseTest(bool passes) {
        int val = TestBase(
            new StatementIf(new ExpressionConstBool(passes),
                new StatementReturn(new ExpressionConstInt(VarType.Int, 75)),
                new StatementReturn(new ExpressionConstInt(VarType.Int, 98))
            )
        );
        Assert.Equal(passes ? 75 : 98, val);
    }

    // Test a loop.
    [Fact]
    public void LoopTestSingle() {
        int val = TestBase(
            new StatementBlock(
                new StatementLoop(new StatementBreak()),
                new StatementReturn(new ExpressionConstInt(VarType.Int, 77))
            )
        );
        Assert.Equal(77, val);
    }

    // Test a loop.
    [Fact]
    public void LoopTestDouble() {
        int val = TestBase(
            new StatementBlock(
                new StatementLoop(
                    new StatementLoop(
                        new StatementBreak(2)
                    )
                ),
                new StatementReturn(new ExpressionConstInt(VarType.Int, 4))
            )
        );
        Assert.Equal(4, val);
    }

    // Detect a valid return in a triple loop.
    [Fact]
    public void TripleLoopReturnTest() {
        int val = TestBase(
            new StatementLoop(
                new StatementBlock(
                    new StatementLoop(new StatementLoop(new StatementBreak(2))),
                    new StatementReturn(new ExpressionConstInt(VarType.Int, 5))
                )
            )
        );
        Assert.Equal(5, val);
    }

    // Detect an invalid return in a triple loop.
    private void TripleLoopInvalidReturnTestBody() {
        int val = TestBase(
            new StatementLoop(
                new StatementBlock(
                    new StatementLoop(new StatementLoop(new StatementBreak(3))),
                    new StatementReturn(new ExpressionConstInt(VarType.Int, 3))
                )
            )
        );
        Assert.Equal(3, val);
    }

    // Detect an invalid return in a triple loop. TODO: FIGURE OUT WHY THIS CRASHES INSTEAD OF THROWING AN EXCEPTION!
    // [Fact]
    // public void TripleLoopInvalidReturnTest() {
    //     Assert.ThrowsAny<Exception>(TripleLoopInvalidReturnTestBody);
    // }

    // TODO: TESTS FOR EDGE CASES OF RETURN, BREAK, AND CONTINUE IN LOOPS!!!

    // Test a simple do while loop.
    [Fact]
    public void DoWhileTest() {
        int val = TestBase(
            new StatementBlock(
                new StatementVariable(VarType.Int, new ExpressionConstInt(VarType.Int, 0), "i"),
                new StatementDoWhileLoop(
                    new ExpressionAssign(
                        new ExpressionVariable("i"),
                        new ExpressionLLVM(
                            "sub",
                            VarType.Int,
                            new ExpressionRValue(new ExpressionVariable("i")),
                            new ExpressionConstInt(VarType.Int, 1)
                        )
                    ),
                    new ExpressionLLVM(
                        "and",
                        VarType.Bool,
                        new ExpressionLLVM(
                            "icmpslt",
                            VarType.Bool,
                            new ExpressionRValue(new ExpressionVariable("i")),
                            new ExpressionConstInt(VarType.Int, 0)
                        ),
                        new ExpressionLLVM(
                            "icmpne",
                            VarType.Bool,
                            new ExpressionRValue(new ExpressionVariable("i")),
                            new ExpressionLLVM(
                                "sub",
                                VarType.Int,
                                new ExpressionConstInt(VarType.Int, 0),
                                new ExpressionConstInt(VarType.Int, 3)
                            )
                        )
                    )
                ),
                new StatementReturn(new ExpressionRValue(new ExpressionVariable("i")))
            )
        );
        Assert.Equal(-3, val);
    }

    // Test a simple while loop.
    [Fact]
    public void WhileTest() {
        int val = TestBase(
            new StatementBlock(
                new StatementVariable(VarType.Int, new ExpressionConstInt(VarType.Int, 4), "i"),
                new StatementWhileLoop(
                    new ExpressionAssign(
                        new ExpressionVariable("i"),
                        new ExpressionLLVM(
                            "add",
                            VarType.Int,
                            new ExpressionRValue(new ExpressionVariable("i")),
                            new ExpressionConstInt(VarType.Int, 2)
                        )
                    ),
                    new ExpressionLLVM(
                        "icmpslt",
                        VarType.Bool,
                        new ExpressionRValue(new ExpressionVariable("i")),
                        new ExpressionConstInt(VarType.Int, 7)
                    )
                ),
                new StatementReturn(new ExpressionRValue(new ExpressionVariable("i")))
            )
        );
        Assert.Equal(8, val);
    }

}