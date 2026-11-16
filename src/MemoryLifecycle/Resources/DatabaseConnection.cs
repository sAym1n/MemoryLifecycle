using System.Runtime.InteropServices;
using System.Text;

namespace MemoryLifecycle.Resources;

public sealed class DatabaseConnection : IDisposable, IAsyncDisposable
{
    private static int counter;
    private readonly int id = ++counter;
    private readonly MemoryStream commandBuffer;
    private IntPtr nativeHandle;
    private bool disposed;

    public string ConnectionString { get; }
    public bool IsDisposed => disposed;

    public DatabaseConnection(string connectionString)
    {
        ConnectionString = connectionString;
        commandBuffer = new MemoryStream();
        nativeHandle = Marshal.AllocHGlobal(64);
        Console.WriteLine($"[DatabaseConnection {id}] открыто: {ConnectionString} (handle = 0x{nativeHandle:X})");
    }

    public void Execute(string sql)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        var bytes = Encoding.UTF8.GetBytes(sql + Environment.NewLine);
        commandBuffer.Write(bytes, 0, bytes.Length);
        Console.WriteLine($"[DatabaseConnection {id}] выполнен запрос: {sql}");
    }

    public async Task ExecuteAsync(string sql, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        var bytes = Encoding.UTF8.GetBytes(sql + Environment.NewLine);
        await commandBuffer.WriteAsync(bytes, cancellationToken);
        Console.WriteLine($"[DatabaseConnection {id}] асинхронно выполнен запрос: {sql}");
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    private async ValueTask DisposeAsyncCore()
    {
        if (disposed)
        {
            return;
        }

        await commandBuffer.FlushAsync();
        await commandBuffer.DisposeAsync();
        Console.WriteLine($"[DatabaseConnection {id}] управляемый буфер асинхронно освобождён");
    }

    private void Dispose(bool disposing)
    {
        if (disposed)
        {
            return;
        }

        if (disposing)
        {
            commandBuffer.Dispose();
            Console.WriteLine($"[DatabaseConnection {id}] управляемый буфер синхронно освобождён");
        }

        if (nativeHandle != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(nativeHandle);
            Console.WriteLine($"[DatabaseConnection {id}] нативный дескриптор 0x{nativeHandle:X} освобождён");
            nativeHandle = IntPtr.Zero;
        }

        disposed = true;
    }

    ~DatabaseConnection()
    {
        Console.WriteLine($"[DatabaseConnection {id}] финализатор вызван");
        Dispose(disposing: false);
    }
}
