using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;

namespace WindFrostBot.SDK.Utils.Images;

public class ProfileItem(string label, string value)
{
    public string Label { get; set; } = label;
    public string Value { get; set; } = value;
    public Color LabelColor { get; set; } = Color.DarkSlateGray;
    public Color ValueColor { get; set; } = Color.Black;
    public Color ValueBackgroundColor { get; set; } = Color.White;
    public bool UseEllipseBackground { get; set; } = true;
}

// 资料条目构建器
public class ProfileItemBuilder
{
    public readonly List<ProfileItem> items = [];

    private ProfileCard profileCard = new();
    public ProfileItemBuilder Add_AddText(string text)
    {
        profileCard.AddText.Add(text);
        return this;
    }
    public ProfileItemBuilder Set_AddText(List<string> text)
    {
        profileCard.AddText = text;
        return this;
    }

    // 添加基本条目 
    public ProfileItemBuilder AddItem(string label, string value)
    {
        items.Add(new ProfileItem(label, value));
        return this;
    }

    // 添加自定义颜色的条目
    public ProfileItemBuilder AddItem(string label, string value, Color labelColor, Color valueColor, Color valueBackgroundColor)
    {
        var item = new ProfileItem(label, value)
        {
            LabelColor = labelColor,
            ValueColor = valueColor,
            ValueBackgroundColor = valueBackgroundColor
        };
        items.Add(item);
        return this;
    }

    // 添加特殊样式的条目
    public ProfileItemBuilder AddSpecialItem(string label, string value, bool useEllipseBackground)
    {
        var item = new ProfileItem(label, value)
        {
            UseEllipseBackground = useEllipseBackground
        };
        items.Add(item);
        return this;
    }

    // 设置 OpenID
    public ProfileItemBuilder SetMemberOpenID(string openid)
    {
        profileCard.OpenID = openid;
        return this;
    }

    // 设置 CardOpacity
    public ProfileItemBuilder SetCardOpacity(byte cardOpacity)
    {
        profileCard.CardOpacity = cardOpacity;
        return this;
    }

    // 设置 CardWidth
    public ProfileItemBuilder SetCardWidth(int cardWidth)
    {
        profileCard.CardWidth = cardWidth;
        return this;
    }

    // 设置 CardCornerRadius
    public ProfileItemBuilder SetCardCornerRadius(float cardCornerRadius)
    {
        profileCard.CardCornerRadius = cardCornerRadius;
        return this;
    }

    // 设置 CardTopMargin
    public ProfileItemBuilder SetCardTopMargin(int cardTopMargin)
    {
        profileCard.CardTopMargin = cardTopMargin;
        return this;
    }

    // 设置 CardBottomMargin
    public ProfileItemBuilder SetCardBottomMargin(int cardBottomMargin)
    {
        profileCard.CardBottomMargin = cardBottomMargin;
        return this;
    }

    // 设置 ContentTopMargin
    public ProfileItemBuilder SetContentTopMargin(int contentTopMargin)
    {
        profileCard.ContentTopMargin = contentTopMargin;
        return this;
    }

    // 设置 ContentBottomMargin
    public ProfileItemBuilder SetContentBottomMargin(int contentBottomMargin)
    {
        profileCard.ContentBottomMargin = contentBottomMargin;
        return this;
    }

    // 设置 RowSpacing
    public ProfileItemBuilder SetRowSpacing(int rowSpacing)
    {
        profileCard.RowSpacing = rowSpacing;
        return this;
    }

    // 设置 Title
    public ProfileItemBuilder SetTitle(string title)
    {
        profileCard.Title = title;
        return this;
    }

    // 设置 Signature
    public ProfileItemBuilder SetSignature(string signature)
    {
        profileCard.Signature = signature;
        return this;
    }

    // 设置 TitleColor
    public ProfileItemBuilder SetTitleColor(Color titleColor)
    {
        profileCard.TitleColor = titleColor;
        return this;
    }

    // 设置 SignatureColor
    public ProfileItemBuilder SetSignatureColor(Color signatureColor)
    {
        profileCard.SignatureColor = signatureColor;
        return this;
    }

    // 设置 DefaultLabelColor
    public ProfileItemBuilder SetDefaultLabelColor(Color defaultLabelColor)
    {
        profileCard.DefaultLabelColor = defaultLabelColor;
        return this;
    }

