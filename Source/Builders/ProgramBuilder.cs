using LLVMSharp.Interop;
using WARD.Exceptions;

namespace WARD.Builders;

// Manage optimizations.
public delegate void AddOptimizationPassesFunc(LLVMPassManagerRef fpm);

// For building an entire program.
public class ProgramBuilder {
    private Dictionary<string, UnitBuilder> CompilationUnits = new Dictionary<string, UnitBuilder>(); // Individual compilation units.
    private AddOptimizationPassesFunc Optimizations; // Optimizations to apply to each unit.
    private bool Compiled = false;

    // Create a new program builder.
    public ProgramBuilder(AddOptimizationPassesFunc optimizations = null) {
        Optimizations = optimizations;
    }

    // Add a compilation unit.
    public void AddCompilationUnit(UnitBuilder ub) {
        if (CompilationUnits.ContainsKey(ub.Path)) {
            Error.ThrowInternal("Compilation unit with same path \"" + ub.Path + "\" has already been added.");
            return;
        }
        CompilationUnits.Add(ub.Path, ub);
    }

    // Compile the program.
    public void Compile() {
        if (Compiled) {
            Error.ThrowInternal("Program has already been compiled.");
            return;
        }
        foreach (var ub in CompilationUnits) {
            var fpm = ub.Value.LLVMModule.CreateFunctionPassManager();
            if (Optimizations != null) Optimizations(fpm);
            else {
                fpm.AddInstructionCombiningPass();
                fpm.AddReassociatePass();
                fpm.AddNewGVNPass();
                fpm.AddCFGSimplificationPass();
            }
            fpm.InitializeFunctionPassManager();
            ub.Value.Compile(fpm);
        }
        Compiled = true;
    }

    // Export LLVM assembly.
    public void ExportLLVMAssembly(string compilationUnit, string path) {
        if (!Compiled) {
            Error.ThrowInternal("Program has not been compiled yet.");
            return;
        }
        if (!CompilationUnits.ContainsKey(compilationUnit)) {
            Error.ThrowInternal("Program does not contain compilation unit \"" + compilationUnit + "\".");
            return;
        }
        CompilationUnits[compilationUnit].LLVMModule.PrintToFile(path);
    }

    // Export LLVM bitcode.
    public void ExportLLVMBitcode(string compilationUnit, string path) {
        if (!Compiled) {
            Error.ThrowInternal("Program has not been compiled yet.");
            return;
        }
        if (!CompilationUnits.ContainsKey(compilationUnit)) {
            Error.ThrowInternal("Program does not contain compilation unit \"" + compilationUnit + "\".");
            return;
        }
        CompilationUnits[compilationUnit].LLVMModule.WriteBitcodeToFile(path);
    }

    // Export assembly file.
    public void ExportAssembly(string compilationUnit, string path, string triple = null, string cpu = "generic", string features = "", LLVMCodeGenOptLevel opt = LLVMCodeGenOptLevel.LLVMCodeGenLevelDefault, LLVMRelocMode reloc = LLVMRelocMode.LLVMRelocDefault, LLVMCodeModel model = LLVMCodeModel.LLVMCodeModelDefault) {
        if (!Compiled) {
            Error.ThrowInternal("Program has not been compiled yet.");
            return;
        }
        if (!CompilationUnits.ContainsKey(compilationUnit)) {
            Error.ThrowInternal("Program does not contain compilation unit \"" + compilationUnit + "\".");
            return;
        }
        ExecutionEngine.InitializeAllTargets();
        if (triple == null) triple = LLVMTargetRef.DefaultTriple;
        var target = LLVMTargetRef.GetTargetFromTriple(triple);
        var machine = target.CreateTargetMachine(triple, cpu, features, opt, reloc, model);
        LLVMModuleRef mod = CompilationUnits[compilationUnit].LLVMModule;
        mod.Target = triple;
        machine.EmitToFile(mod, path, LLVMCodeGenFileType.LLVMAssemblyFile);
    }

    // Export object file.
    public void ExportObject(string compilationUnit, string path, string triple = null, string cpu = "generic", string features = "", LLVMCodeGenOptLevel opt = LLVMCodeGenOptLevel.LLVMCodeGenLevelDefault, LLVMRelocMode reloc = LLVMRelocMode.LLVMRelocDefault, LLVMCodeModel model = LLVMCodeModel.LLVMCodeModelDefault) {
        if (!Compiled) {
            Error.ThrowInternal("Program has not been compiled yet.");
            return;
        }
        if (!CompilationUnits.ContainsKey(compilationUnit)) {
            Error.ThrowInternal("Program does not contain compilation unit \"" + compilationUnit + "\".");
            return;
        }
        ExecutionEngine.InitializeAllTargets();
        if (triple == null) triple = LLVMTargetRef.DefaultTriple;
        var target = LLVMTargetRef.GetTargetFromTriple(triple);
        var machine = target.CreateTargetMachine(triple, cpu, features, opt, reloc, model);
        LLVMModuleRef mod = CompilationUnits[compilationUnit].LLVMModule;
        mod.Target = triple;
        machine.EmitToFile(mod, path, LLVMCodeGenFileType.LLVMObjectFile);
    }

    // Execute the main function of the compiled program.
    public int Execute(string[] envp = null, params string[] args) {

        // Initialize engine.
        ExecutionEngine.InitializeAllTargets();
        LLVMExecutionEngineRef exe = null;
        LLVMValueRef func = null;
        foreach (var unit in CompilationUnits) {
            func = unit.Value.LLVMModule.GetNamedFunction("main");
            if (func != null) {
                exe = unit.Value.LLVMModule.CreateMCJITCompiler();
                break;
            }
        }
        if (exe == null) {
            Error.ThrowInternal("Program does not have a main function to execute.");
            return -1;
        }

        // Set proper envp if none and execute main.
        if (envp == null) envp = new string[0];
        return exe.RunFunctionAsMain(func, (uint)args.Length, args, envp);
    }

    // Get a function to execute from the compiled program. NOTE: Does not support variadic arguments!
    // public TDelegate GetFunctionExecuter<TDelegate>(Function function) {

    //     // Initialize engine.
    //     ExecutionEngine.InitializeAllTargets();
    //     var exe = Mods.ElementAt(0).Value.CreateMCJITCompiler();

    //     // Get args and function.
    //     LLVMValueRef func = Mods.ElementAt(0).Value.GetNamedFunction(function.ToString());
    //     var exeFunc = exe.GetPointerToGlobal<TDelegate>(func);
    //     return exeFunc;

    // }

}