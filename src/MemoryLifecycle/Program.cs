using MemoryLifecycle.Demos;
using MemoryLifecycle.Gc;

if (args.Length == 0)
{
    Console.WriteLine("Укажите команду: gc-pressure | gc-modes | resources | all");
    return;
}

switch (args[0].ToLowerInvariant())
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

    case "all":
        PressureScenarios.RunAll();
        Console.WriteLine();
        GcModeScenarios.RunParallelAllocation();
        Console.WriteLine();
        RunResources();
        break;

    default:
        Console.WriteLine($"Неизвестная команда: {args[0]}");
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
