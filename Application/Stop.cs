namespace Application;

public class Stop(
    CancellationTokenSource cancellationTokenSource,
    CommandsList commandsList)
{
    private readonly CancellationTokenSource _cancellationTokenSource = cancellationTokenSource;
    private readonly CommandsList _commandsList = commandsList;

    public async Task StopAsync()
    {
        Console.WriteLine("bot stoping..");

        await _commandsList.OffCommandsAsync();

        await _cancellationTokenSource.CancelAsync();
    }
}
