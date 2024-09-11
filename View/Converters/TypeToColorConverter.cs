using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace PokeCollection.View.Converters
{
    public class TypeToColorConverter : IValueConverter
    {
        private static Dictionary<string, string> _typeColors = new Dictionary<string, string>()
        {
            { "bug", "#90c12c" },
            { "dark", "#5a5366" },
            { "dragon", "#0a6dc4" },
            { "electric", "#f3d23b" },
            { "fairy", "#ec8fe6" },
            { "fighting", "#ce4069" },
            { "fire", "#ff9c54" },
            { "flying", "#8fa8dd" },
            { "ghost", "#5269ac" },
            { "grass", "#63bb5b" },
            { "ground", "#d97746" },
            { "ice", "#74cec0" },
            { "normal", "#9099a1" },
            { "poison", "#ab6ac8" },
            { "psychic", "#f97176" },
            { "rock", "#c7b78b" },
            { "shadow", "#3f3353" },
            { "steel", "#5a8ea1" },
            { "unknown", "#689e8e" },
            { "water", "#4d90d5" },
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string type = value.ToString();

            if (_typeColors.TryGetValue(type, out string hexValue) == false)
                hexValue = "#4b4b4b";

            return hexValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
