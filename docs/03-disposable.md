C:\Users\user201-4\Desktop\MemoryLifecycle-main>dotnet run --project src/MemoryLifecycle -c Release -- resources


1. Финализатор: момент вызова не контролируется
[FakeResource 1] открыт
[FakeResource 2] открыт
Метод завершён, финализаторы ещё не выполнялись
[FakeResource 1] финализатор
[FakeResource 2] финализатор
После GC.Collect + WaitForPendingFinalizers

2. using: детерминированное освобождение
[ManagedResource 1] открыт
Работа внутри using-блока
[ManagedResource 1] Dispose вызван
Строка после using-блока

3. Забытый Dispose: ресурс живёт до сборки
[ManagedResource 2] открыт
Ресурс создан, IsDisposed = False
Метод завершён, Dispose не вызывался
После сборки (ресурс уже недостижим)

4. LogFile: корректный и некорректный сценарии
[LogFile 1] файл открыт: C:\Users\user201-4\AppData\Local\Temp\memory-lifecycle.log
[LogFile 1] файл закрыт
После корректного using-блока
[LogFile 2] файл открыт: C:\Users\user201-4\AppData\Local\Temp\memory-lifecycle.log
Метод с забытым Dispose завершён
[LogFile 2] финализатор! — файл не был закрыт явно
После сборки: сработал финализатор