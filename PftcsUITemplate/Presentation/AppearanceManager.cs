using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace PftcsUITemplate.Presentation
{
   public class AppearanceManager
    {
        public static void SetTheme(Color backColor)
        {
            Application.Current.Resources["Color_ImageOrColor"] = Visibility.Visible;
            Application.Current.Resources["Image_ImageOrColor"] = Visibility.Collapsed;

            Application.Current.Resources["DazzleUI_BackGroudColor"] = backColor;
        }

        public static void SetTheme(ImageSource backImage)
        {
            Application.Current.Resources["Color_ImageOrColor"] = Visibility.Collapsed;
            Application.Current.Resources["Image_ImageOrColor"] = Visibility.Visible;

            Application.Current.Resources["DazzleUI_BackGroudImage"] = backImage;
        }
    }
}
