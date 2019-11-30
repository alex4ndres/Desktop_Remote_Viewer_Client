﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.Serialization.Formatters.Binary;

namespace Viewer_Client
{
    public partial class FrmClient : Form
    {
        private readonly TcpClient client = new TcpClient();
        private NetworkStream mainStream;
        private int portNumber;
        

        private static Image GrabDesktop()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap screenshot = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(screenshot);
            graphics.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);

            return screenshot;
        }

        private void SendDesktopImage()
        {
            BinaryFormatter binFormatter = new BinaryFormatter();
            mainStream = client.GetStream();
            binFormatter.Serialize(mainStream, GrabDesktop());
        }

        public FrmClient()
        {
            InitializeComponent();
        }

        private void FrmClient_Load(object sender, EventArgs e)
        {
            timer1.Interval = 1;
            btnShare.Enabled = false;
            labelStatus.Text = "Not Connected";
            labelStatus.ForeColor = Color.Red;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if(txtPort.Text.ToString().Trim() == "")
            {
                MessageBox.Show("Fill the field 'Port'", "Warning", MessageBoxButtons.OK,MessageBoxIcon.Error);
                txtPort.Focus();
            }
            //if (txtIP.Text.ToString().Trim() == "")
            //{
            //    MessageBox.Show("Fill the field 'IP'", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    txtIP.Focus();
            //}
            else
            {
                portNumber = int.Parse(txtPort.Text);
                try
                {
                    client.Connect(txtIP.Text, portNumber);
                    btnConnect.Text = "Connected";
                    labelStatus.Text = "Connected";
                    labelStatus.ForeColor = Color.GreenYellow;
                    btnConnect.Enabled = false;
                    btnShare.Enabled = true;
                }
                catch (Exception)
                {
                    MessageBox.Show("Connection Failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnConnect.Text = "Connect";
                    labelStatus.Text = "Not Connected";
                    labelStatus.ForeColor = Color.Red;
                }
            }
            
        }

        private void btnShare_Click(object sender, EventArgs e)
        {
            if (btnShare.Text.StartsWith("Share"))
            {
                timer1.Start();
                btnShare.Text = "Stop Sharing";
            }
            else
            {
                timer1.Stop();
                btnShare.Text = "Share Screen";
                btnConnect.Enabled = true;
                labelStatus.Text = "Not Connected";
                labelStatus.ForeColor = Color.Red;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            SendDesktopImage();
        }
    }
}
