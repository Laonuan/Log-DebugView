﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Configuration;

namespace DebugView
{
    public partial class Form1 : Form
    {
        long bytcount = 0;
        bool bClosed = false;
        int iScreenW;
        int iScreenH;

        public Form1()
        {
            InitializeComponent();

            Rectangle ScreenArea = System.Windows.Forms.Screen.GetBounds(this);
            iScreenW = ScreenArea.Width;
            iScreenH = ScreenArea.Height;
            this.Width = iScreenW / 2;
            this.Height = iScreenH;

            buttonSize.Tag = true;
            buttonSide.Tag = true;           
            checkBoxAutoScroll.Checked = true;

            string programName = ConfigurationManager.AppSettings["programName"];
            if (File.Exists(programName))
            {
                Process.Start(programName);
            }

            textBoxFilePath.Text = System.Environment.CurrentDirectory;
            string defaultPath = ConfigurationManager.AppSettings["defaultPath"];
            if(Directory.Exists(defaultPath) || File.Exists(defaultPath))
            {
                textBoxFilePath.Text = defaultPath;
            }

            bool startPositonRight = ConfigurationManager.AppSettings["startPositonRight"] == "True" ? true : false;
            if (startPositonRight)
                btnSide_Click(this, new EventArgs());

            bool stratPostionDown = ConfigurationManager.AppSettings["startPositonDown"] == "True" ? true : false;
            if (stratPostionDown)
                btnDown_Click(this, new EventArgs());
        }

        public void LoadLog()
        {
            string filePath = textBoxFilePath.Text;
            FileStream file = null;
            StreamReader reader = null;
            try
            {
                file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                reader = new StreamReader(file, Encoding.UTF8);
                long dataLengthToRead = file.Length;
                byte[] bytcontent = new byte[file.Length];

                if (dataLengthToRead > 0 && dataLengthToRead > bytcount)
                {
                    file.Seek(bytcount, SeekOrigin.Begin);
                    int lengthRead = file.Read(bytcontent, 0, Convert.ToInt32(dataLengthToRead - bytcount));
                    string log = Encoding.UTF8.GetString(bytcontent);
                    richTextBoxLog.AppendText(log);
                    if(checkBoxAutoScroll.Checked)
                    {
                        richTextBoxLog.SelectionStart = richTextBoxLog.TextLength;
                        richTextBoxLog.ScrollToCaret(); 
                    }
                    dataLengthToRead -= lengthRead;
                }
                bytcount = bytcontent.Length;
            }
            catch(Exception e)
            {
                if(timer1.Enabled)buttonAutoReflash_Click(this,null);
                MessageBox.Show(e.Message);
            }
            finally
            {
                if(file != null)file.Close();
                if(reader != null)reader.Close();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            LoadLog();
        }

        private void buttonReflash_Click(object sender, EventArgs e)
        {
            LoadLog();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if ((bool)buttonSize.Tag)
                buttonSize_Click(sender, e);
            this.Location = new Point(this.Location.X, iScreenH / 2);
        }

        private void buttonSize_Click(object sender, EventArgs e)
        {
            
            if((bool)buttonSize.Tag)
            {
                this.Height = iScreenH/2;
                buttonSize.Tag = false;
                buttonSize.Text = "Increase";
            }
            else
            {
                this.Height = iScreenH;
                this.Location = new Point(this.Location.X, 0);
                buttonSize.Tag = true;
                buttonSize.Text = "Discrease";
            }
        }

        private void buttonAutoReflash_Click(object sender, EventArgs e)
        {
            if(!timer1.Enabled)
            {
                buttonAutoReflash.Text = "Stop Refresh";
                timer1.Enabled = true;
            }
            else
            {
                buttonAutoReflash.Text = "Auto Refresh";
                timer1.Enabled = false;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            bClosed = true;
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            this.Opacity = 1;
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            if (!bClosed)
                this.Opacity = 0.65;
        }

        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            if(Directory.Exists(textBoxFilePath.Text) || File.Exists(textBoxFilePath.Text))
            {
                openFileDlg.InitialDirectory =textBoxFilePath.Text;
            }
            else
            {
                openFileDlg.InitialDirectory = System.Environment.CurrentDirectory;
            }      
            openFileDlg.Filter = "所有文件(*.*)|*.*";

           if(openFileDlg.ShowDialog() == DialogResult.OK)
           {
               textBoxFilePath.Text = openFileDlg.FileName;
               richTextBoxLog.Text = "";
               bytcount = 0;
           }
        }

        private void btnSide_Click(object sender, EventArgs e)
        {
            if((bool)buttonSide.Tag)
            {
                this.Location = new Point(iScreenW / 2, this.Location.Y);
                buttonSide.Tag = false;
                buttonSide.Text = "Left Side";
            }
            else
            {
                this.Location = new Point(0, this.Location.Y);
                buttonSide.Tag = true;
                buttonSide.Text = "Right Side";
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            textBoxFilePath.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            buttonAutoReflash_Click(null, null);
        }

    }
}
