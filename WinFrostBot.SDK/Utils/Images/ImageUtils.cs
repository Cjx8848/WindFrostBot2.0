using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace WindFrostBot.SDK.Utils.Images;

public class ImageUtils
{
    public static string GetBotBackground() => Directory.GetFiles("Pictures/BotBackgrounds").Rand();
    public FontFamily FontFamily { get; }

    public static readonly ImageUtils Instance = new();
    private ImageUtils()
    {
        var fc = new FontCollection();
        FontFamily = fc.Add("Pictures/default.ttf");
    }
    public static FontFamily GetFontFamily()
    {
        return Instance.FontFamily;
    }
    public static Image<Rgba32> GetAvatar(string openid, int size)
    {
        string url = $"https://q.qlogo.cn/qqapp/{MainSDK.BotConfig.AppID}/{openid}/640";
        var buffer = Utils.GetByteAsync(url).Result;//$"http://q.qlogo.cn/headimg_dl?dst_uin={uin}&spec=640&img_type=png"
        using var image = Image.Load<Rgba32>(buffer);
        var avatar = image.CutCircles(size);
        return avatar;
    }
    public void DrawImage(Image target, Image source, int X, int Y)
    {
        target.Mutate(x => x.DrawImage(source, new Point(X, Y), new GraphicsOptions()));
    }

    public void DrawText(Image image, string text, int x, int y, int fontSize, Color color)
    {
        Font font = new Font(FontFamily, fontSize);
        RichTextOptions textOptions = new(font)
        {
            Origin = new(x, y),
            TextAlignment = TextAlignment.Center
        };
        image.Mutate(ctx => ctx.DrawText(textOptions, text, color));
    }

    /// <summary>
    /// 等比例缩放
    /// </summary>
    /// <param name="image"></param>
    /// <param name="size"></param>
    public void ResetSize(Image image, int size)
    {
        int height = image.Height;
        int width = image.Width;
        if (height > width)
        {
            width = size * (width / height);
            image.Mutate(x => x.Resize(width, size));
        }
        else
        {
            height = size * (height / width);
            image.Mutate(x => x.Resize(size, height));
        }
    }
}