    // 设置 DefaultValueColor
    public ProfileItemBuilder SetDefaultValueColor(Color defaultValueColor)
    {
        profileCard.DefaultValueColor = defaultValueColor;
        return this;
    }

    // 设置 DefaultValueBackgroundColor
    public ProfileItemBuilder SetDefaultValueBackgroundColor(Color defaultValueBackgroundColor)
    {
        profileCard.DefaultValueBackgroundColor = defaultValueBackgroundColor;
        return this;
    }

    // 设置 TitleFontSize
    public ProfileItemBuilder SetTitleFontSize(float titleFontSize)
    {
        profileCard.TitleFontSize = titleFontSize;
        return this;
    }

    // 设置 NormalFontSize
    public ProfileItemBuilder SetNormalFontSize(float normalFontSize)
    {
        profileCard.NormalFontSize = normalFontSize;
        return this;
    }

    // 设置 SmallFontSize
    public ProfileItemBuilder SetSmallFontSize(float smallFontSize)
    {
        profileCard.SmallFontSize = smallFontSize;
        return this;
    }

    // 设置 AvatarSize
    public ProfileItemBuilder SetAvatarSize(int avatarSize)
    {
        profileCard.AvatarSize = avatarSize;
        return this;
    }

    // 设置 AvatarBorderSize
    public ProfileItemBuilder SetAvatarBorderSize(int avatarBorderSize)
    {
        profileCard.AvatarBorderSize = avatarBorderSize;
        return this;
    }

    public static ProfileCard Create() => new();

    // 获取构建好的条目列表
    public byte[] Build()
    {
        return profileCard.Generate(this);
    }
}

public class ProfileCard
{
    // 配置选项
    //public uint AvatarPath { get; set; }
    public string OpenID { get; set; }
    public string BackgroundPath => ImageUtils.GetBotBackground();
    public byte CardOpacity { get; set; } = 230;
    public int CardWidth { get; set; } = 450;
    public float CardCornerRadius { get; set; } = 60;

    // 蒙层边距
    public int CardTopMargin { get; set; } = 50;
    public int CardBottomMargin { get; set; } = 50;

    // 内容边距
    public int ContentTopMargin { get; set; } = 20;
    public int ContentBottomMargin { get; set; } = 20;
    public int RowSpacing { get; set; } = 50; // 行间距

    // 个人资料
    //public List<ProfileItem> ProfileItems { get; set; } = [];
    public string Title { get; set; } = "个人资料卡";
    public string Signature { get; set; } = "Generated by PatchouliBot";

    //// 字体设置
    //public string FontPath { get; set; } = "arial.ttf";
    public Color TitleColor { get; set; } = Color.Black;
    public Color SignatureColor { get; set; } = Color.DarkGray;

    // 默认颜色（对没有指定颜色的条目使用）
    public Color DefaultLabelColor { get; set; } = Color.DarkSlateGray;
    public Color DefaultValueColor { get; set; } = Color.Black;
    public Color DefaultValueBackgroundColor { get; set; } = Color.White;

    // 字体大小
    public float TitleFontSize { get; set; } = 26;
    public float NormalFontSize { get; set; } = 18;
    public float SmallFontSize { get; set; } = 14;
    // 头像设置
    public int AvatarSize { get; set; } = 120;
    public int AvatarBorderSize { get; set; } = 5;
    public List<string> AddText { get; set; } = new List<string>();

