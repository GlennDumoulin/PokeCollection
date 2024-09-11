using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PokeCollection.View.Converters
{
    public class TypeToCardConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return new BitmapImage(new Uri("https://dribbble.com/shots/2832850-Pok-mon-Go?utm_source=Clipboard_Shot&utm_campaign=PanDeters&utm_content=Pok%C3%A9mon%20Go&utm_medium=Social_Share&utm_source=Clipboard_Shot&utm_campaign=PanDeters&utm_content=Pok%C3%A9mon%20Go&utm_medium=Social_Share"));

            string type = value.ToString();

            return new BitmapImage(
                new Uri($"pack://Application:,,,/Resources/Images/Cards/{type}-card.png", UriKind.Absolute)
                );
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}