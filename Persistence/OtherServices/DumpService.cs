using Persistence.OtherModels;

namespace Persistence.OtherServices;

public class DumpService(DumpConfiguration dumpConfiguration)
{
    private readonly DumpConfiguration _dumpConfiguration = dumpConfiguration;

    public async void CreateNewAndKeep30(object? state)
    {
        if (!_dumpConfiguration.Enable)
            return;

        await CreateAsync();
        await DeleteToKeep30Async();
    }

    public async Task CreateAsync(string? name = null)
    {
        if (string.IsNullOrEmpty(name))
            name = DateTime.UtcNow.ToString("dd.MM.yyyyy-HH.mm.ss");

        Console.WriteLine("Создан дамп: " + name);
    }

    public async Task Delete(string name)
    {
        Console.WriteLine("Удален дамп: " + name);
    }

    public async Task DeleteToKeep30Async()
    {
        string? name = null;

        Console.WriteLine("Удален дамп: " + name);
    }
}
