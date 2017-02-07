using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RadioScheduler.Models;

namespace RadioScheduler.Services
{
    public class SchedulingOptions
    {
        public string SchedulingFile { get; set; }
    }

    public class SchedulingService
    {
        private readonly Stack<MonitoringTask> _tasks;
        private readonly Dictionary<string, MonitoringTask> _assignedTasks;

        public SchedulingService(IOptions<SchedulingOptions> options)
        {
            List<MonitoringTask> taskList;
            try
            {
                var json = File.ReadAllText(options.Value.SchedulingFile);
                taskList = JsonConvert.DeserializeObject<List<MonitoringTask>>(json);
            }
            catch (Exception)
            {
                taskList = new List<MonitoringTask>();
            }
            _tasks = new Stack<MonitoringTask>(taskList);
            _assignedTasks = new Dictionary<string, MonitoringTask>();
        }

        public MonitoringTask GetTask()
        {
            return  _tasks.Count > 0 ? _tasks.Pop() : MonitoringTask.DefaultTask;
        }

        public void AssignTask(MonitoringTask task, string ip)
        {
            if(task == MonitoringTask.DefaultTask)
                return;

            if (_assignedTasks.ContainsKey(ip))
            {
                var previousTask = _assignedTasks[ip];
                _tasks.Push(previousTask);
            }

            _assignedTasks[ip] = task;
        }
    }
}