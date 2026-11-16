using MemoryLifecycle.Resources;

namespace MemoryLifecycle.Demos;

public static class DisposeDemos
{
    public static async Task RunAllAsync()
    {
        Console.WriteLine("=== КТ №43: Полный паттерн Dispose и IAsyncDisposable ===");
        Console.WriteLine();

        await RunDatabaseConnectionDemoAsync();
        Console.WriteLine();

        await RunFileBufferDemoAsync();
    }

    public static async Task RunDatabaseConnectionDemoAsync()
    {
        Console.WriteLine("1. Вариант 1 — DatabaseConnection (using и await using):");

        DatabaseConnection syncRef;
        using (var conn = new DatabaseConnection("Server=localhost;Database=DemoSync"))
        {
            conn.Execute("SELECT 1");
            syncRef = conn;
        }

        syncRef.Dispose();

        try
        {
            syncRef.Execute("SELECT 2");
        }
        catch (ObjectDisposedException)
        {
            Console.WriteLine("[Проверка] ObjectDisposedException перехвачено после Dispose");
        }

        await using (var asyncConn = new DatabaseConnection("Server=localhost;Database=DemoAsync"))
        {
            await asyncConn.ExecuteAsync("INSERT INTO Logs VALUES ('OK')");
        }

        CreateAbandonedConnection();
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    private static void CreateAbandonedConnection()
    {
        var conn = new DatabaseConnection("Server=localhost;Database=Leaked");
        conn.Execute("SELECT * FROM Temp");
        GC.KeepAlive(conn);
    }

    public static async Task RunFileBufferDemoAsync()
    {
        Console.WriteLine("2. Вариант 2 — FileBuffer (нативная память + FileStream):");
        var syncPath = Path.Combine(Path.GetTempPath(), "filebuffer-sync.txt");
        var asyncPath = Path.Combine(Path.GetTempPath(), "filebuffer-async.txt");

        using (var buffer = new FileBuffer(syncPath, 256))
        {
            buffer.Write("Первая строка (sync)");
            buffer.Write("Вторая строка (sync)");
        }

        Console.WriteLine($"Содержимое {Path.GetFileName(syncPath)}: {File.ReadAllText(syncPath).Trim()}");

        await using (var asyncBuffer = new FileBuffer(asyncPath, 256))
        {
            asyncBuffer.Write("Первая строка (async)");
            asyncBuffer.Write("Вторая строка (async)");
        }

        Console.WriteLine($"Содержимое {Path.GetFileName(asyncPath)}: {File.ReadAllText(asyncPath).Trim()}");

        File.Delete(syncPath);
        File.Delete(asyncPath);
    }
}
