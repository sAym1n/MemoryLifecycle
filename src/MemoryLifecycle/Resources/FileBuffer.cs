using System.Runtime.InteropServices;
using System.Text;

namespace MemoryLifecycle.Resources;

public sealed class FileBuffer : IDisposable, IAsyncDisposable
{
    private static int counter;
    private readonly int id = ++counter;
    private readonly FileStream fileStream;
    private readonly int capacity;
    private IntPtr unmanagedBuffer;
    private int offset;
    private bool disposed;

    public string FilePath { get; }
    public bool IsDisposed => disposed;

    public FileBuffer(string filePath, int capacity = 1024)
    {
        FilePath = filePath;
        this.capacity = capacity;
        fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read, capacity, useAsync: true);
        unmanagedBuffer = Marshal.AllocHGlobal(capacity);
        Console.WriteLine($"[FileBuffer {id}] открыт файл '{FilePath}', выделен нативный буфер {capacity} байт");
    }

    public void Write(string text)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        var bytes = Encoding.UTF8.GetBytes(text + Environment.NewLine);
        if (offset + bytes.Length > capacity)
        {
            Flush();
        }

        Marshal.Copy(bytes, 0, IntPtr.Add(unmanagedBuffer, offset), bytes.Length);
        offset += bytes.Length;
    }

    public void Flush()
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        if (offset == 0)
        {
            return;
        }

        var temp = new byte[offset];
        Marshal.Copy(unmanagedBuffer, temp, 0, offset);
        fileStream.Write(temp, 0, offset);
        fileStream.Flush();
        Console.WriteLine($"[FileBuffer {id}] синхронно записано {offset} байт на диск");
        offset = 0;
    }

    public async ValueTask FlushAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        if (offset == 0)
        {
            return;
        }

        var temp = new byte[offset];
        Marshal.Copy(unmanagedBuffer, temp, 0, offset);
        await fileStream.WriteAsync(temp, cancellationToken);
        await fileStream.FlushAsync(cancellationToken);
        Console.WriteLine($"[FileBuffer {id}] асинхронно записано {offset} байт на диск");
        offset = 0;
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

        if (offset > 0 && unmanagedBuffer != IntPtr.Zero)
        {
            var temp = new byte[offset];
            Marshal.Copy(unmanagedBuffer, temp, 0, offset);
            await fileStream.WriteAsync(temp);
            await fileStream.FlushAsync();
            Console.WriteLine($"[FileBuffer {id}] при DisposeAsync сброшено {offset} байт");
            offset = 0;
        }

        await fileStream.DisposeAsync();
        Console.WriteLine($"[FileBuffer {id}] FileStream асинхронно закрыт");
    }

    private void Dispose(bool disposing)
    {
        if (disposed)
        {
            return;
        }

        if (disposing)
        {
            if (offset > 0 && unmanagedBuffer != IntPtr.Zero)
            {
                var temp = new byte[offset];
                Marshal.Copy(unmanagedBuffer, temp, 0, offset);
                fileStream.Write(temp, 0, offset);
                fileStream.Flush();
                Console.WriteLine($"[FileBuffer {id}] при Dispose сброшено {offset} байт");
                offset = 0;
            }

            fileStream.Dispose();
            Console.WriteLine($"[FileBuffer {id}] FileStream синхронно закрыт");
        }

        if (unmanagedBuffer != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(unmanagedBuffer);
            unmanagedBuffer = IntPtr.Zero;
            Console.WriteLine($"[FileBuffer {id}] нативная память освобождена");
        }

        disposed = true;
    }

    ~FileBuffer()
    {
        Console.WriteLine($"[FileBuffer {id}] финализатор вызван");
        Dispose(disposing: false);
    }
}
