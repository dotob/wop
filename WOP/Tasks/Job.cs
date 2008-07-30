using System.Collections.Generic;
using WOP.Objects;

namespace WOP.Tasks {
    public class Job {
        public string Name { get; set; }
        public int Progress { get; set; }
        public List<ITask> TasksList { get; set; }
        public List<IWorkItem> WorkItems { get; set; }

        public void Start()
        {
            if (TasksList != null) {
                for (int i = TasksList.Count - 1; i >= 0; i--) {
                    TasksList[i].Start();
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
            if (TasksList == null) {
                TasksList = new List<ITask>();
                t.Position = TASKPOS.FIRST;
                t.WIProcessed += task_WIProcessed;
            }
            TasksList.Add(t);

            // connect tasks
            if (TasksList.Count > 1) {
                TasksList[TasksList.Count - 1].NextTask = t;
            }

            // tell them their position
            int i = 0;
            foreach (ITask task in TasksList) {
                task.Position = i == TasksList.Count - 1 ? TASKPOS.LAST : TASKPOS.MIDDLE;
                i++;
            }
        }

        public static Job CreateTestJob()
        {
            var j = new Job();
            var fgt = new FileGatherTask();
            fgt.SourceDirectory = @"..\..\..\IM\pix";
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

        private static void task_WIProcessed(object sender, TaskEventArgs e)
        {
            // listen for first task (usually the filegatherer). so we learn all wi from him
            e.Task.Parent.WorkItems.Add(e.WorkItem);
        }
    }
}