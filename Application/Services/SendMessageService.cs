using Persistence.Models;
using Persistence.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Application.Services;

public class SendMessageService(
    TelegramBotClient botClient,
    UserService userService,
    ChatMessageService chatMessageService)
{
    private readonly TelegramBotClient _botClient = botClient;
    private readonly UserService _userService = userService;
    private readonly ChatMessageService _chatMessageService = chatMessageService;

    public async Task SendAsync(Message senderMessage)
    {
        var senderUserId = senderMessage.From.Id;

        await _userService.UpdateLastMessageSendAndGetMessages(senderUserId);

        var getterUsers = await _userService.GetMessageGettersAsync();

        var chatMessages = new List<ChatMessage>();

        var usersToLeave = new List<long>();

        foreach (var getterUserId in getterUsers)
        {
            try
            {
                Message getterMessage;

                var replyMessageId = senderMessage.ReplyToMessage is not null
                    ? await _chatMessageService.GetMessageIdForReply(getterUserId, senderMessage.ReplyToMessage.MessageId)
                    : 0;

                switch (senderMessage.Type)
                {
                    case MessageType.Text:
                        getterMessage = await _botClient.SendMessage(getterUserId,
                            senderMessage.Text,
                            replyParameters: replyMessageId);
                        break;

                    case MessageType.Photo:
                        getterMessage = await _botClient.SendPhoto(getterUserId, 
                            senderMessage.Photo[0].FileId,
                            senderMessage.Caption,
                            replyParameters: replyMessageId);
                        break;

                    case MessageType.Audio:
                        getterMessage = await _botClient.SendAudio(getterUserId, 
                            senderMessage.Audio.FileId,
                            senderMessage.Caption,
                            replyParameters: replyMessageId);
                        break;

                    case MessageType.Video:
                        getterMessage = await _botClient.SendVideo(getterUserId, 
                            senderMessage.Video.FileId, 
                            senderMessage.Caption,
                            replyParameters: replyMessageId);
                        break;

                    case MessageType.Voice:
                        getterMessage = await _botClient.SendVoice(getterUserId,
                            senderMessage.Audio.FileId, 
                            senderMessage.Caption,
                            replyParameters: replyMessageId);
                        break;

                    case MessageType.Document:
                        getterMessage = await botClient.SendDocument(getterUserId,
                            senderMessage.Document.FileId,
                            senderMessage.Caption,
                            replyParameters: replyMessageId);
                        break;

                    case MessageType.Sticker:
                        getterMessage = await botClient.SendSticker(getterUserId,
                            senderMessage.Sticker.FileId,
                            replyParameters: replyMessageId);
                        break;

                    case MessageType.Location:
                        getterMessage = await botClient.SendLocation(getterUserId,
                            senderMessage.Location.Latitude,
                            senderMessage.Location.Longitude,
                            replyParameters: replyMessageId);
                        break;

                    case MessageType.Contact:
                        getterMessage = await botClient.SendContact(getterUserId,
                            senderMessage.Contact.PhoneNumber,
                            senderMessage.Contact.FirstName,
                            replyParameters: replyMessageId);
                        break;

                    case MessageType.VideoNote:
                        getterMessage = await botClient.SendVideoNote(getterUserId,
                           senderMessage.VideoNote.FileId,
                            replyParameters: replyMessageId);
                        break;

                    //case MessageType.PinnedMessage:
                    //    await botClient.PinChatMessage(getterUserId,
                    //       message.MessageId);
                    //    continue;

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
                            replyParameters: replyMessageId);
                        break;

                    case MessageType.Animation:
                        getterMessage = await botClient.SendAnimation(getterUserId,
                            senderMessage.Animation.FileId,
                            replyParameters: replyMessageId);
                        break;

                    case MessageType.Gift:
                        getterMessage = await _botClient.SendMessage(getterUserId,
                            "<b>Боту отправили подарок</b>",
                            ParseMode.Html,
                            replyParameters: replyMessageId);
                        break;

                    case MessageType.UniqueGift:
                        getterMessage = await _botClient.SendMessage(getterUserId,
                            "<b>Боту отправили уникальный подарок</b>",
                            ParseMode.Html,
                            replyParameters: replyMessageId);
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
}
