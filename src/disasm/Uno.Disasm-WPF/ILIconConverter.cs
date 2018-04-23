using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Uno.Disasm.ILView;

namespace Uno.Disasm
{
    [ValueConversion(typeof(ILIcon), typeof(BitmapImage))]
    public class ILIconConverter : IValueConverter
    {
        static readonly Dictionary<ILIcon, BitmapImage> _bitmaps = new Dictionary<ILIcon, BitmapImage>();

        public static BitmapImage GetBitmapImage(ILIcon value)
        {
            BitmapImage bitmap;
            if (!_bitmaps.TryGetValue(value, out bitmap))
            {
                try
                {
                    using (var stream = ILIconInfo.OpenRead(value))
                    {
                        if (stream != null)
                        {
                            bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.StreamSource = stream;
                            bitmap.EndInit();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                    bitmap = null;
                }

                _bitmaps.Add(value, bitmap);
            }
            
            return bitmap;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetBitmapImage((ILIcon)value);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}