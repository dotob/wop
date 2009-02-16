using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using WOP.Objects;

namespace WOP.Tasks {
    public enum TASKPOS {
        FIRST,
        MIDDLE,
        LAST
    }

    public class TaskEventArgs : EventArgs {
        public ITask Task;
        public IWorkItem WorkItem;

        public TaskEventArgs(ITask task, IWorkItem wi)
        {
            Task = task;
            WorkItem = wi;
        }
    }

    public interface ITask:INotifyPropertyChanged {
        Queue<IWorkItem> WorkItems { get; }
        ITask NextTask { get; set; }
        string Name { get; set; }
        UserControl UI { get; set; }
        bool IsEnabled { get; set; }
        Job Parent { get; set; }
        TASKPOS Position { get; set; }
        Dictionary<ITask, string> TaskInfos { get; set; }
        event EventHandler<TaskEventArgs> WIProcessed;
        void Start();
        void Pause();
    }
}