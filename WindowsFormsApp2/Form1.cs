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
                label5.Text = ("GPU: " + obj["Name"]);
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
            if(label4.Text=="Mining")
            {
                if (Process.GetProcessesByName("claymore").Length > 0 && Process.GetProcessesByName("claymore") != null)
                {
                    Process proc = Process.GetProcessesByName("claymore")[0];
                    proc.Kill();
                    button1.BackgroundImage = WindowsFormsApp2.Properties.Resources.Spinner_1s_200px;
                }
            }
            else
            {

                if (label15.Text == "NiceHash Mode")
                {
                    if (textBox1.TextLength == 0)
                    {
                        MessageBox.Show("Please add wallet address! (go to options)");
                    }
                    else
                    {
                        //NiceHash mode
                        Process process = new Process();
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.CreateNoWindow = true;
                        startInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "claymore.exe";
                        startInfo.Arguments = "-epool daggerhashimoto.eu.nicehash.com:3353 -ewal " + textBox1.Text.Trim()+ ".Ghostnicehash" + " -li " + trackBar1.Value + " -epsw x -dbg -1 -retrydelay 1 -ftime 55 -tt 79 -ttli 77 -tstop 89 -tstart 85 -fanmin 30 -r 0 -esm 3 -erate 1 -allcoins 1 -allpools 1";
                        process.StartInfo = startInfo;
                        process.Start();
                        SetParent(process.MainWindowHandle, this.Handle);
                    }
                    

                }
                else if(label15.Text == "Direct Mining Mode")
                {
                    if (textBox2.TextLength == 0)
                    {
                        MessageBox.Show("Please add wallet address! (go to options)");
                    }
                    if (textBox3.TextLength == 0)
                    {
                        MessageBox.Show("Please add pool address! (go to options)");
                    }
                    else
                    {
                        //Direct mining mode
                        Process process = new Process();
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.CreateNoWindow = true;
                        startInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "claymore.exe";
                        startInfo.Arguments = "-epool " + textBox3.Text.Trim() + " -ewal " + textBox2.Text.Trim() + " -eworker afhMiner -li " + trackBar1.Value + " -epsw x -dbg -1 -retrydelay 1 -ftime 55 -tt 79 -ttli 77 -tstop 89 -tstart 85 -fanmin 30 -r 0 -esm 3 -erate 1 -allcoins 1 -allpools 1";
                        process.StartInfo = startInfo;
                        process.Start();
                        SetParent(process.MainWindowHandle, this.Handle);
                    }

                }

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
                        //reading the hashrate from localhost
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


                    timer2.Enabled = true;
                    label4.ForeColor = Color.Green;
                    label4.Text = "Mining";

                    button1.BackgroundImage = WindowsFormsApp2.Properties.Resources.mining;
                }

            }
            else
            {
                timer2.Enabled = false;
                label4.ForeColor = Color.Red; 
                label4.Text = "Closed";
                button1.BackgroundImage = WindowsFormsApp2.Properties.Resources.start;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "3F1zYbiCB5JKR21GiFgFwzxZoyJzTsk3U2";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label15.Text = "NiceHash Mode";
            label15.ForeColor = Color.FromArgb(251, 195, 66);
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (panel2.Visible == true)
            {
                label15.Text = "NiceHash Mode";
                label15.ForeColor = Color.FromArgb(251, 195, 66);
            }
            else if (panel3.Visible == true)
            {
                label15.Text = "Direct Mining Mode";
                label15.ForeColor = Color.FromArgb(0, 192, 192);
            }

            if (panel1.Visible==false)
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
            if(this.BackColor==Color.FromArgb(226, 226, 226))
            {
                this.BackColor = Color.FromArgb(58, 57, 57);
                button4.Text = "Light Mode";
            }
            else if(this.BackColor == Color.FromArgb(58, 57, 57))
            {
                this.BackColor = Color.FromArgb(226, 226, 226);
                button4.Text = "Dark Mode";
            }
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (panel3.Visible == true)
            {
                label15.Text = "NiceHash Mode";
                label15.ForeColor = Color.FromArgb(251, 195, 66);
            }
            else if (panel2.Visible == true)
            {
                label15.Text = "Direct Mining Mode";
                label15.ForeColor = Color.FromArgb(0, 192, 192);
            }

            if (panel2.Visible == true)
            {
                panel3.Visible = true;
                panel2.Visible = false;
            }
            else if (panel2.Visible == false)
            {
                panel3.Visible = false;
                panel2.Visible = true;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox3.Text = "eu1.ethermine.org:4444";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox2.Text = "0x94b57107c9163c507355C3e7755d3eb6f4306a89";
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        public int showMsgBox = 0;

        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                //getting current BTC price
                string btcJson;
    
                using (var web = new System.Net.WebClient())
                {
                    var url = @"https://api.coindesk.com/v1/bpi/currentprice.json";
                    btcJson = web.DownloadString(url);
                }

                string intPrice = btcJson.Split(',')[9].Trim(new char[] { ' ', '"', ':', 'r', 'a', 't', 'e' }) + btcJson.Split(',')[10].Trim(new char[] { ' ', '"', ':', 'r', 'a', 't', 'e' });

                decimal currentPrice = Decimal.Parse(intPrice);


                //getting current profitability from nicehash
                WebClient Client = new WebClient();
                var receiveBytesProfit = new byte[256];
                Client.DownloadFile("https://api2.nicehash.com/main/api/v2/mining/external/" + textBox1.Text.Trim() + "/rigs/stats/unpaid/", AppDomain.CurrentDomain.BaseDirectory + "nicehash.json");
                var json = Encoding.ASCII.GetString(receiveBytesProfit);

                //show current profitability
                string profitability = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "nicehash.json").Truncate(200);
                
                decimal dec = Decimal.Parse(profitability.Split(',')[10], System.Globalization.NumberStyles.Any);
                decimal hashrate = Decimal.Parse(label3.Text.Replace("MH/s", ""), System.Globalization.NumberStyles.Any);

                label1.Text = (dec * currentPrice).ToString().Truncate(4) + "USD/day";

                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "nicehash.json");
            }
            catch(Exception)
            {
                showMsgBox = showMsgBox + 1;
                if(showMsgBox == 1)
                {
                    MessageBox.Show("Profitability is not working! Be sure that you are using NiceHash address.");
                }
            }
            

        }
    }

    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}
