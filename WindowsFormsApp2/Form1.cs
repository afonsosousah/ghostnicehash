using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Threading;
using System.Drawing.Configuration;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData,
        UIntPtr dwExtraInfo);

        public Form1()
        {
            InitializeComponent();

            //Download Claymore v15
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "claymore.exe") == false)
            {
                WebClient Client = new WebClient();
                Client.DownloadFile("https://cdn.discordapp.com/attachments/754055553470431346/758458942824120350/EthDcrMiner64.exe", AppDomain.CurrentDomain.BaseDirectory + "claymore.exe");
                Client.DownloadFile("https://cdn.discordapp.com/attachments/754055553470431346/758458933848834058/msvcr110.dll", AppDomain.CurrentDomain.BaseDirectory + "msvcr110.dll");
                Client.DownloadFile("https://cdn.discordapp.com/attachments/754055553470431346/758458940039102484/libcurl.dll", AppDomain.CurrentDomain.BaseDirectory + "libcurl.dll");
                Client.DownloadFile("https://cdn.discordapp.com/attachments/754055553470431346/758458937124192266/cudart64_80.dll", AppDomain.CurrentDomain.BaseDirectory + "cudart64_80.dll");
            }

            label4.ForeColor = Color.Red;
            label4.Text = "Closed";

            this.FormClosed += Form1_FormClosed;

            ManagementObjectSearcher objvide = new ManagementObjectSearcher("select * from Win32_VideoController");

            foreach (ManagementObject obj in objvide.Get())
            {
                label5.Text = ("Detected GPU: " + obj["Name"]);
            }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Process.GetProcessesByName("claymore").Length > 0 && Process.GetProcessesByName("claymore") != null)
            {
                Process proc = Process.GetProcessesByName("claymore")[0];
                proc.Kill();
            }

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.TextLength == 0)
            {
                MessageBox.Show("Please add wallet address! (go to options)");
            }
            else if(label4.Text=="Mining")
            {
                if (Process.GetProcessesByName("claymore").Length > 0 && Process.GetProcessesByName("claymore") != null)
                {
                    Process proc = Process.GetProcessesByName("claymore")[0];
                    proc.Kill();
                }
            }
            else
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.CreateNoWindow = true;
                startInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "claymore.exe";
                startInfo.Arguments = "-epool daggerhashimoto.eu.nicehash.com:3353 -ewal " + textBox1.Text.Trim() + " -eworker afhMiner -li " + trackBar1.Value + " -epsw x -dbg -1 -retrydelay 1 -ftime 55 -tt 79 -ttli 77 -tstop 89 -tstart 85 -fanmin 30 -r 0 -esm 3 -erate 1 -allcoins 1 -allpools 1";
                process.StartInfo = startInfo;
                process.Start();
                SetParent(process.MainWindowHandle, this.Handle);
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Process.GetProcessesByName("claymore").Length > 0 && Process.GetProcessesByName("claymore") != null)
            {
                
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    try
                    {
                        var receiveBytes = new byte[256];
                        socket.Connect("localhost", 3333);
                        socket.Send(Encoding.ASCII.GetBytes("{\"id\":0,\"jsonrpc\":\"2.0\",\"method\":\"miner_getstat1\"}"));
                        socket.Receive(receiveBytes, receiveBytes.Length, SocketFlags.None);
                        var responseJson = Encoding.ASCII.GetString(receiveBytes);
                        if (responseJson.Split(',')[4].Trim(new char[] { ' ', '"' }) != "" && responseJson.Split(',')[4].Trim(new char[] { ' ', '"' }) != " ")
                        {
                            int hashrate = int.Parse(responseJson.Split(',')[5].Trim(new char[] { ' ', '"' }));
                            label3.Text = String.Format("{0,0:N3}", hashrate / 1000.0) + "MH/s";
                        }
                        if (responseJson.Split(',')[5].Trim(new char[] { ' ', '"' }) != "" && responseJson.Split(',')[5].Trim(new char[] { ' ', '"' }) != " ")
                        {
                            int hashrate = int.Parse(responseJson.Split(',')[5].Trim(new char[] { ' ', '"' }));
                            label3.Text = String.Format("{0,0:N3}", hashrate / 1000.0) + "MH/s";
                        }   
                    }
                    catch (Exception)
                    {
                        
                    }
                        

                    label4.ForeColor = Color.Green;
                    label4.Text = "Mining";

                    button1.BackgroundImage = WindowsFormsApp2.Properties.Resources.mining;
                }

            }
            else
            {
                label4.ForeColor = Color.Red; 
                label4.Text = "Closed";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "3F1zYbiCB5JKR21GiFgFwzxZoyJzTsk3U2";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(panel1.Visible==false)
            {
                panel1.Visible = true;
            }
            else if(panel1.Visible==true)
            {
                panel1.Visible = false;
            }
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label9_Click_1(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(this.BackColor==SystemColors.ButtonFace)
            {
                this.BackColor = SystemColors.ControlDarkDark;
                button4.Text = "Light Mode";
            }
            else if(this.BackColor == SystemColors.ControlDarkDark)
            {
                this.BackColor = SystemColors.ButtonFace;
                button4.Text = "Dark Mode";
            }
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }
    }
}
