namespace Application.Commands;

public class BotCommands
{
    public readonly string StopCommand = "/stopEchoBot";

    public readonly string SqlCommand = "/sqlecho";

    public bool Stop(string command)
    {
        return command == StopCommand;
    }

    public bool Sql(string command)
    {
        return command.StartsWith(SqlCommand);
    }
}
