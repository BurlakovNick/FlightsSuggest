using System.Threading.Tasks;

namespace FlightsSuggest.Core.Telegram
{
    public interface ITelegramClient
    {
        Task SendMessageAsync(long chatId, string text, ReplyKeyboardBuilder replyKeyboard = null);
    }
}