namespace Application;

public class Stop(CancellationTokenSource cancellationTokenSource)
{
    private readonly CancellationTokenSource _cancellationTokenSource = cancellationTokenSource;

    public async Task StopAsync()
    {
        await _cancellationTokenSource.CancelAsync();
    }
}
