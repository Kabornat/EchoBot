using Application.Commands;
using Persistence.OtherModels;

namespace Application.Services;

public class MainTextService(
    BotData botData,
    BotCommands botCommands)
{
    private readonly BotData _botData = botData;
    private readonly BotCommands _botCommands = botCommands;

    public string GetMainMenuText(Status status)
    {
        string text =
$@"
<b>{_botData.Name} 💭 Главное меню</b>

🚪 {_botCommands.LeaveCommand} - Выйти из чата
🎭 {_botCommands.AnonCommand} - Включить/выключить анонимность
❓ {_botCommands.HelpCommand} - Информация о боте
";

        if (status == Status.Admin || status == Status.Owner)
        {
            text +=
$@"
<b>Команды админов</b>

🤐 {_botCommands.MuteCommand} {{реплай/айди}} {{period в сек}} - Замутить
😦 {_botCommands.UnmuteCommand} {{реплай/айди}} - Размутить
🤜🏼 {_botCommands.BanCommand} {{реплай/айди}} - Забанить
✋🏼 {_botCommands.UnbanCommand} {{реплай/айди}} - Разбанить
📋 {_botCommands.MuteListCommand} - Список замученых
📋 {_botCommands.BanListCommand} - Список забаненых
";
        }
        if (status == Status.Owner)
        {
            text +=
$@"
<b>Команды создателя</b>

📊 {_botCommands.ChatMembersCountCommand} - Количество участников чата
⬆️ {_botCommands.RankUpCommand} {{айди}} - Возвести в админы
⬇️ {_botCommands.RankDownCommand} {{айди}} - Снять админку
📋 {_botCommands.AdminsListCommand} Список админов
ℹ️ {_botCommands.UserInfoCommand} {{реплай/айди}} - Информация о пользователе
";
        }

        return text;
    }
}

public enum Status
{
    User,
    Admin,
    Owner
}
