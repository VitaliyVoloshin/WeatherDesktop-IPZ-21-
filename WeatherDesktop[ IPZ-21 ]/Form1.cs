﻿using System;
using System.Drawing;
using System.Windows.Forms;

using WeatherDesktop__IPZ_21__.Properties;

namespace WeatherDesktop__IPZ_21__
{

    public partial class MainForm : Form
    {
        Point PanelMouseDownLocation;
        Timer timer = new Timer();

        public MainForm()
        {
            InitializeComponent();
            UpPanel.BackColor = Color.FromArgb(150, UpPanel.BackColor);  // Setting transparency of panel
            comboBox1.SelectedIndex = 0;
            label2_copy.Visible = false;
            InfoUpdate();
        }

        public void InfoUpdate() {
            if (comboBox1.SelectedIndex == 0) {
                new AccuWeather(this);
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                pictureBox1.Image = Resources.logo_AW;
            }
            else if (comboBox1.SelectedIndex == 1) {
                new OpenWeather(this);
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                pictureBox1.Image = Resources.logo_OW;
            }
        }

        public void ChangeWeatherData(string weather, string status) {
            labelWeatherData.Text = weather;
            Status.Text = status;
        }

        public void WeatherIcon(string image_icon)
        {
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.Image = (Bitmap)Resources.ResourceManager.GetObject(image_icon);
        }

        public void Warning()
        {
            MessageBox.Show("Cache file is empty or not found","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            Status.Text = "Updated " + DateTime.Now.ToString("HH:mm:ss");
            labelWeatherData.Text = string.Empty;
            pictureBox3.Image = null;
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
            label2.Visible = false;
            label2_copy.Visible = true;
            timer.Interval = 3000;
            timer.Start();
            timer.Tick += delegate { label2.Visible = true; label2_copy.Visible = false; timer.Stop(); };
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            InfoUpdate();
        }
    }



}
