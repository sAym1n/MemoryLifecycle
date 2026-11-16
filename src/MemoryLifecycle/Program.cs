using MemoryLifecycle.Demos;
using MemoryLifecycle.Gc;

if (args.Length == 0)
{
    Console.WriteLine("Укажите команду: gc-pressure | gc-modes | resources | dispose | all");
    return;
}

switch (args.AsSpan()[0].ToLowerInvariant())
{
    case "gc-pressure":
        PressureScenarios.RunAll();
        break;

    case "gc-modes":
        GcModeScenarios.RunParallelAllocation();
        break;

    case "resources":
        RunResources();
        break;

    case "dispose":
        await DisposeDemos.RunAllAsync();
        break;

    case "all":
        PressureScenarios.RunAll();
        Console.WriteLine();
        GcModeScenarios.RunParallelAllocation();
        Console.WriteLine();
        RunResources();
        Console.WriteLine();
        await DisposeDemos.RunAllAsync();
        break;

    default:
        Console.WriteLine($"Неизвестная команда: {args.AsSpan()[0]}");
        break;
}

static void RunResources()
{
    var logPath = Path.Combine(Path.GetTempPath(), "memory-lifecycle.log");

    ResourceDemos.RunFinalizerDemo();
    ResourceDemos.RunUsingDemo();
    ResourceDemos.RunForgottenDisposeDemo();
    ResourceDemos.RunLogFileDemo(logPath);
}
