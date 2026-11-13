using MemoryLifecycle.Gc;

if (args.Length == 0)
{
    Console.WriteLine("Укажите команду: gc-pressure | gc-modes | all");
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

    case "all":
        PressureScenarios.RunAll();
        Console.WriteLine();
        GcModeScenarios.RunParallelAllocation();
        break;

    default:
        Console.WriteLine($"Неизвестная команда: {args.AsSpan()[0]}");
        break;
}
