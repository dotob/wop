using System;
using System.Collections.Generic;
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
  /// Interaction logic for TaskUIContainer.xaml
  /// </summary>
  public partial class TaskUIContainer : UserControl
  {
    public TaskUIContainer()
    {
      InitializeComponent();
      DataContextChanged += new DependencyPropertyChangedEventHandler(TaskUIContainer_DataContextChanged);
    }

    void TaskUIContainer_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      calcNewWorkingStyle();
    }


    private void flowButton_Click(object sender, RoutedEventArgs e)
    {
      this.calcNewWorkingStyle();
    }

    private void calcNewWorkingStyle() {
      // get bound task
      ITask task = this.DataContext as ITask;
      if (task != null) {
        // cycle button around
        TASKWORKINGSTYLE oldStyle = task.WorkingStyle;
        TASKWORKINGSTYLE newStyle = oldStyle;
        switch (oldStyle) {
          case TASKWORKINGSTYLE.STRAIGHT:
            newStyle = TASKWORKINGSTYLE.COPYOUTPUT;
            break;
          case TASKWORKINGSTYLE.COPYOUTPUT:
            newStyle = TASKWORKINGSTYLE.COPYINPUT;
            break;
          case TASKWORKINGSTYLE.COPYINPUT:
            newStyle = TASKWORKINGSTYLE.STRAIGHT;
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
        this.setButtonStyle(newStyle);
        task.WorkingStyle = newStyle;
      }
    }

    private void setButtonStyle(TASKWORKINGSTYLE taskworkingstyle)
    {
      switch (taskworkingstyle) {
        case TASKWORKINGSTYLE.STRAIGHT:
          flowButton_curvedBottom.Visibility = System.Windows.Visibility.Collapsed;
          flowButton_curvedTop.Visibility = System.Windows.Visibility.Collapsed;
          flowButton_straightTop.Visibility = System.Windows.Visibility.Visible;
          flowButton_straightBottom.Visibility = System.Windows.Visibility.Visible;
          break;
        case TASKWORKINGSTYLE.COPYOUTPUT:
          flowButton_curvedBottom.Visibility = System.Windows.Visibility.Collapsed;
          flowButton_curvedTop.Visibility = System.Windows.Visibility.Visible;
          flowButton_straightTop.Visibility = System.Windows.Visibility.Visible;
          flowButton_straightBottom.Visibility = System.Windows.Visibility.Visible;
          break;
        case TASKWORKINGSTYLE.COPYINPUT:
          flowButton_curvedBottom.Visibility = System.Windows.Visibility.Visible;
          flowButton_curvedTop.Visibility = System.Windows.Visibility.Visible;
          flowButton_straightTop.Visibility = System.Windows.Visibility.Visible;
          flowButton_straightBottom.Visibility = System.Windows.Visibility.Collapsed;
          break;
        default:
          throw new ArgumentOutOfRangeException("taskworkingstyle");
      }
    }
  }
}
