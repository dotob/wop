using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WOP.Objects;

namespace WOP.TasksUI
{
  /// <summary>
  /// Interaction logic for ImageShrinkTaskUI.xaml
  /// </summary>
  public partial class ImageShrinkTaskUI : UserControl
  {
    public ImageShrinkTaskUI()
    {
      this.InitializeComponent();
    }
  }

  public class WorkingStyle2BoolConverter :IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if(value is TASKWORKINGSTYLE) {
        TASKWORKINGSTYLE ws = (TASKWORKINGSTYLE) value;
        if (ws != TASKWORKINGSTYLE.STRAIGHT) {
          return true;
        } 
      }
      return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}