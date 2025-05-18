using SixLabors.Fonts;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;

namespace WindFrostBot.SDK.Utils.Images;

public class OnlineCell
{
    public string OpenID { get; set; } = "459FC29AB246694C3C875A22A696A4FD";

    public string Text { get; set; }

    public Color Color { get; set; }

    public bool UseColor { get; set; }

    public OnlineCell(string openid, string text, Color color)
    {
        OpenID = openid;
        Text = text;
        Color = color;
        UseColor = true;
    }

    public OnlineCell(string openid, string text)
    {
        OpenID = openid;
        Text = text;
    }
}

public class OnlineContent
{
    public string Title { get; set; } = "在线人数";

    public List<OnlineCell> OnlineCells { get; set; } = [];
}

public class OnlineBuilder
{
    public List<OnlineContent> Contents { get; set; } = [];

    private OnlineGenerate onlineGenerate = new();

    public static OnlineBuilder Create() => new();

    public OnlineBuilder Add(string tileName, params OnlineCell[] cells)
    {
        var content = new OnlineContent()
        {
            Title = tileName,
            OnlineCells = [.. cells]
        };
        Contents.Add(content);
        return this;
    }

    public OnlineBuilder SetFontSize(int size)
    {
        onlineGenerate.FontSize = size;
        return this;
    }

    public OnlineBuilder SetTitleFontSize(int size)
    {
        onlineGenerate.TitleFontSize = size;
        return this;
    }

    public OnlineBuilder SetTilePadding(int padding)
    {
        onlineGenerate.TilePadding = padding;
        return this;
    }

    public OnlineBuilder SetAvatarSize(int size)
    {
        onlineGenerate.AvatarSize = size;
        return this;
    }

    public OnlineBuilder SetAvatarPadding(int padding)
    {
        onlineGenerate.AvatarPadding = padding;
        return this;
    }

    public OnlineBuilder SetLineMax(int max)
    {
        onlineGenerate.LineMax = max;
        return this;
    }

    public OnlineBuilder SetSpacing(int spacing)
    {
        onlineGenerate.Spacing = spacing;
        return this;
    }

    public OnlineBuilder SetCardTopPadding(int padding)
    {
        onlineGenerate.CardTopPadding = padding;
        return this;
    }

    public OnlineBuilder SetCardBottomPadding(int padding)
    {
        onlineGenerate.CardBottomPadding = padding;
        return this;
    }

    public OnlineBuilder SetCardMargin(int margin)
    {
        onlineGenerate.CardMargin = margin;
        return this;
    }

    public OnlineBuilder SetCardDrawPadding(int padding)
    {
        onlineGenerate.CardDrawPadding = padding;
        return this;
    }

    public OnlineBuilder SetOnlinePadding(int padding)
    {
        onlineGenerate.OnlinePadding = padding;
        return this;
    }

    public byte[] Build() => onlineGenerate.DrawContent(this);
}

public class OnlineGenerate
{
    public string BackgroundPath => ImageUtils.GetBotBackground();

    public int FontSize { get; set; } = 36; //字体大小

    public int TitleFontSize { get; set; } = 80; //每个OnlineContent标题大小

    public int TilePadding { get; set; } = 40; //标题间距

    public int AvatarSize { get; set; } = 36 * 7; //绘制头像大小

    public int AvatarPadding { get; set; } = 10; //头像与文本之间的

    public int LineMax { get; set; } = 6; //一行最多绘制个数

    public int Spacing { get; set; } = 40; //绘制间隔

    public int CardTopPadding { get; set; } = 300; //卡片上方距离

    public int CardBottomPadding { get; set; } = 200; //卡片下方距离

    public int CardMargin { get; set; } = 100; //卡片左右距离

    public int CardDrawPadding { get; set; } = 100; //卡片与绘制内容距离

    public int OnlinePadding { get; set; } = 200;

