using System;
using System.Collections.Generic;
using System.Windows.Input;
using WOP.Objects;

namespace WOP.Tasks {
    public class Job {
        public static readonly RoutedCommand StartJobCommand = new RoutedCommand("StartJobCommand", typeof(Job));
        public static readonly RoutedCommand PauseJobCommand = new RoutedCommand("PauseJobCommand", typeof(Job));

        public event EventHandler JobFinished;
        public string Name { get; set; }
        public int Progress { get; set; }
        public List<ITask> TasksList { get; set; }
        public List<IWorkItem> WorkItems { get; set; }
        public bool IsProcessing { get; set; }
        public bool IsFinished { get; set; }
        public bool IsPaused { get; set; }

        public void Start()
        {
            if (TasksList != null) {
                for (int i = TasksList.Count - 1; i >= 0; i--) {
                    if (TasksList[i].IsEnabled) {
                        TasksList[i].Start();
                    }
                }
            }
        }

        public void Pause()
        {
            if (TasksList != null) {
                foreach (ITask task in TasksList) {
                    task.Pause();
                }
            }
        }

        public void AddTask(ITask t)
        {
            t.Parent = this;

            if (TasksList == null) {
                WorkItems = new List<IWorkItem>();
                TasksList = new List<ITask>();
                t.Position = TASKPOS.FIRST;
            }
            TasksList.Add(t);

            // connect tasks
            if (TasksList.Count > 1) {
                TasksList[TasksList.Count - 2].NextTask = t;
            }

            // tell them their position
            int i = 0;
            foreach (ITask task in TasksList) {
                if (i > 0) {
                    task.Position = i == TasksList.Count - 1 ? TASKPOS.LAST : TASKPOS.MIDDLE;
                }
                i++;
            }

            // listen to last task
            i = 0;
            foreach (ITask task in TasksList) {
                if (i < TasksList.Count - 1) {
                    task.WIProcessed -= task_WIProcessed;
                } else {
                    task.WIProcessed += task_WIProcessed;
                }
                i++;
            }
        }

        public static Job CreateTestJob()
        {
            var j = new Job();
            var fgt = new FileGatherTask();
            fgt.SourceDirectory = @"..\..\..\IM\pixweniger";
            fgt.TargetDirectory = @"c:\tmp";
            fgt.SortOrder = FileGatherTask.SORTSTYLE.FILENAME;
            fgt.RecurseDirectories = true;
            fgt.FilePattern = "*jpg";
            j.AddTask(fgt);

            var frt = new FileRenamerTask();
            frt.RenamePattern = "bastiTest_{0}";
            j.AddTask(frt);

            j.AddTask(new ImageShrinkTask());

            return j;
        }

        private void task_WIProcessed(object sender, TaskEventArgs e)
        {
            // listen for first task (usually the filegatherer). so we learn all wi from him
            e.Task.Parent.WorkItems.Add(e.WorkItem);
            if(e.WorkItem is StopWI) {
                // job seems finished tell it
                EventHandler temp = this.JobFinished;
                if(temp!=null) {
                    temp(this, EventArgs.Empty);
                }
            }
        }
    }
}