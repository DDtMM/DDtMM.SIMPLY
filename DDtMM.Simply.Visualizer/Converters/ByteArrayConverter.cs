using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DDtMM.SIMPLY.Visualizer.Converters
{
    public class ByteArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            byte[] bytes = (byte[])value;
            object result;

            if (targetType == typeof(String))
            {
                result = System.Text.Encoding.Default.GetString(bytes);
            }
            else
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    result = new BinaryFormatter().Deserialize(ms);
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            byte[] result;

            if (value is string)
            {
                result = System.Text.Encoding.Default.GetBytes((string)value);
            }
            else
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    new BinaryFormatter().Serialize(ms, value);
                    result = ms.ToArray();
                }
            }

            return result;
        }
    }
}
