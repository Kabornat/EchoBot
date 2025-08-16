using Application.Commands;
using Persistence.OtherModels;

namespace Application.Services;

public class MainTextService(
    BotData botData)
{
    private readonly BotData _botData = botData;

    public string GetMainMenuText(Rank status)
    {
        string text =
$@"
<b>{_botData.Name} Главное меню</b>

🚪 {BotCommands.LeaveCommand} - Выйти из чата
❓ {BotCommands.HelpCommand} - Информация о боте
🎭 {BotCommands.AnonCommand} - Включить/выключить анонимность
⚖️ {BotCommands.RulesCommand} - Правила
";

        if (status is Rank.Admin || status is Rank.Owner)
        {
            text +=
$@"
<b>Команды админов</b>

🤐 {BotCommands.MuteCommand} {{реплай/айди}} {{период в секундах}} {{сообщение}} - Выдать мут
😦 {BotCommands.UnmuteCommand} {{реплай/айди}} - Размутить
🤜🏼 {BotCommands.BanCommand} {{реплай/айди}} {{сообщение}} - Забанить
✋🏼 {BotCommands.UnbanCommand} {{реплай/айди}} - Разбанить
🗑 {BotCommands.DeleteCommand} {{реплай}} - Удалить сообщение
📌 {BotCommands.PinCommand} {{реплай}} - Закрепить сообщение
";
        }
        if (status is Rank.Owner)
        {
            text +=
$@"
<b>Команды создателя</b>

📊 {BotCommands.ChatMembersCountCommand} - Количество участников чата
⬆️ {BotCommands.RankUpCommand} {{реплай/айди}} - Возвести в админы
⬇️ {BotCommands.RankDownCommand} {{реплай/айди}} - Снять админку
📋 {BotCommands.AdminListCommand} Список админов
📋 {BotCommands.BanListCommand} - Список забаненых
📋 {BotCommands.MuteListCommand} - Список замученых
ℹ️ {BotCommands.UserInfoCommand} {{реплай/айди}} - Информация о пользователе
";
//⌛️ {BotCommands.CoolDownCommand} {{период в сек}} - Установить задержку
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
