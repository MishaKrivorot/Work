using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Management;
using System.Xml;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TCP_SERVER
{
    public partial class Service1 : ServiceBase
    {
        private Thread wmiThread;
        private Thread serverThread;
        private bool shouldStop;
        private ManagementEventWatcher _watcher;
        private ManualResetEvent stopEvent = new ManualResetEvent(false);
        private const string LogFilePath = @"C:\WORK\LAB4\Laba-4.log";

        public Service1()
        {
            InitializeComponent();
            this.CanStop = true;
            this.CanPauseAndContinue = false;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            // Inform the Service Control Manager that the service is starting
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000; // 10 seconds
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            WriteLog("Service is starting...");

            // Initialize stop flag and event
            shouldStop = false;
            stopEvent.Reset();

            // Start the threads for WMI monitoring and TCP server
            wmiThread = new Thread(StartProcessWatcher);
            wmiThread.IsBackground = true;
            wmiThread.Start();

            serverThread = new Thread(WorkerThread);
            serverThread.IsBackground = true;
            serverThread.Start();

            // Notify SCM that the service has started
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            WriteLog("Service started successfully.");
        }

        protected override void OnStop()
        {
            WriteLog("Service is stopping...");

            // Inform the SCM that we are stopping
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Signal threads to stop
            shouldStop = true;
            stopEvent.Set();

            // Stop the WMI watcher
            StopProcessWatcher();

            // Wait for threads to finish
            if (wmiThread != null && wmiThread.IsAlive)
            {
                wmiThread.Join(5000); // wait up to 5 seconds
            }
            if (serverThread != null && serverThread.IsAlive)
            {
                serverThread.Join(5000); // wait up to 5 seconds
            }

            // Notify SCM that the service has stopped
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            WriteLog("Service stopped successfully.");
        }

        private void StartProcessWatcher()
        {
            try
            {
                var query = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Process' AND TargetInstance.Name = 'notepad.exe'");
                _watcher = new ManagementEventWatcher(query);
                _watcher.EventArrived += new EventArrivedEventHandler(OnProcessDeleted);
                _watcher.Start();

                WriteLog("Monitoring process notepad.exe started.");
            }
            catch (Exception ex)
            {
                WriteLog($"Error starting process watcher: {ex.Message}");
            }
        }

        private void StopProcessWatcher()
        {
            try
            {
                if (_watcher != null)
                {
                    _watcher.Stop();
                    _watcher.Dispose();
                    _watcher = null;
                }
            }
            catch (Exception ex)
            {
                WriteLog($"Error stopping process watcher: {ex.Message}");
            }
        }

        private void OnProcessDeleted(object sender, EventArrivedEventArgs e)
        {
            WriteLog("Process notepad.exe has been terminated.");
        }

        private void WorkerThread()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 44000);
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                serverSocket.Bind(ipEndPoint);
                serverSocket.Listen(10);

                while (!shouldStop)
                {
                    try
                    {
                        using (Socket clientSocket = serverSocket.Accept())
                        {
                            byte[] requestBytes = new byte[10000];
                            int bytesReceived = clientSocket.Receive(requestBytes);
                            string requestXml = Encoding.GetEncoding(1251).GetString(requestBytes, 0, bytesReceived);
                            WriteLog(requestXml);

                            // Handle the request
                            string responseXml = HandleRequest(requestXml);
                            byte[] responseBytes = Encoding.GetEncoding(1251).GetBytes(responseXml);
                            clientSocket.Send(responseBytes);
                        }
                    }
                    catch (SocketException ex)
                    {
                        WriteLog($"Socket exception: {ex.Message}");
                        if (shouldStop) break;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog($"Error in WorkerThread: {ex.Message}");
            }
            finally
            {
                serverSocket.Close();
            }
        }

        private string HandleRequest(string requestXml)
        {
            ReadRegistryAndSaveToFile();
            XmlDocument requestDoc = new XmlDocument();
            requestDoc.LoadXml(requestXml);
            string filePath = "C:\\WORK\\LAB4\\TCP_SERVER\\bin\\Debug\\student.txt";
            string fileContent = File.ReadAllText(filePath, Encoding.GetEncoding(1251));
            XmlDocument responseDoc = new XmlDocument();

            if (requestDoc.DocumentElement.Name == "Request-1")
            {
                SaveXml(requestDoc, "Request-1.XML");
                XmlElement root = responseDoc.CreateElement("Response-1");
                XmlElement lines = responseDoc.CreateElement("Lines");
                string filter = requestDoc.SelectSingleNode("Request-1")?.InnerText;
                string[] fileLines = fileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                foreach (string line in fileLines)
                {
                    if (MatchesFilter(line.Split(' ')[0], filter))
                    {
                        XmlElement lineElement = responseDoc.CreateElement("Line");
                        lineElement.InnerText = line.Split(' ')[0];
                        lines.AppendChild(lineElement);
                    }
                }
                root.AppendChild(lines);
                responseDoc.AppendChild(root);
                SaveXml(responseDoc, "Response-1.XML");
            }
            else if (requestDoc.DocumentElement.Name == "Request-2")
            {
                SaveXml(requestDoc, "Request-2.XML");
                XmlElement root = responseDoc.CreateElement("Response-2");
                XmlElement lines = responseDoc.CreateElement("Lines");
                string lastname = requestDoc.SelectSingleNode("Request-2")?.InnerText;
                string[] fileLines = fileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                foreach (string line in fileLines)
                {
                    if (line.Split(' ')[0] == lastname)
                    {
                        XmlElement lineElement = responseDoc.CreateElement("Line");
                        lineElement.InnerText = line;
                        lines.AppendChild(lineElement);
                    }
                }
                root.AppendChild(lines);
                responseDoc.AppendChild(root);
                SaveXml(responseDoc, "Response-2.XML");
            }
            return responseDoc.OuterXml;
        }

        private bool MatchesFilter(string lastName, string filter)
        {
            if (string.IsNullOrEmpty(filter)) return false;
            if (filter == "*") return true;
            if (filter.StartsWith("*") && filter.EndsWith("*")) return lastName.Contains(filter.Trim('*'));
            if (filter.StartsWith("*")) return lastName.EndsWith(filter.TrimStart('*'));
            if (filter.EndsWith("*")) return lastName.StartsWith(filter.TrimEnd('*'));
            return lastName == filter;
        }

        private void ReadRegistryAndSaveToFile()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\SEARCH-Server"))
                {
                    if (key != null)
                    {
                        string[] valueNames = key.GetValueNames();
                        using (StreamWriter writer = new StreamWriter("C:\\WORK\\LAB4\\TCP_SERVER\\bin\\Debug\\student.txt", false, Encoding.GetEncoding(1251)))
                        {
                            foreach (string name in valueNames)
                            {
                                var value = key.GetValue(name);
                                writer.WriteLine(value != null ? $"{value}" : "null");
                            }
                        }
                        WriteLog("Registry data read successfully.");
                    }
                    else
                    {
                        WriteLog("Registry key not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog($"Registry read error: {ex.Message}");
            }
        }

        private void SaveXml(XmlDocument xmlDoc, string fileName)
        {
            if (xmlDoc != null)
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                xmlDoc.Save(path);
            }
        }

        private void WriteLog(string message)
        {
            using (StreamWriter writer = new StreamWriter(LogFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {message}");
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        }

        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
    }
}
