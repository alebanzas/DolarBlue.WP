using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace DolarBlue
{
    public static class ColorExtensions
    {
        public static SolidColorBrush ToSolidColorBrush(this string hexaColor)
        {
            try
            {
                return new SolidColorBrush(
                    Color.FromArgb(
                        Convert.ToByte(hexaColor.Substring(1, 2), 16),
                        Convert.ToByte(hexaColor.Substring(3, 2), 16),
                        Convert.ToByte(hexaColor.Substring(5, 2), 16),
                        Convert.ToByte(hexaColor.Substring(7, 2), 16)
                    )
                );
            }
            catch (Exception)
            {
                return new SolidColorBrush();
            }
        }
    }
}
