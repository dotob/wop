using System;
using System.Windows;
using System.Windows.Controls;
using WOP.Objects;
using WOP.Tasks;

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
            if (TASKWORKINGSTYLE.COPYOUTPUT == (task.WorkingStyleConstraint & TASKWORKINGSTYLE.COPYOUTPUT)) {
              newStyle = TASKWORKINGSTYLE.COPYOUTPUT;
            } else if (TASKWORKINGSTYLE.COPYINPUT == (task.WorkingStyleConstraint & TASKWORKINGSTYLE.COPYINPUT)) {
              newStyle = TASKWORKINGSTYLE.COPYINPUT;
            }
            break;
          case TASKWORKINGSTYLE.COPYOUTPUT:
            if (TASKWORKINGSTYLE.COPYINPUT == (task.WorkingStyleConstraint & TASKWORKINGSTYLE.COPYINPUT)) {
              newStyle = TASKWORKINGSTYLE.COPYINPUT;
            } else if (TASKWORKINGSTYLE.STRAIGHT == (task.WorkingStyleConstraint & TASKWORKINGSTYLE.STRAIGHT)) {
              newStyle = TASKWORKINGSTYLE.STRAIGHT;
            }

            break;
          case TASKWORKINGSTYLE.COPYINPUT:
            if (TASKWORKINGSTYLE.STRAIGHT == (task.WorkingStyleConstraint & TASKWORKINGSTYLE.STRAIGHT)) {
              newStyle = TASKWORKINGSTYLE.STRAIGHT;
            } else if (TASKWORKINGSTYLE.COPYOUTPUT == (task.WorkingStyleConstraint & TASKWORKINGSTYLE.COPYOUTPUT)) {
              newStyle = TASKWORKINGSTYLE.COPYOUTPUT;
            }
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
        task.WorkingStyle = newStyle;
        this.setButtonStyle(task);
      }
    }

    private void setWorkingStyle()
    {
      // get bound task
      ITask task = this.DataContext as ITask;
      if (task != null) {
        // cycle button around
        this.setButtonStyle(task);
      }
    }

    private void setButtonStyle(ITask task)
    {
      switch (task.WorkingStyle) {
        case TASKWORKINGSTYLE.STRAIGHT:
          this.straight.Visibility = Visibility.Visible;
          this.copyoutput.Visibility = Visibility.Hidden;
          this.copyinput.Visibility = Visibility.Hidden;
          this.last.Visibility = Visibility.Hidden;
          this.flowButton.ToolTip = "Eingangsbild wird verändert und weitergegeben. Erstellt keine Kopie.";
          break;
        case TASKWORKINGSTYLE.COPYOUTPUT:
          this.straight.Visibility = Visibility.Hidden;
          this.copyoutput.Visibility = Visibility.Visible;
          this.copyinput.Visibility = Visibility.Hidden;
          this.last.Visibility = Visibility.Hidden;
          this.flowButton.ToolTip = "Eingangsbild wird weitergegeben. Verändertes Bild wird gespeichert.";
          break;
        case TASKWORKINGSTYLE.COPYINPUT:
          this.straight.Visibility = Visibility.Hidden;
          this.copyoutput.Visibility = Visibility.Hidden;
          this.copyinput.Visibility = Visibility.Visible;
          this.last.Visibility = Visibility.Hidden;
          this.flowButton.ToolTip = "Eingangsbild wird gespeichert. Verändertes Bild wird weitergegeben";
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