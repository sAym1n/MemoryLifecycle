using MemoryLifecycle.Gc;

if (args.Length == 0)
{
    Console.WriteLine("Укажите команду: gc-pressure | all");
    return;
}

switch (args[0].ToLowerInvariant())
{
    case "gc-pressure":
        PressureScenarios.RunAll();
        break;

    case "all":
        PressureScenarios.RunAll();
        break;

    default:
        Console.WriteLine($"Неизвестная команда: {args[0]}");
        break;
}
