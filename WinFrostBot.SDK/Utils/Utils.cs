using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;
using System.Web;
using SixLabors.ImageSharp.Drawing.Processing;
using WindFrostBot.SDK.Utils.Images;

namespace WindFrostBot.SDK.Utils
{
    public static class Utils
    {
        public static void DrawRoundedRectanglePath(this IImageProcessingContext context, float x, float y, float width, float height, float cornerRadius, int size, Rgba32 color)
        {
            if (cornerRadius <= 0)
            {
                // 如果没有圆角，就直接绘制矩形
                context.Draw(color, size, new RectangleF(x, y, width, height));
                return;
            }
            var radius = cornerRadius * 2;
            var pathBuilder = new PathBuilder();
            pathBuilder.StartFigure()
                .AddLine(x + cornerRadius, y, x + width - cornerRadius, y)
                .AddArc(new RectangleF(x + width - radius, y, radius, radius), 0, 270, 90)
                .AddLine(x + width, y + cornerRadius, x + width, y + height - cornerRadius)
                .AddArc(new RectangleF(x + width - radius, y + height - radius, radius, radius), 0, 0, 90)
                .AddLine(x + width - cornerRadius, y + height, x + cornerRadius, y + height)
                .AddArc(new RectangleF(x, y + height - radius, radius, radius), 0, 90, 90)
                .AddLine(x, y + height - cornerRadius, x, y + cornerRadius)
                .AddArc(new RectangleF(x, y, radius, radius), 0, 180, 90)
                .CloseFigure();
            var path = pathBuilder.Build();
            context.Draw(color, size, path);
        }
        public static void DrawRoundedRectangle(this IImageProcessingContext context, float x, float y, float width, float height, float cornerRadius, Rgba32 color)
        {
            if (cornerRadius <= 0)
            {
                // 如果没有圆角，就直接绘制矩形
                context.Fill(color, new RectangleF(x, y, width, height));
                return;
            }
            var radius = cornerRadius * 2;
            var pathBuilder = new PathBuilder();
            pathBuilder.StartFigure()
                .AddLine(x + cornerRadius, y, x + width - cornerRadius, y)
                .AddArc(new RectangleF(x + width - radius, y, radius, radius), 0, 270, 90)
                .AddLine(x + width, y + cornerRadius, x + width, y + height - cornerRadius)
                .AddArc(new RectangleF(x + width - radius, y + height - radius, radius, radius), 0, 0, 90)
                .AddLine(x + width - cornerRadius, y + height, x + cornerRadius, y + height)
                .AddArc(new RectangleF(x, y + height - radius, radius, radius), 0, 90, 90)
                .AddLine(x, y + height - cornerRadius, x, y + cornerRadius)
                .AddArc(new RectangleF(x, y, radius, radius), 0, 180, 90)
                .CloseFigure();
            var path = pathBuilder.Build();
            context.Fill(color, path);
        }
        public static Image<Rgba32> Crop(this Image<Rgba32> image, int width, int height)
        {
            var option = new GraphicsOptions
            {
                Antialias = true,
                AntialiasSubpixelDepth = 16,
                BlendPercentage = 1,
                AlphaCompositionMode = PixelAlphaCompositionMode.Src
            };
            var background = new Image<Rgba32>(width, height);
            background.Mutate(x => x.SetGraphicsOptions(option));
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(width, height),
                Mode = ResizeMode.Crop
            }));
            background.Mutate(x => x.DrawImage(image, new Point(0, 0), 1f));
            return background;
        }

        /// <summary>
        /// 裁剪为圆形图片
        /// </summary>
        /// <param name="image"></param>
        /// <param name="diameter"></param>
        /// <param name="BorderSize"></param>
        /// <returns></returns>
        public static Image<Rgba32> CutCircles(this Image<Rgba32> image, int diameter, int BorderSize = 5)
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(diameter, diameter),
                Mode = ResizeMode.Crop
            }));

            // 创建圆形头像
            using var circular = new Image<Rgba32>(diameter, diameter);
            circular.Mutate(x =>
            {
                var circle = new EllipsePolygon(diameter / 2, diameter / 2, diameter / 2);
                x.Clear(Color.Transparent);
                x.Fill(Color.White, circle);
                x.DrawImage(image, new Point(0, 0), new GraphicsOptions
                {
                    ColorBlendingMode = PixelColorBlendingMode.Multiply,
                    AlphaCompositionMode = PixelAlphaCompositionMode.SrcIn
                });
            });

            // 创建带边框的头像
            int finalSize = diameter + BorderSize * 2;
            var final = new Image<Rgba32>(finalSize, finalSize);
            final.Mutate(x =>
            {
                var borderCircle = new EllipsePolygon(finalSize / 2, finalSize / 2, finalSize / 2);
                x.Fill(Color.White, borderCircle);
                x.DrawImage(circular, new Point(BorderSize, BorderSize), 1f);
            });
            return final;
        }

        /// <summary>
        /// 将 Image 转换为 byte[]
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static async Task<byte[]> ToBytesAsync(this Image<Rgba32> image)
        {
            await using var ms = new MemoryStream();
            await image.SaveAsPngAsync(ms);
            return ms.ToArray();
        }
        public static ProfileItemBuilder AddIf(this ProfileItemBuilder b, bool condition, string key, string value) => condition ? b.AddItem(key, value) : b;
        private static readonly Random _random = new Random();
        public static T Rand<T>(this IEnumerable<T> source)
        {
            return source.ElementAt(_random.Next(0, source.Count()));
        }
        private static readonly HttpClient HttpClient = new();
        public static async Task<byte[]> GetByteAsync(string url, Dictionary<string, string>? args = null)
        {
            UriBuilder uriBuilder = new UriBuilder(url);
            System.Collections.Specialized.NameValueCollection param = HttpUtility.ParseQueryString(uriBuilder.Query);
            if (args != null)
                foreach ((string key, string val) in args)
                    param[key] = val;
            uriBuilder.Query = param.ToString();
            return await HttpClient.GetByteArrayAsync(uriBuilder.ToString());
        }
        /*
        public static void SendTextMessage(string message, long group, int type = 0)
        {
            switch (type)
            {
                case 0:

                    break;
                default:
                    break;
            }
        }
        public static void SendImage(System.Drawing.Image img, long group, int type = 0)
        {
            switch (type)
            {
                case 0:

                    break;
            }
        }
        */
    }
}
