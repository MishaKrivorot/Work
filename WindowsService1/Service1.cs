using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Win32;

namespace WindowsService1
{
    public partial class Service1 : ServiceBase
    {
        private Thread claimCheckThread;
        private Thread taskExecutionThread1;
        private Thread taskExecutionThread2;
        private Thread taskExecutionThread3;
        private int taskExecutionDuration;
        private int taskClaimCheckPeriod;
        private int taskExecutionQuantity;
        private Queue<string> taskQueue = new Queue<string>();
        private bool stopThreads = false;


        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteLog("Service TaskQueue is STARTED");
            ReadRegistryParameters();

            // Запуск потоку для перевірки заявок
            claimCheckThread = new Thread(new ThreadStart(RunClaimCheck));
            claimCheckThread.Start();

            taskExecutionThread1 = new Thread(new ThreadStart(RunTaskExecution));
            taskExecutionThread1.Start();

            if (taskExecutionQuantity == 2)
            {
                taskExecutionThread2 = new Thread(new ThreadStart(RunTaskExecution));
                taskExecutionThread2.Start();
            }

            if (taskExecutionQuantity == 3)
            {
                taskExecutionThread2 = new Thread(new ThreadStart(RunTaskExecution));
                taskExecutionThread2.Start();
                taskExecutionThread3 = new Thread(new ThreadStart(RunTaskExecution));
                taskExecutionThread3.Start();
            }
        }

        protected override void OnStop()
        {
            WriteLog("Service TaskQueue is STOPPED");
            stopThreads = true;

            // Очікування завершення потоків
            if (claimCheckThread != null && claimCheckThread.IsAlive)
                claimCheckThread.Join();
            if (taskExecutionThread1 != null && taskExecutionThread1.IsAlive)
                taskExecutionThread1.Join();
            if (taskExecutionQuantity == 2)
            {
                if (taskExecutionThread2 != null && taskExecutionThread2.IsAlive)
                    taskExecutionThread2.Join();
            }
            if (taskExecutionQuantity == 3)
            {
                if (taskExecutionThread2 != null && taskExecutionThread2.IsAlive)
                    taskExecutionThread2.Join();

                if (taskExecutionThread3 != null && taskExecutionThread3.IsAlive)
                    taskExecutionThread3.Join();
            }

        }

        private void RunClaimCheck()
        {
            while (!stopThreads)
            {
                CheckClaims();
                Thread.Sleep(taskClaimCheckPeriod * 1000); // Періодичність перевірки
            }
        }

        private void RunTaskExecution()
        {
            while (!stopThreads)
            {
                ExecuteTasks();
                Thread.Sleep(2000); // Час між оновленням прогресу
            }
        }

        private void ReadRegistryParameters()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Task_Queue\Parameters"))
                {
                    // Читання Task_Execution_Duration
                    object executionDurationValue = key.GetValue("Task_Execution_Duration");
                    if (executionDurationValue != null && int.TryParse(executionDurationValue.ToString(), out int executionDuration))
                    {
                        taskExecutionDuration = Clamp(executionDuration, 30, 180);
                    }
                    else
                    {
                        taskExecutionDuration = 60; // дефолтне значення
                    }

                    // Читання Task_Claim_Check_Period
                    object claimCheckPeriodValue = key.GetValue("Task_Claim_Check_Period");
                    if (claimCheckPeriodValue != null && int.TryParse(claimCheckPeriodValue.ToString(), out int claimCheckPeriod))
                    {
                        taskClaimCheckPeriod = Clamp(claimCheckPeriod, 10, 45);
                    }
                    else
                    {
                        taskClaimCheckPeriod = 30; // дефолтне значення
                    }

                    // Читання Task_Execution_Quantity
                    object executionQuantityValue = key.GetValue("Task_Execution_Quantity");
                    if (executionQuantityValue != null && int.TryParse(executionQuantityValue.ToString(), out int executionQuantity))
                    {
                        taskExecutionQuantity = Clamp(executionQuantity, 1, 3);
                    }
                    else
                    {
                        taskExecutionQuantity = 1; // дефолтне значення
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("Error reading registry: " + ex.Message);
                taskExecutionDuration = 60;
                taskClaimCheckPeriod = 30;
                taskExecutionQuantity = 1;
            }
        }

