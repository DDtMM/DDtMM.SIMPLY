using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DDtMM.SIMPLY.Visualizer.Converters
{
    public class MultiBool : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch (parameter as string)
            {
                case "|":
                case "OR":
                    foreach (object obj in values)
                    {
                        if ((obj is bool) && (bool)obj) return true;
                    }
                    return false;
                default:
                    // and case
                    foreach (object obj in values) 
                    {
                        if (!(obj is bool) || !(bool)obj) return false;
                    }
                    return true;
            }

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
