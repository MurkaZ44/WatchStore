using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Kursach.Converters;

public class ImagePathConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            // Возвращаем изображение по умолчанию
            try
            {
                var defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Media", "watch.ico");
                if (File.Exists(defaultPath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(defaultPath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    return bitmap;
                }
            }
            catch { }
            return null;
        }

        string imagePath = value.ToString()!;
        
        // Если путь относительный, делаем его абсолютным
        if (!Path.IsPathRooted(imagePath))
        {
            imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);
        }

        if (File.Exists(imagePath))
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch
            {
                // В случае ошибки возвращаем null
                return null;
            }
        }

        // Если файл не существует, возвращаем null
        return null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

