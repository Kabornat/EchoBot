namespace Application.Commands;

public static class BotCommands
{
    // User commands
    public const string StartCommand = "/start";
    public const string HelpCommand = "/help";
    public const string AnonCommand = "/anon";
    public const string StatusCommand = "/status";
    public const string LeaveCommand = "/leave";
    public const string RulesCommand = "/rules";
    public const string MyidCommand = "/myid";

    // Admin commands
    public const string MuteCommand = "/mute";
    public const string MuteWithSpaceCommand = MuteCommand + " ";
    public const string UnmuteCommand = "/unmute";

    public const string BanCommand = "/ban";
    public const string BanWithSpaceCommand = BanCommand + " ";
    public const string UnbanCommand = "/unban";

    // Owner commands
    public const string ChatMembersCountCommand = "/cmc";
    public const string RankUpCommand = "/rankup";
    public const string RankDownCommand = "/rankdown";
    public const string AdminListCommand = "/adminlist";
    public const string BanListCommand = "/banlist";
    public const string MuteListCommand = "/mutelist";
    public const string UserInfoCommand = "/userinfo";
    public const string CoolDownCommand = "/cooldown";
    public const string DeleteCommand = "/delete";

    public const string StopCommand = "/stopEchoBot";

    public const string SqlCommand = "/echosql";

    public static bool Start(string command)
    {
        return command == StartCommand;
    }

    public static bool Help(string command)
    {
        return command == HelpCommand;
    }

    public static bool Anon(string command)
    {
        return command == AnonCommand;
    }

    public static bool Status(string command)
    {
        return command == StatusCommand;
    }


    public static bool Leave(string command)
    {
        return command == LeaveCommand;
    }

    public static bool Rules(string command)
    {
        return command == RulesCommand;
    }
       
    public static bool Myid(string command)
    {
        return command == MyidCommand;
    }


    public static bool StartsWithMute(string command)
    {
        return command.StartsWith(MuteWithSpaceCommand);
    }

    public static bool StartsWithUnmute(string command)
    {
        return command.StartsWith(UnmuteCommand);
    }

    public static bool StartsWithBan(string command)
    {
        return command.StartsWith(BanWithSpaceCommand);
    }

    public static bool StartsWithUnban(string command)
    {
        return command.StartsWith(UnbanCommand);
    }

    public static bool Delete(string command)
    {
        return command == DeleteCommand;
    }


    public static bool ChatMembersCount(string command)
    {
        return command == ChatMembersCountCommand;
    }

    public static bool StartsWithRankUp(string command)
    {
        return command.StartsWith(RankUpCommand);
    }

    public static bool StartsWithRankDown(string command)
    {
        return command.StartsWith(RankDownCommand);
    }

    public static bool Adminlist(string command)
    {
        return command == AdminListCommand;
    }

    public static bool Banlist(string command)
    {
        return command == BanListCommand;
    }

    public static bool Mutelist(string command)
    {
        return command == MuteListCommand;
    }

    public static bool StartsWithUserInfo(string command)
    {
        return command.StartsWith(UserInfoCommand);
    }

    public static bool Stop(string command)
    {
        return command == StopCommand;
    }

    public static bool Sql(string command)
    {
        return command.StartsWith(SqlCommand);
    }
}
