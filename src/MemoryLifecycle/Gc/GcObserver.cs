namespace MemoryLifecycle.Gc;

public static class GcObserver
{
    public static void PrintMemoryInfo(string label)
    {
        var info = GC.GetGCMemoryInfo();
        Console.WriteLine($"{label}:");
        Console.WriteLine($"  HeapSizeBytes:       {info.HeapSizeBytes / 1024d / 1024d:F2} ла");
        Console.WriteLine($"  TotalCommittedBytes: {info.TotalCommittedBytes / 1024d / 1024d:F2} ла");
        Console.WriteLine($"  MemoryLoadBytes:     {info.MemoryLoadBytes / 1024d / 1024d:F2} ла");
        Console.WriteLine($"  PauseTimePercentage: {info.PauseTimePercentage:F2} %");
    }
}
