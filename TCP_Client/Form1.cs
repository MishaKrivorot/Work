using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TCP_Client
{
    public partial class Form1 : Form
    {
        private const int Port = 44000;
        private const string ServerAddress = "192.168.214.129";
        public Form1()
        {
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filter = textBox1.Text;
            if (string.IsNullOrEmpty(filter))
            {
                MessageBox.Show("Введіть фільтр пошуку.");
                return;
            }

            XmlDocument requestXml = new XmlDocument();
            XmlElement root = requestXml.CreateElement("Request-1");
            root.InnerText = filter;
            requestXml.AppendChild(root);

            SaveXml(requestXml, "Request-1.XML");

            string response = SendRequestToServer(requestXml.OuterXml);
            if (!string.IsNullOrEmpty(response))
            {
                XmlDocument responseXml = new XmlDocument();
                responseXml.LoadXml(response);
                SaveXml(responseXml, "Response-1.XML");
                listBox1.Items.Clear();

                // Get the Lines node
                XmlNode linesNode = responseXml.DocumentElement.SelectSingleNode("Lines");
                if (linesNode != null)
                {
                    foreach (XmlNode lineNode in linesNode.ChildNodes)
                    {
                        listBox1.Items.Add(lineNode.InnerText);
                    }
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Оберіть прізвище зі списку.");
                return;
            }

            string selectedLastName = listBox1.SelectedItem.ToString();

            XmlDocument requestXml = new XmlDocument();
            XmlElement root = requestXml.CreateElement("Request-2");
            root.InnerText = selectedLastName;
            requestXml.AppendChild(root);

            SaveXml(requestXml, "Request-2.XML");

            string response = SendRequestToServer(requestXml.OuterXml);
            if (!string.IsNullOrEmpty(response))
            {
                XmlDocument responseXml = new XmlDocument();
                responseXml.LoadXml(response);
                SaveXml(responseXml, "Response-2.XML");
                XmlNode linesNode = responseXml.SelectSingleNode("/Response-2/Lines");
                string result = string.Empty;

                if (linesNode != null)
                {
                    foreach (XmlNode lineNode in linesNode.ChildNodes)
                    {
                        result += lineNode.InnerText + Environment.NewLine;
                    }
                }
                MessageBox.Show(result.Trim(), "Повне прізвище та ім'я");
            }
        }

        private string SendRequestToServer(string requestXml)
        {
            try
            {
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ServerAddress), Port);
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                byte[] requestBytes = Encoding.GetEncoding(1251).GetBytes(requestXml);
                byte[] responseBytes = new byte[10000];

                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    socket.Connect(ipEndPoint);
                    socket.Send(requestBytes);

                    // Receive the response from the server
                    int bytesReceived = socket.Receive(responseBytes);
                    string response = Encoding.GetEncoding(1251).GetString(responseBytes, 0, bytesReceived);
                    socket.Shutdown(SocketShutdown.Both);
                    return response;
                }
            }
            catch (SocketException sockEx)
            {
                MessageBox.Show("Помилка мережі: " + sockEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Загальна помилка: " + ex.Message);
            }

            return null;
        }

        private void SaveXml(XmlDocument xmlDoc, string fileName)
        {
            if (xmlDoc == null)
            {
                MessageBox.Show("Received null XML document; unable to save.");
                return;
            }

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            xmlDoc.Save(path);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}

