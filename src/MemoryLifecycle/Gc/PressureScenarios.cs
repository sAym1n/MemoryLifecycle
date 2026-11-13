using System.Diagnostics;

namespace MemoryLifecycle.Gc;

public readonly record struct ScenarioResult(
    string Name,
    TimeSpan Elapsed,
    GcDelta Delta);

public static class PressureScenarios
{
    private static readonly List<byte[]> LongLivedStorage = [];

    public static ScenarioResult RunSmallObjects(int count = 100_000)
    {
        var before = GcSnapshot.Capture();
        var stopwatch = Stopwatch.StartNew();

        long checksum = 0;
        for (var i = 0; i < count; i++)
        {
            var buffer = new byte[100];
            buffer[0] = (byte)i;
            checksum += buffer[0];
        }

        stopwatch.Stop();
        var after = GcSnapshot.Capture();
        GC.KeepAlive(checksum);

        return new ScenarioResult("Мелкие объекты (byte[100] x 100k)", stopwatch.Elapsed, after.Diff(before));
    }

    public static ScenarioResult RunLargeObjects(int count = 10)
    {
        var before = GcSnapshot.Capture();
        var stopwatch = Stopwatch.StartNew();

        var localLargeList = new List<byte[]>(count);
        for (var i = 0; i < count; i++)
        {
            localLargeList.Add(new byte[100_000]);
        }

        stopwatch.Stop();
        var after = GcSnapshot.Capture();
        GC.KeepAlive(localLargeList);

        return new ScenarioResult("Крупные объекты (byte[100k] x 10)", stopwatch.Elapsed, after.Diff(before));
    }

    public static ScenarioResult RunLongLived(int count = 5_000)
    {
        LongLivedStorage.Clear();

        var before = GcSnapshot.Capture();
        var stopwatch = Stopwatch.StartNew();

        for (var i = 0; i < count; i++)
        {
            LongLivedStorage.Add(new byte[1024]);
        }

        stopwatch.Stop();
        var after = GcSnapshot.Capture();

        return new ScenarioResult("Долгоживущие в static (byte[1k] x 5k)", stopwatch.Elapsed, after.Diff(before));
    }

    public static void RunAll()
    {
        long initialAllocated = GC.GetTotalAllocatedBytes(precise: true);

        Console.WriteLine("=== КТ №40: Измерение давления на управляемую кучу (Вариант 2) ===");
        Console.WriteLine($"Начальный GC.GetTotalAllocatedBytes: {initialAllocated / 1024d / 1024d:F2} МБ\n");

        GcObserver.PrintMemoryInfo("Состояние кучи ДО экспериментов");
        Console.WriteLine(new string('-', 96));
        Console.WriteLine(
            "{0,-38}{1,10}{2,7}{3,7}{4,7}{5,13}{6,14}",
            "Сценарий", "Время, мс", "Gen0", "Gen1", "Gen2", "Alloc, МБ", "Heap Δ, МБ");
        Console.WriteLine(new string('-', 96));

        var results = new[]
        {
            RunSmallObjects(),
            RunLargeObjects(),
            RunLongLived()
        };

        foreach (var result in results)
        {
            Console.WriteLine(
                "{0,-38}{1,10:F2}{2,7}{3,7}{4,7}{5,13:F2}{6,14:F2}",
                result.Name,
                result.Elapsed.TotalMilliseconds,
                result.Delta.Gen0,
                result.Delta.Gen1,
                result.Delta.Gen2,
                result.Delta.AllocatedMegabytes,
                result.Delta.HeapDeltaMegabytes);
        }

        Console.WriteLine(new string('-', 96));
        GcObserver.PrintMemoryInfo("Состояние кучи ПОСЛЕ экспериментов");

        long finalAllocated = GC.GetTotalAllocatedBytes(precise: true);
        Console.WriteLine($"\nИтоговый GC.GetTotalAllocatedBytes:  {finalAllocated / 1024d / 1024d:F2} МБ");
        Console.WriteLine($"Общий прирост за все сценарии:     {(finalAllocated - initialAllocated) / 1024d / 1024d:F2} МБ");
    }
}
