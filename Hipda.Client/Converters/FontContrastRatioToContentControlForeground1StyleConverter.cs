﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Hipda.Client.Converters
{
    public class FontContrastRatioToContentControlForeground1StyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int val = (int)value;
            if (val == 0)
            {
                return (Style)App.Current.Resources["ContentControlForegroundStyle01"];
            }
            else if (val == 1)
            {
                return (Style)App.Current.Resources["ContentControlForegroundStyle11"];
            }
            else if (val == 2)
            {
                return (Style)App.Current.Resources["ContentControlForegroundStyle21"];
            }
            else
            {
                return (Style)App.Current.Resources["ContentControlForegroundStyle11"];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
