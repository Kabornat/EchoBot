using Persistence.OtherModels;

namespace Application.Commands;

public class BotCommands(BotData botData)
{
    public readonly string StartCommand = "/start";
    public readonly string StartWithUDCommand = "/start" + botData.UsernameWithDog;

    public readonly string AnonCommand = "/anon";
    public readonly string AnonWithUDCommand = "/anon" + botData.UsernameWithDog;

    public readonly string HelpCommand = "/help";
    public readonly string HelpWithUDCommand = "/help" + botData.UsernameWithDog;

    public readonly string LeaveCommand = "/leave";
    public readonly string RulesCommand = "/rules";
    public readonly string MyidCommand = "/myid";

    // Admin commands
    public readonly string MuteCommand = "/mute";
    public readonly string UnmuteCommand = "/unmute";
    public readonly string MuteListCommand = "/mutelist";

    public readonly string BanCommand = "/ban";
    public readonly string UnbanCommand = "/unban";
    public readonly string BanListCommand = "/banlist";

    // Owner commands
    public readonly string ChatMembersCountCommand = "/cmc";

    public readonly string RankUpCommand = "/rankup";
    public readonly string RankDownCommand = "/rankdown";
    public readonly string AdminsListCommand = "/adminslist";
    public readonly string UserInfoCommand = "/userinfo";
    public readonly string CoolDownCommand = "/cooldown";

    public readonly string StopCommand = "/stopEchoBot";

    public readonly string SqlCommand = "/echosql";

    public bool Start(string command)
    {
        return command == StartCommand || command == StartWithUDCommand;
    }

    public bool Help(string command)
    {
        return command == HelpCommand || command == HelpWithUDCommand;
    }

    public bool Anon(string command)
    {
        return command == AnonCommand || command == AnonWithUDCommand;
    }


    public bool Leave(string command)
    {
        return command == LeaveCommand;
    }

    public bool Rules(string command)
    {
        return command == RulesCommand;
    }
       
    public bool Myid(string command)
    {
        return command == MyidCommand;
    }


    public bool Stop(string command)
    {
        return command == StopCommand;
    }

    public bool Sql(string command)
    {
        return command.StartsWith(SqlCommand);
    }
}
