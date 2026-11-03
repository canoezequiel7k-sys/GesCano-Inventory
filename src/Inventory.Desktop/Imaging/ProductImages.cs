using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Inventory.Desktop.Imaging;

// Guarda y carga las imágenes de producto dentro de la carpeta de la aplicación.
public static class ProductImages
{
    private const string ImagesFolder = "images";

    private static readonly Dictionary<string, Color> CategoryColors = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Bebidas"] = Color.FromArgb(41, 128, 185),
        ["Cereales y desayuno"] = Color.FromArgb(211, 132, 44),
        ["Golosinas"] = Color.FromArgb(192, 57, 43),
        ["Panadería"] = Color.FromArgb(176, 122, 74),
        ["Almacén"] = Color.FromArgb(127, 140, 141),
        ["Lácteos"] = Color.FromArgb(52, 152, 219),
        ["Limpieza"] = Color.FromArgb(22, 160, 133),
        ["Perfumería"] = Color.FromArgb(155, 89, 182),
        ["Congelados"] = Color.FromArgb(52, 73, 94),
        ["Carnes"] = Color.FromArgb(192, 57, 43),
        ["Frutas y Verduras"] = Color.FromArgb(39, 174, 96)
    };

    public static string? Save(string sourcePath)
    {
        if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
            return null;

        var directory = Path.Combine(AppContext.BaseDirectory, ImagesFolder);
        Directory.CreateDirectory(directory);

        var extension = Path.GetExtension(sourcePath);
        if (string.IsNullOrWhiteSpace(extension))
            extension = ".png";

        var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var destination = Path.Combine(directory, fileName);

        File.Copy(sourcePath, destination, overwrite: false);

        return Path.Combine(ImagesFolder, fileName).Replace('\\', '/');
    }

    public static string? Resolve(string? storedPath)
    {
        if (string.IsNullOrWhiteSpace(storedPath))
            return null;

        if (Path.IsPathRooted(storedPath))
            return File.Exists(storedPath) ? storedPath : null;

        var fullPath = Path.Combine(AppContext.BaseDirectory, storedPath);
        return File.Exists(fullPath) ? fullPath : null;
    }

    /// <summary>
    /// Busca la imagen guardada y, si no existe, una imagen masiva por PLU o SKU
    /// en images/products (por ejemplo 1157.png o ALM-015.jpg).
    /// </summary>
    public static string? ResolveForProduct(string? storedPath, string plu, string sku)
    {
        var direct = Resolve(storedPath);
        if (direct is not null)
            return direct;

        var folder = Path.Combine(AppContext.BaseDirectory, ImagesFolder, "products");
        if (!Directory.Exists(folder))
            return null;

        string[] keys = [plu, sku];
        string[] extensions = [".png", ".jpg", ".jpeg", ".bmp", ".gif"];

        foreach (var key in keys)
        {
            if (string.IsNullOrWhiteSpace(key))
                continue;

            foreach (var extension in extensions)
            {
                var candidate = Path.Combine(folder, key.Trim() + extension);
                if (File.Exists(candidate))
                    return candidate;
            }
        }

        return null;
    }

    public static Image? Load(string? storedPath) => LoadFile(Resolve(storedPath));

    public static Image? LoadForProduct(string? storedPath, string plu, string sku)
        => LoadFile(ResolveForProduct(storedPath, plu, sku));

    private static Image? LoadFile(string? fullPath)
    {
        if (fullPath is null)
            return null;

        try
        {
            // Se copia a memoria para no dejar el archivo bloqueado por GDI+.
            using var stream = new MemoryStream(File.ReadAllBytes(fullPath));
            using var image = Image.FromStream(stream);
            return new Bitmap(image);
        }
        catch
        {
            // GDI+ no soporta WebP ni algunos formatos modernos. WPF/WIC sí.
            return LoadWithWpf(fullPath);
        }
    }

    private static Image? LoadWithWpf(string fullPath)
    {
        try
        {
            var decoder = BitmapDecoder.Create(
                new Uri(fullPath),
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.OnLoad);

            var frame = decoder.Frames[0];
            var converted = new FormatConvertedBitmap(frame, System.Windows.Media.PixelFormats.Bgra32, null, 0);

            using var stream = new MemoryStream();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(converted));
            encoder.Save(stream);

            stream.Position = 0;
            using var bitmap = new Bitmap(stream);
            return new Bitmap(bitmap);
        }
        catch
        {
            return null;
        }
    }

    public static Image CreatePlaceholder(string name, string category, int width = 320, int height = 320)
    {
        var color = CategoryColors.TryGetValue(category, out var found)
            ? found
            : Color.FromArgb(52, 73, 94);

        var bitmap = new Bitmap(width, height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

        using (var background = new LinearGradientBrush(
            new Rectangle(0, 0, width, height),
            ControlPaint.Light(color, 0.15f),
            ControlPaint.Dark(color, 0.2f),
            45f))
        {
            graphics.FillRectangle(background, 0, 0, width, height);
        }

        using (var circleBrush = new SolidBrush(Color.FromArgb(35, 255, 255, 255)))
        {
            graphics.FillEllipse(circleBrush, width * 0.58f, -height * 0.2f, width * 0.7f, height * 0.7f);
        }

        var initials = GetInitials(name);
        using (var initialsFont = new Font("Segoe UI", width * 0.24f, FontStyle.Bold, GraphicsUnit.Pixel))
        using (var initialsBrush = new SolidBrush(Color.White))
        using (var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        })
        {
            graphics.DrawString(initials, initialsFont, initialsBrush,
                new RectangleF(0, 0, width, height * 0.82f), format);
        }

        using (var categoryFont = new Font("Segoe UI", width * 0.045f, FontStyle.Regular, GraphicsUnit.Pixel))
        using (var categoryBrush = new SolidBrush(Color.FromArgb(225, 255, 255, 255)))
        using (var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        })
        {
            graphics.DrawString(category, categoryFont, categoryBrush,
                new RectangleF(0, height * 0.72f, width, height * 0.18f), format);
        }

        return bitmap;
    }

    private static string GetInitials(string name)
    {
        var words = (name ?? string.Empty)
            .Split(new[] { ' ', '-', '.' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(word => word.Length > 0 && char.IsLetter(word[0]))
            .Take(2)
            .ToList();

        if (words.Count == 0)
            return "?";

        return string.Concat(words.Select(word => char.ToUpperInvariant(word[0])));
    }
}
