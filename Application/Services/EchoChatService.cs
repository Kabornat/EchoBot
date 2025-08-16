using Persistence.Models;
using Persistence.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using User = Persistence.Models.User;

namespace Application.Services;

public class EchoChatService(
    TelegramBotClient botClient,
    UserService userService,
    ChatMessageService chatMessageService)
{
    private readonly TelegramBotClient _botClient = botClient;
    private readonly UserService _userService = userService;
    private readonly ChatMessageService _chatMessageService = chatMessageService;

    public async Task SendMessageAsync(Message senderMessage, User user)
    {
        var senderUserId = senderMessage.From.Id;

        await _userService.UpdateLastMessageSendAndGetMessages(senderUserId);

        var getterUsers = await _userService.GetMessageGettersAsync();

        var chatMessages = new List<ChatMessage>();

        var usersToLeave = new List<long>();

        InlineKeyboardMarkup inlineKeyboard;

        if (!user.Anon)
        {
            string callbackText = 
                senderMessage.From.Username is not null ?
                senderMessage.From.Username : senderMessage.From.FirstName;

            string callbackData = 
                senderMessage.From.Username is not null ?
                $"https://t.me/{senderMessage.From.Username}" : 
                $"tg://user?id={senderMessage.From.Id}";

            inlineKeyboard = InlineKeyboardButton.WithUrl(callbackText, callbackData);
        }
        else
        {
            inlineKeyboard = InlineKeyboardMarkup.Empty();
        }

        foreach (var getterUserId in getterUsers)
        {
            try
            {
                Message getterMessage;

                var replyMessageId = senderMessage.ReplyToMessage is not null
                    ? await _chatMessageService.GetMessageIdForGetter(getterUserId, senderMessage.ReplyToMessage.MessageId)
                    : 0;

                switch (senderMessage.Type)
                {
                    case MessageType.Text:
                        getterMessage = await _botClient.SendMessage(getterUserId,
                            senderMessage.Text,
                            replyParameters: replyMessageId,
                            replyMarkup: inlineKeyboard);
                        break;

                    case MessageType.Photo:
                        getterMessage = await _botClient.SendPhoto(getterUserId, 
                            senderMessage.Photo[0].FileId,
                            senderMessage.Caption,
                            replyParameters: replyMessageId,
                            replyMarkup: inlineKeyboard);
                        break;

                    case MessageType.Audio:
                        getterMessage = await _botClient.SendAudio(getterUserId, 
                            senderMessage.Audio.FileId,
                            senderMessage.Caption,
                            replyParameters: replyMessageId,
                            replyMarkup: inlineKeyboard);
                        break;

                    case MessageType.Video:
                        getterMessage = await _botClient.SendVideo(getterUserId, 
                            senderMessage.Video.FileId, 
                            senderMessage.Caption,
                            replyParameters: replyMessageId,
                            replyMarkup: inlineKeyboard);
                        break;

                    case MessageType.Voice:
                        getterMessage = await _botClient.SendVoice(getterUserId,
                            senderMessage.Audio.FileId, 
                            senderMessage.Caption,
                            replyParameters: replyMessageId,
                            replyMarkup: inlineKeyboard);
                        break;

                    case MessageType.Document:
                        getterMessage = await botClient.SendDocument(getterUserId,
                            senderMessage.Document.FileId,
                            senderMessage.Caption,
                            replyParameters: replyMessageId,
                            replyMarkup: inlineKeyboard);
                        break;

                    case MessageType.Sticker:
                        getterMessage = await botClient.SendSticker(getterUserId,
                            senderMessage.Sticker.FileId,
                            replyParameters: replyMessageId,
                            replyMarkup: inlineKeyboard);
                        break;

                    case MessageType.Location:
                        getterMessage = await botClient.SendLocation(getterUserId,
                            senderMessage.Location.Latitude,
                            senderMessage.Location.Longitude,
                            replyParameters: replyMessageId,
                            replyMarkup: inlineKeyboard);
                        break;

                    case MessageType.Contact:
                        getterMessage = await botClient.SendContact(getterUserId,
                            senderMessage.Contact.PhoneNumber,
                            senderMessage.Contact.FirstName,
                            replyParameters: replyMessageId,
                            replyMarkup: inlineKeyboard);
                        break;

                    case MessageType.VideoNote:
                        getterMessage = await botClient.SendVideoNote(getterUserId,
                           senderMessage.VideoNote.FileId,
                            replyParameters: replyMessageId,
                            replyMarkup: inlineKeyboard);
                        break;

                    case MessageType.Poll:
                        getterMessage = await botClient.ForwardMessage(getterUserId,
                            senderMessage.From.Id,
                            senderMessage.MessageId);
                        break;

                    case MessageType.Dice:
                        getterMessage = await botClient.SendDice(getterUserId,
                            emoji: senderMessage.Dice.Emoji switch
                            {
                                "🎰" => DiceEmoji.SlotMachine,
                                "🎲" => DiceEmoji.Dice,
                                "🏀" => DiceEmoji.Basketball,
                                "🎳" => DiceEmoji.Bowling,
                                "🎯" => DiceEmoji.Darts,
                                _    => DiceEmoji.Football
                            },
                            replyParameters: replyMessageId,
                            replyMarkup: inlineKeyboard);
                        break;

                    case MessageType.Animation:
                        getterMessage = await botClient.SendAnimation(getterUserId,
                            senderMessage.Animation.FileId,
                            replyParameters: replyMessageId,
                            replyMarkup: inlineKeyboard);
                        break;

                    case MessageType.Gift:
                        getterMessage = await _botClient.SendMessage(getterUserId,
                            "<b>Боту отправили подарок</b>",
                            ParseMode.Html,
                            replyParameters: replyMessageId,
                            replyMarkup: inlineKeyboard);
                        break;

                    case MessageType.UniqueGift:
                        getterMessage = await _botClient.SendMessage(getterUserId,
                            "<b>Боту отправили уникальный подарок</b>",
                            ParseMode.Html,
                            replyParameters: replyMessageId,
                            replyMarkup: inlineKeyboard);
                        break;

                    default:
                        return;
                }

                var chatMessage = new ChatMessage()
                {
                    UserId = senderUserId,
                    MessageId = senderMessage.MessageId,
                    GetterUserId = getterUserId,
                    GetterMessageId = getterMessage.MessageId,
                    SendDate = DateTime.UtcNow
                };

                chatMessages.Add(chatMessage);
            }
            catch (Exception ex)
            {
                usersToLeave.Add(getterUserId);
                Console.WriteLine(ex);
                continue;
            }
        }

        await _chatMessageService.AddAsync(chatMessages);
        await _userService.LeaveAsync(usersToLeave);
    }

    public async Task DeleteMessageAsync(Message senderMessage)
    {
        if (senderMessage.ReplyToMessage is null)
            return;

        var getterUsers = await _userService.GetAsync();

        var usersToLeave = new List<long>();

        foreach (var getterUserId in getterUsers)
        {
            try
            {
                var replyMessageId = senderMessage.ReplyToMessage is not null
                    ? await _chatMessageService.GetMessageIdForGetter(getterUserId, senderMessage.ReplyToMessage.MessageId)
                    : 0;

                if (replyMessageId == 0)
                    continue;

                await _botClient.DeleteMessage(getterUserId,
                    replyMessageId);
            }
            catch (Exception ex)
            {
                usersToLeave.Add(getterUserId);
                Console.WriteLine(ex);
                continue;
            }
        }

        await _userService.LeaveAsync(usersToLeave);

        await _botClient.SendMessage(senderMessage.From.Id, "<b>Сообщение было удалено</b>", ParseMode.Html);
    }

    public async Task PinMessageAsync(Message senderMessage)
    {
        var getterUsers = await _userService.GetMessageGettersAsync();

        var usersToLeave = new List<long>();

        foreach (var getterUserId in getterUsers)
        {
            try
            {
                var replyMessageId = senderMessage.ReplyToMessage is not null
                    ? await _chatMessageService.GetMessageIdForGetter(getterUserId, senderMessage.ReplyToMessage.MessageId)
                    : 0;

                if (replyMessageId == 0)
                    continue;

                await botClient.PinChatMessage(getterUserId,
                   replyMessageId);
            }
            catch (Exception ex)
            {
                usersToLeave.Add(getterUserId);
                Console.WriteLine(ex);
                continue;
            }
        }

        await _userService.LeaveAsync(usersToLeave);
    }
}