    public (int Width, List<int> Heights) ComputeLayout(OnlineBuilder builder)
    {
        var family = ImageUtils.GetFontFamily();
        var font = family.CreateFont(FontSize);
        var titleFont = family.CreateFont(TitleFontSize);

        var titleSize = TextMeasurer.MeasureSize("测", new TextOptions(titleFont));

        // 计算宽度
        var width = CardMargin * 2 + CardDrawPadding * 2 + LineMax * AvatarSize + (LineMax - 1) * Spacing;

        // 计算每个OnlineContent的高度
        var heights = new List<int>();
        foreach (var content in builder.Contents)
        {
            var height = TilePadding + (int)titleSize.Height;

            if (content.OnlineCells.Count == 0)
            {
                // 设置最小高度
                height += AvatarSize + AvatarPadding + Spacing;
            }
            else
            {
                int cellCount = 0;
                foreach (var cell in content.OnlineCells)
                {
                    var textSize = TextMeasurer.MeasureSize(cell.Text, new TextOptions(font)
                    {
                        WrappingLength = AvatarSize,
                        WordBreaking = WordBreaking.BreakAll
                    });

                    if (cellCount % LineMax == 0 && cellCount != 0)
                    {
                        height += AvatarSize + AvatarPadding + Spacing;
                    }

                    cellCount++;
                }

                // 增加最后一行的高度
                height += AvatarSize + AvatarPadding + Spacing;
            }

            // 增加OnlinePadding的高度
            height += OnlinePadding;
            height += TilePadding;
            heights.Add(height);
        }

        return (width, heights);
    }



    public byte[] DrawContent(OnlineBuilder builder)
    {
        using var background = Image.Load<Rgba32>(BackgroundPath);
        var (width, heights) = ComputeLayout(builder);

        // 计算总高度
        var totalHeight = CardTopPadding + CardBottomPadding + heights.Sum();

        using var image = background.Crop(width, totalHeight);

        var family = ImageUtils.GetFontFamily();
        var font = family.CreateFont(FontSize);
        var titleFont = family.CreateFont(TitleFontSize);

        image.Mutate(ctx =>
        {
            float yOffset = CardTopPadding;
            for (int i = 0; i < builder.Contents.Count; i++)
            {
                var content = builder.Contents[i];
                var contentHeight = heights[i];

                // 绘制新的卡片背景
                ctx.DrawRoundedRectangle(CardMargin, yOffset, width - CardMargin * 2, contentHeight, 60, Color.FromRgba(255, 255, 255, 230));

                yOffset += TilePadding;

                // 计算标题宽度并居中绘制
                var titleSize = TextMeasurer.MeasureSize(content.Title, new TextOptions(titleFont));
                float titleX = (width - titleSize.Width) / 2;
                ctx.DrawText(content.Title, titleFont, Color.Black, new PointF(titleX, yOffset));
                yOffset += titleFont.Size + TilePadding;

                int cellCount = 0;
                foreach (var cell in content.OnlineCells)
                {
                    // 计算头像位置
                    int row = cellCount / LineMax;
                    int col = cellCount % LineMax;
                    int centerX = width / 2;
                    int x = centerX + (col % 2 == 0 ? 1 : -1) * ((col + 1) / 2) * (AvatarSize + Spacing);
                    if (content.OnlineCells.Count == 1)
                    {
                        x = centerX - AvatarSize / 2;
                    }
                    int y = (int)(yOffset + row * (AvatarSize + Spacing + font.Size));

                    // 绘制头像
                    var avatar = ImageUtils.GetAvatar(cell.OpenID, AvatarSize);
                    ctx.DrawImage(avatar, new Point(x, y), 1);

                    // 计算文本位置并绘制
                    var textColor = cell.UseColor ? cell.Color : Color.Black;
                    var textOptions = new RichTextOptions(font)
                    {
                        WrappingLength = AvatarSize,
                        WordBreaking = WordBreaking.BreakAll,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Top,
                        Origin = new PointF(x + AvatarSize / 2, y + AvatarSize + AvatarPadding)
                    };

                    ctx.DrawText(textOptions, cell.Text, textColor);

                    cellCount++;
                }

                if (content.OnlineCells.Count == 0)
                {
                    yOffset += AvatarSize + AvatarPadding + Spacing;
                }
                else
                {
                    yOffset += (int)Math.Ceiling(content.OnlineCells.Count / (double)LineMax) * (AvatarSize + Spacing + font.Size);
                }

                // 增加OnlinePadding的距离
                yOffset += OnlinePadding;

                // 确保每个卡片之间有足够的间距
                yOffset += TilePadding;
            }
        });

        return image.ToBytesAsync().Result;
    }
}
