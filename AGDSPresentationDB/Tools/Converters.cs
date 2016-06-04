using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using AGDSPresentationDB.ViewModels;

namespace AGDSPresentationDB.Tools
{
    public class IsSelectedToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool selected = (bool)value;
            SolidColorBrush brush;
            if (selected)
            {
                brush = Brushes.PaleGreen;
            }
            else
            {
                brush = Brushes.Transparent;
            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SearchOptionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            SearchOption opt = (SearchOption)value;
            string output = null;
            switch (opt)
            {
                case SearchOption.Default:
                    output = MainViewModel.SearchDefaultString;
                    break;
                case SearchOption.Depth:
                    output = MainViewModel.SearchDepthString;
                    break;
                case SearchOption.Extended:
                    output = MainViewModel.SearchExtendedString;
                    break;
            }
            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string name = value.ToString();
            if (name == MainViewModel.SearchDefaultString)
            {
                return SearchOption.Default;
            }
            else if (name == MainViewModel.SearchDepthString)
            {
                return SearchOption.Depth;
            }
            else if(name == MainViewModel.SearchExtendedString)
            {
                return SearchOption.Extended;
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }

    public class SearchOptToDepthVisibiliy : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SearchOption option = (SearchOption)value;
            if (option == SearchOption.Depth)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RelativeDepthToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int currentDepth = (int)values[0];
            int maxDept = (int)values[1];
            double relativeDepth = (double)currentDepth / (double)maxDept;
            relativeDepth = (1 - relativeDepth) * 255;
            byte green = (byte)relativeDepth;
            SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(0, green, 0));
            return brush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RelativeDepthToGraphNodeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(item => item == DependencyProperty.UnsetValue))
            {
                return Brushes.PaleGreen;
            }
            SearchOption option = (SearchOption)values[0];
            bool isSelected = (bool)values[1];
            if (option == SearchOption.Default)
            {
                if (isSelected)
                {
                    return Brushes.PaleGreen;
                }
                return Brushes.Transparent;
            }
            else if(option == SearchOption.Depth)
            {
                if (!isSelected)
                {
                    return Brushes.Transparent;
                }
                RelativeDepthToColorConverter converter = new RelativeDepthToColorConverter();
                object[] subvalues = { values[2], values[3] };
                return converter.Convert(subvalues, targetType, parameter, culture);
            }
            if (option == SearchOption.Extended)
            {
                int weight = (int)values[4];
                if (weight == Int32.MinValue)
                {
                    return Brushes.Yellow;
                }
                else
                {
                    return Brushes.PaleGreen;
                }
            }
            else
            {
                return Brushes.PaleGreen;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsSearchExtendedToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SearchOption opt = (SearchOption) value;
            if (opt == SearchOption.Extended)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsEnumSetToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var a = value as Enum;
            if (a == null) return Visibility.Collapsed;
            return a.ToString() == parameter.ToString() ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
