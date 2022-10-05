using System.Diagnostics;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using VideoLibrary;

TelegramBotClient bot = new TelegramBotClient("Your Bot API TOKEN");


bot.OnMessage += Bot_OnMessageAsync;


Console.WriteLine("Bot ishga tushdi.");

bot.StartReceiving();

Console.ReadKey();

async void Bot_OnMessageAsync(object? sender, MessageEventArgs e)
{

    long chatId = e.Message.Chat.Id;
    string msg = e.Message.Text.Trim();

    if (msg == "/start")
    {
        await bot.SendTextMessageAsync(chatId,
            $"Assalomu aleykum hurmatli {e.Message.Chat.FirstName}.");
        return;
    }
    if (msg.ToLower().StartsWith("https://www.youtube.com/"))
    {
        await bot.SendTextMessageAsync(chatId, "Kuting ...");
        try
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            string videoLink = msg;
            var youtube = YouTube.Default;
            var videos = await youtube.GetAllVideosAsync(videoLink);
            //var video = videos.FirstOrDefault(v => v.Resolution == 720);
            //if (video is null)
            //    video = await youtube.GetVideoAsync(videoLink);
            foreach (var video in videos)
                using (Stream stream = video.Stream())
                {
                    sw.Stop();
                    await bot.SendVideoAsync(
                                chatId: chatId,
                                video: stream,
                                /*
                                // zoom rasm
                                thumb: "http://inordic.ru/images/news/2019/cs.jpg",                 
                                duration: 30, // davomiyligi                
                                height: 300, // balandligi                
                                width: 300, // kengligi
                                */
                                supportsStreaming: true, // videoni yuklash davomida ochish
                                replyToMessageId: e.Message.MessageId,
                                disableNotification: true,
                                caption: "Name: " + video.FullName + $"\nFormat: {video.Resolution}p"
                                + $"\n{sw.ElapsedMilliseconds} msek da yuklab olindi."
                                + "\n\n@vide0_d0wn_bot orqali yuklab olindi."

                            );
                }
        }
        catch (Exception ex)
        {
            await bot.SendTextMessageAsync(chatId,
                $"Exception: {ex.Message}",
                replyToMessageId: e.Message.MessageId);
        }
        return;
    }

    await bot.SendTextMessageAsync(chatId,
        "Iltimos <b><a href='https://www.youtube.com/'>youtube</a></b> video linkini yuboring!",
        ParseMode.Html);
}