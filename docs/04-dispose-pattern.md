 КТ №43: Полный паттерн Dispose и IAsyncDisposable 

1. Вариант 1 — DatabaseConnection (using и await using):
[DatabaseConnection 1] открыто: Server=localhost;Database=DemoSync (handle = 0x2367CA12B10)
[DatabaseConnection 1] выполнен запрос: SELECT 1
[DatabaseConnection 1] управляемый буфер синхронно освобождён
[DatabaseConnection 1] нативный дескриптор 0x2367CA12B10 освобождён
[Проверка] ObjectDisposedException перехвачено после Dispose
[DatabaseConnection 2] открыто: Server=localhost;Database=DemoAsync (handle = 0x2367CA136F0)
[DatabaseConnection 2] асинхронно выполнен запрос: INSERT INTO Logs VALUES ('OK')
[DatabaseConnection 2] управляемый буфер асинхронно освобождён
[DatabaseConnection 2] нативный дескриптор 0x2367CA136F0 освобождён
[DatabaseConnection 3] открыто: Server=localhost;Database=Leaked (handle = 0x2367CA135B0)
[DatabaseConnection 3] выполнен запрос: SELECT * FROM Temp
[DatabaseConnection 3] финализатор вызван
[DatabaseConnection 3] нативный дескриптор 0x2367CA135B0 освобождён

2. Вариант 2 — FileBuffer (нативная память + FileStream):
[FileBuffer 1] открыт файл 'C:\Users\user201-4\AppData\Local\Temp\filebuffer-sync.txt', выделен нативный буфер 256 байт
[FileBuffer 1] при Dispose сброшено 68 байт
[FileBuffer 1] FileStream синхронно закрыт
[FileBuffer 1] нативная память освобождена
Содержимое filebuffer-sync.txt: Первая строка (sync)
Вторая строка (sync)
[FileBuffer 2] открыт файл 'C:\Users\user201-4\AppData\Local\Temp\filebuffer-async.txt', выделен нативный буфер 256 байт
[FileBuffer 2] при DisposeAsync сброшено 70 байт
[FileBuffer 2] FileStream асинхронно закрыт
[FileBuffer 2] нативная память освобождена
Содержимое filebuffer-async.txt: Первая строка (async)
Вторая строка (async)