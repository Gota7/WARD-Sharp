using System.Runtime.InteropServices;
using WARD.Builders;
using WARD.Common;
using WARD.Expressions;
using WARD.Statements;
using WARD.Types;
using Xunit;

// This file is for testing all of the LLVM expressions. We just return the result of an LLVM operation and monitor its result.
public class LLVMTests
{

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate int BinaryInt32Operation(int op1, int op2);

    // Test an operation with two ints.
    public int IntTest(string func, int a, int b) {
        ProgramBuilder pb = new ProgramBuilder();
        UnitBuilder ub = new UnitBuilder("TestMod");
        var test = ub.AddFunction("test", new VarTypeFunction(VarType.Int, null, new Variable("a", VarType.Int), new Variable("b", VarType.Int)), "", new ItemAttribute("NoMangle"));
        test.Define(new StatementReturn(new ExpressionLLVM(func, VarType.Int, new ExpressionConstInt(VarType.Int, (ulong)a), new ExpressionConstInt(VarType.Int, (ulong)b))));
        pb.AddUnitBuilder(ub);
        pb.Compile();
        BinaryInt32Operation op = pb.GetFunctionExecuter<BinaryInt32Operation>("TestMod", test);
        int res = op(a, b);
        Console.WriteLine(func + "(" + a + ", " + b + ") = " + res);
        return res;
    }

    [Fact]
    public void Add() {
        Assert.Equal(7, IntTest("add", 3, 4));
    }

    [Fact]
    public void AddNSW() {
        Assert.Equal(7, IntTest("addnsw", 3, 4));
    }

    [Fact]
    public void AddNUW() {
        Assert.Equal(7, IntTest("addnuw", 3, 4));
    }

    [Fact]
    public void And() {
        Assert.Equal(0xF0, IntTest("and", 0xF8, 0xF0));
    }

    // TODO: REST OF TESTS!!!

}