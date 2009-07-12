using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WOP.Util {
  public class BoolToVisibilityConverter : IValueConverter {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter,
                          CultureInfo culture)
    {
      if (targetType != typeof(Visibility)) {
        throw new InvalidOperationException("The target must be a boolean");
      }

      bool val = (bool) value;
      return val ? Visibility.Visible : Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
                              CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    #endregion
  }
}