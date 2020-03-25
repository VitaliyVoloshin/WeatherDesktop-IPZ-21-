using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Resources;


namespace WeatherDesktop__IPZ_21__
{

    public partial class MainForm : Form
    {
        Point PanelMouseDownLocation;
        
        public MainForm()
        {
            InitializeComponent();
            UpPanel.BackColor = Color.FromArgb(150, UpPanel.BackColor);  // Setting transparency of panel
            comboBox1.SelectedIndex = 0;
            InfoUpdate();
        }
        public void InfoUpdate() {
            if (comboBox1.SelectedIndex == 0) {
                new AccuWeather(this);
            }
            else if (comboBox1.SelectedIndex == 1) {
                new OpenWeather(this);
            }
        }
        public void ChangeWeatherData(string weather, string status) {
            labelWeatherData.Text = weather;
            Status.Text = status;
        }
        private void UpPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) PanelMouseDownLocation = e.Location;
        }
        private void UpPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - PanelMouseDownLocation.X;
                this.Top += e.Y - PanelMouseDownLocation.Y;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void label2_Click(object sender, EventArgs e)
        {
            InfoUpdate();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            InfoUpdate();
        }
    }



}
