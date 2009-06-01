using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WOP.Objects;
using WOP.Tasks;
using WOP.Util;

namespace WOP.TasksUI {
  /// <summary>
  /// Interaction logic for TaskUIContainer.xaml
  /// </summary>
  public partial class TaskUIContainer : UserControl {
    public TaskUIContainer()
    {
      this.InitializeComponent();
      this.DataContextChanged += this.TaskUIContainer_DataContextChanged;
    }

    private void TaskUIContainer_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.setWorkingStyle();
    }

    private void flowButton_Click(object sender, RoutedEventArgs e)
    {
      this.calcNewWorkingStyle();
    }

    private void calcNewWorkingStyle()
    {
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

    private void setWorkingStyle()
    {
      // get bound task
      ITask task = this.DataContext as ITask;
      if (task != null) {
        // cycle button around
        this.setButtonStyle(task.WorkingStyle);
      }
    }

    private void setButtonStyle(TASKWORKINGSTYLE taskworkingstyle)
    {
      switch (taskworkingstyle) {
        case TASKWORKINGSTYLE.STRAIGHT:
          this.flowButton_curvedBottom.Visibility = Visibility.Hidden;
          this.flowButton_curvedTop.Visibility = Visibility.Hidden;
          this.flowButton_straightTop.Visibility = Visibility.Visible;
          this.flowButton_straightBottom.Visibility = Visibility.Visible;
          break;
        case TASKWORKINGSTYLE.COPYOUTPUT:
          this.flowButton_curvedBottom.Visibility = Visibility.Hidden;
          this.flowButton_curvedTop.Visibility = Visibility.Visible;
          this.flowButton_straightTop.Visibility = Visibility.Visible;
          this.flowButton_straightBottom.Visibility = Visibility.Visible;
          break;
        case TASKWORKINGSTYLE.COPYINPUT:
          this.flowButton_curvedBottom.Visibility = Visibility.Visible;
          this.flowButton_curvedTop.Visibility = Visibility.Visible;
          this.flowButton_straightTop.Visibility = Visibility.Visible;
          this.flowButton_straightBottom.Visibility = Visibility.Hidden;
          break;
        default:
          throw new ArgumentOutOfRangeException("taskworkingstyle");
      }
    }

    private void ButtonUpClick(object sender, RoutedEventArgs e)
    {
      // get bound task
      ITask task = this.DataContext as ITask;
      if (task != null) {
        Job j = task.ParentJob;
        j.moveTaskUp(task);
      }
    }

    private void buttonDownClick(object sender, RoutedEventArgs e)
    {
//      // try to position mouse on button
//      Button b = sender as Button;
//      if (b != null)
//      {
//        Point relativePoint = b.TransformToAncestor(this)
//                              .Transform(new Point(0, 0));
//
//        Point screen = b.PointToScreen(relativePoint);
//        //UIUtils.SetCursorPos((int)screen.X, (int)screen.Y);
//        UIUtils.SetCursorPos(50, 50);
//      }

      // get bound task
      ITask task = this.DataContext as ITask;
      if (task != null) {
        Job j = task.ParentJob;
        j.moveTaskDown(task);
      }

    }
  }
}