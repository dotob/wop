using System.Collections.Generic;
using WOP.Objects;

namespace WOP.Tasks {
    public class Job {
        public List<ITask> TasksList { get; set; }
        public List<IWorkItem> WorkItems { get; set; }

        public void Start()
        {
            //TODO: start all tasks
        }

        public void Pause()
        {
            //TODO: stop all tasks
        }

        public static Job CreateJob()
        {
            Job j = new Job();
            j.TasksList.Add(new FileGatherTask(j));
            j.TasksList.Add(new FileRenamerTask(j));
            j.TasksList.Add(new ImageShrinkTask(j));
            // inform tasks of their position
            int i = 0;
            foreach (ITask task in j.TasksList) {
                if (i == 0) {
                    task.Position = TASKPOS.FIRST;
                    task.WIProcessed += new System.EventHandler<TaskEventArgs>(task_WIProcessed);
                } else if (i == j.TasksList.Count - 1) {
                    task.Position = TASKPOS.LAST;
                } else {
                    task.Position = TASKPOS.MIDDLE;
                }
                i++;
            }
            return j;
        }

        static void task_WIProcessed(object sender, TaskEventArgs e)
        {
            // listen for first task (usually the filegatherer). so we learn all wi from him
            e.Task.Parent.WorkItems.Add(e.WorkItem);
        }
    }
}