        private void CheckClaims()
        {
                using (RegistryKey claimsKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Task_Queue\Claims"))
                {
                    if (claimsKey == null) return;

                    List<string> validClaims = new List<string>();
                    List<string> invalidClaims = new List<string>();
                    List<string> queueClaims = new List<string>();
                // Перевірка всіх заявок
                foreach (string claim in claimsKey.GetSubKeyNames())
                    {
                        if (Regex.IsMatch(claim, @"^Task_\d{4}$"))
                        {
                            validClaims.Add(claim); // Зберігаємо коректну заявку
                        }
                        else
                        {
                            invalidClaims.Add(claim); // Зберігаємо некоректну заявку
                        }
                    }

                    // Обробка некоректних заявок
                    foreach (var claim in invalidClaims)
                    {
                        WriteLog($"ERROR: Invalid claim syntax {claim}...");
                        Registry.LocalMachine.DeleteSubKey($@"SOFTWARE\Task_Queue\Claims\{claim}");
                    }
                foreach (var claim in validClaims)
                {
                    foreach (var claim1 in queueClaims)
                        if (claim == claim1)
                        {
                            validClaims.Remove(claim);
                            WriteLog($"ERROR: Task {claim} already exists...");
                            Registry.LocalMachine.DeleteSubKey($@"SOFTWARE\Task_Queue\Claims\{claim}");
                        }
                }
                // Якщо є коректні заявки, вибираємо одну з них з найменшим номером
                if (validClaims.Any())
                    {
                        string nextTask = validClaims.OrderBy(c => c).First();

                        // Додаємо в чергу у межах окремого блокування
                            taskQueue.Enqueue(nextTask);
                            WriteLog($"Task {nextTask} successfully queued...");

                            // Оновлюємо реєстр для черги і видаляємо із Claims
                            Registry.LocalMachine.DeleteSubKey($@"SOFTWARE\Task_Queue\Claims\{nextTask}");
                            Registry.LocalMachine.CreateSubKey($@"SOFTWARE\Task_Queue\{nextTask}-[....................]-Queued");
                        queueClaims.Add(nextTask);
                }
            }
        }

    private void ExecuteTasks()
        {
            while (taskQueue.Count > 0)
            {
                string currentTask = taskQueue.Dequeue();
                string taskPath = $@"SOFTWARE\Task_Queue\{currentTask}-[....................]-Queued";
                using (RegistryKey taskKey = Registry.LocalMachine.OpenSubKey(taskPath, true))
                {
                    if (taskKey == null)
                    {
                        WriteLog($"ERROR: Task {currentTask} does not exist in queue.");
                        continue;
                    }
                    int interval = 2;
                    int totalUpdates = taskExecutionDuration / interval;
                    // Оновлення статусу задачі на In Progress
                    Registry.LocalMachine.DeleteSubKey($@"SOFTWARE\Task_Queue\{currentTask}-[....................]-Queued");
                    for (int i = 1; i <= totalUpdates; i++) // Simulate 100% completion in steps of 5%
                    {
                        int percent = (i * 100) / totalUpdates;
                        string progress = new string('I', (i * 20) / totalUpdates) + new string('.', 20-(i * 20) / totalUpdates);
                        Registry.LocalMachine.CreateSubKey($@"SOFTWARE\Task_Queue\{currentTask}-[{progress}]-In progress - {percent}% completed");
                        Thread.Sleep(interval * 1000);
                        Registry.LocalMachine.DeleteSubKey($@"SOFTWARE\Task_Queue\{currentTask}-[{progress}]-In progress - {percent}% completed");
                    }

                    WriteLog($"Task {currentTask} successfully COMPLETED!");
                    Registry.LocalMachine.CreateSubKey($@"SOFTWARE\Task_Queue\{currentTask}-[IIIIIIIIIIIIIIIIIIII]-COMPLETED");
                }
            }
        }


        private int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private void WriteLog(string message)
        {
            string logFilePath = @"C:\Windows\Logs\TaskQueue_14-10-2024.log";
            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine($"-----------------------------------------{DateTime.Now}-----------------------------------------");
                sw.WriteLine(message);
            }
        }
    }
}