    // 主要生成函数
    public byte[] Generate(ProfileItemBuilder builder)
    {
        try
        {
            // 创建头像
            using var avatar = ImageUtils.GetAvatar(OpenID, AvatarSize);
            // 计算尺寸和位置
            var layout = CalculateLayout(avatar.Height, builder);
            // 准备背景
            using var background = PrepareBackground(layout.BackgroundWidth, layout.BackgroundHeight);

            // 创建卡片层
            using var cardBackground = CreateCardLayer(background, layout);

            // 将内容绘制到卡片上
            DrawContent(cardBackground, avatar, layout, builder);

            return cardBackground.ToBytesAsync().Result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"生成个人资料卡时出错: {ex.Message}");
            throw;
        }
    }
    // 获取字体
    private FontFamily GetFontFamily()
    {
        return ImageUtils.Instance.FontFamily;
    }
    // 计算布局
    private (int CardHeight, int CardWidth, int BackgroundWidth, int BackgroundHeight, int CardX, int CardY) CalculateLayout(int avatarHeight, ProfileItemBuilder builder)
    {
        var fontFamily = GetFontFamily();
        var titleFont = fontFamily.CreateFont(TitleFontSize, FontStyle.Bold);
        var normalFont = fontFamily.CreateFont(NormalFontSize, FontStyle.Regular);

        // 预先计算卡片所需高度
        int titleHeight = !string.IsNullOrEmpty(Title) ? (int)TextMeasurer.MeasureSize(Title, new TextOptions(titleFont)).Height + 30 : 0;
        int avatarAreaHeight = avatarHeight + 30; // 头像高度加上额外间距
        int itemAreaHeight = builder.items.Count * RowSpacing + 20; // 所有项目的高度
        int signatureHeight = !string.IsNullOrEmpty(Signature) ? 50 : 0; // 签名高度
        int textheight = (int)TextMeasurer.MeasureSize(Signature, new TextOptions(fontFamily.CreateFont(SmallFontSize))).Height + 20;
        int addtexthight = AddText.Count * textheight;
        // 计算卡片内容总高度
        int contentHeight = ContentTopMargin + titleHeight + avatarAreaHeight + itemAreaHeight + signatureHeight + ContentBottomMargin + addtexthight;

        // 计算卡片总高度（内容高度 + 上下边距）
        int cardHeight = contentHeight;
        int cardWidth = CardWidth;

        // 计算背景图所需高度 (比卡片高度加上所需边距)
        int backgroundHeight = cardHeight + CardTopMargin + CardBottomMargin;
        int backgroundWidth = cardWidth + 50; // 左右各留25px边距

        // 计算卡片在背景中的位置
        int cardX = (backgroundWidth - cardWidth) / 2;
        int cardY = CardTopMargin;

        return (cardHeight, cardWidth, backgroundWidth, backgroundHeight, cardX, cardY);
    }

    // 准备背景
    private Image<Rgba32> PrepareBackground(int width, int height)
    {
        using Image<Rgba32> originalBackground = Image.Load<Rgba32>(BackgroundPath);

        // 创建背景画布
        var background = new Image<Rgba32>(width, height);

        // 调整原始背景大小并填充到新画布
        originalBackground.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(width, height),
            Mode = ResizeMode.Crop
        }));

        background.Mutate(x => x.DrawImage(originalBackground, new Point(0, 0), 1f));

        return background;
    }

    // 创建卡片层
    private Image<Rgba32> CreateCardLayer(Image<Rgba32> background,
        (int CardHeight, int CardWidth, int BackgroundWidth, int BackgroundHeight, int CardX, int CardY) layout)
    {
        var cardBackground = new Image<Rgba32>(layout.BackgroundWidth, layout.BackgroundHeight);
        cardBackground.Mutate(x =>
        {
            // 复制背景图像
            x.DrawImage(background, new Point(0, 0), 1f);

            // 创建圆角矩形卡片
            x.DrawRoundedRectangle(
                layout.CardX,
                layout.CardY,
                layout.CardWidth,
                layout.CardHeight,
                CardCornerRadius,
                new Rgba32(255, 255, 255, CardOpacity));
        });

        return cardBackground;
    }

    // 绘制内容
    private void DrawContent(Image<Rgba32> canvas, Image<Rgba32> avatar, (int CardHeight, int CardWidth, int BackgroundWidth, int BackgroundHeight, int CardX, int CardY) layout, ProfileItemBuilder builder)
    {
        var fontFamily = GetFontFamily();
        var titleFont = fontFamily.CreateFont(TitleFontSize, FontStyle.Bold);
        var normalFont = fontFamily.CreateFont(NormalFontSize, FontStyle.Regular);
        var smallFont = fontFamily.CreateFont(SmallFontSize, FontStyle.Regular);

        canvas.Mutate(x =>
        {
            int currentY = layout.CardY + ContentTopMargin;

            // 添加标题
            if (!string.IsNullOrEmpty(Title))
            {
                currentY = DrawTitle(x, titleFont, currentY, layout.BackgroundWidth);
            }

            // 添加头像
            currentY = DrawAvatar(x, avatar, currentY, layout.BackgroundWidth);

            // 添加信息项
            currentY = DrawProfileItems(x, normalFont, currentY, layout.CardX, layout.CardWidth, builder);

            if (AddText.Count > 0)
            {
                AddText.ForEach(text =>
                {
                    if (!string.IsNullOrEmpty(text))
                    {
                        currentY = DrawAddText(x, normalFont, currentY, layout.BackgroundWidth, text);
                    }
                });
            }

            // 添加签名
            if (!string.IsNullOrEmpty(Signature))
            {
                DrawSignature(x, smallFont, currentY, layout.BackgroundWidth);
            }
        });
    }
    //绘制附加文字
    private int DrawAddText(IImageProcessingContext context, Font Font, int currentY, int canvasWidth, string text)
    {
        var signatureOptions = new RichTextOptions(Font)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Origin = new PointF(canvasWidth / 2, currentY + 10)
        };
        context.DrawText(signatureOptions, text, Color.Black);
        var addHeight = (int)TextMeasurer.MeasureSize(text, new TextOptions(Font)).Height;
        return currentY + addHeight + 20;
    }
    // 绘制标题
    private int DrawTitle(IImageProcessingContext context, Font titleFont, int currentY, int canvasWidth)
    {
        var titleOptions = new RichTextOptions(titleFont)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Origin = new PointF(canvasWidth / 2, currentY)
        };

        context.DrawText(titleOptions, Title, TitleColor);

        var titleHeight = (int)TextMeasurer.MeasureSize(Title, new TextOptions(titleFont)).Height;
        return currentY + titleHeight + 20; // 返回新的Y坐标
    }

    // 绘制头像
    private int DrawAvatar(IImageProcessingContext context, Image avatar, int currentY, int canvasWidth)
    {
        int avatarX = canvasWidth / 2 - avatar.Width / 2;
        context.DrawImage(avatar, new Point(avatarX, currentY), 1f);

        return currentY + avatar.Height + 30; // 返回头像下方的Y坐标
    }

    // 绘制资料项
    private int DrawProfileItems(IImageProcessingContext context, Font normalFont, int currentY, int cardX, int cardWidth, ProfileItemBuilder builder)
    {
        int leftMargin = cardX + 40;
        int rightMargin = cardX + cardWidth - 40;

        foreach (var item in builder.items)
        {
            // 获取颜色，如果没有自定义颜色则使用默认颜色
            var labelColor = item.LabelColor;
            var valueColor = item.ValueColor;
            var valueBackgroundColor = item.ValueBackgroundColor;

            // 标签 - 左对齐
            var labelOptions = new RichTextOptions(normalFont)
            {
                Origin = new PointF(leftMargin, currentY),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            context.DrawText(labelOptions, item.Label, labelColor);

            // 值文本 - 右对齐
            var valueTextSize = TextMeasurer.MeasureSize(item.Value, new TextOptions(normalFont));
            int paddingX = 15;
            int paddingY = 5;

            // 根据文本尺寸设置背景
            float backgroundWidth = valueTextSize.Width + paddingX * 2;
            float backgroundHeight = valueTextSize.Height + paddingY * 2;

            // 计算值的位置
            float valueTextX = rightMargin - backgroundWidth + paddingX;
            float valueTextY = currentY;

            // 根据配置绘制背景
            if (item.UseEllipseBackground)
            {
                // 椭圆背景
                float ellipseCenterX = rightMargin - backgroundWidth / 2;
                float ellipseCenterY = currentY + valueTextSize.Height / 2;

                var ellipse = new EllipsePolygon(ellipseCenterX, ellipseCenterY, backgroundWidth / 2, backgroundHeight / 2);
                context.Fill(valueBackgroundColor, ellipse);
            }
            else
            {
                // 矩形背景
                context.Fill(valueBackgroundColor, new RectangleF(
                    rightMargin - backgroundWidth,
                    currentY,
                    backgroundWidth,
                    backgroundHeight
                ));
            }

            // 绘制文本值
            var valueTextOptions = new RichTextOptions(normalFont)
            {
                Origin = new PointF(valueTextX, valueTextY),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            context.DrawText(valueTextOptions, item.Value, valueColor);

            currentY += RowSpacing;
        }

        return currentY; // 返回最后一项下方的Y坐标
    }

    // 绘制签名
    private void DrawSignature(IImageProcessingContext context, Font smallFont, int currentY, int canvasWidth)
    {
        var signatureOptions = new RichTextOptions(smallFont)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Origin = new PointF(canvasWidth / 2, currentY + 10)
        };
        context.DrawText(signatureOptions, Signature, SignatureColor);
    }
}
