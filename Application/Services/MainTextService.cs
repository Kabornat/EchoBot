using Application.Commands;
using Persistence.OtherModels;

namespace Application.Services;

public class MainTextService(
    BotData botData,
    BotCommands botCommands)
{
    private readonly BotData _botData = botData;
    private readonly BotCommands _botCommands = botCommands;

    public string GetMainMenuText(Rank status)
    {
        string text =
$@"
<b>{_botData.Name} 💭 Главное меню</b>

🚪 {_botCommands.LeaveCommand} - Выйти из чата
🎭 {_botCommands.AnonCommand} - Включить/выключить анонимность
❓ {_botCommands.HelpCommand} - Информация о боте
⚖️ {_botCommands.RulesCommand} - Правила
";

        if (status == Rank.Admin || status == Rank.Owner)
        {
            text +=
$@"
<b>Команды админов</b>

🤐 {_botCommands.MuteCommand} {{реплай/айди}} {{период в сек}} {{сообщение}} - Выдать мут
😦 {_botCommands.UnmuteCommand} {{реплай/айди}} - Размутить
🤜🏼 {_botCommands.BanCommand} {{реплай/айди}} {{сообщение}} - Забанить
✋🏼 {_botCommands.UnbanCommand} {{реплай/айди}} - Разбанить
";
        }
        if (status == Rank.Owner)
        {
            text +=
$@"
<b>Команды создателя</b>

📊 {_botCommands.ChatMembersCountCommand} - Количество участников чата
⬆️ {_botCommands.RankUpCommand} {{айди}} - Возвести в админы
⬇️ {_botCommands.RankDownCommand} {{айди}} - Снять админку
📋 {_botCommands.AdminsListCommand} Список админов
📋 {_botCommands.MuteListCommand} - Список замученых
📋 {_botCommands.BanListCommand} - Список забаненых
ℹ️ {_botCommands.UserInfoCommand} {{реплай/айди}} - Информация о пользователе
⌛️ {_botCommands.CoolDownCommand} {{период в сек}} - Установить задержку
";
        }

        return text;
    }
}

public enum Rank
{
    User,
    Admin,
    Owner
}
