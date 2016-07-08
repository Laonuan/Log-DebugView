using System;
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

        public Form1()
        {
            InitializeComponent();
            this.Width = 640;
            this.Height = 1024;
            buttonSize.Tag = true;
            textBoxFilePath.Text = System.Environment.CurrentDirectory;
            checkBoxAutoScroll.Checked = true;
            string programName =  ConfigurationManager.AppSettings["programName"];
            if (File.Exists(programName))
            {
                Process.Start(programName);
            }
        }

        public void LoadLog()
        {
            //DateTime time = DateTime.Now;
            //string filePath = ".\\LOG\\LOG" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            string filePath = textBoxFilePath.Text;
            FileStream file = null;
            StreamReader reader = null;
            try
            {
                File.Copy(filePath, ".\\tmp.txt", true);
                file = new FileStream(".\\tmp.txt", FileMode.Open, FileAccess.Read);
                reader = new StreamReader(file, Encoding.Default);
                long dataLengthToRead = file.Length;
                byte[] bytcontent = new byte[file.Length];
                if (dataLengthToRead > 0 && dataLengthToRead > bytcount)
                {
                    file.Seek(bytcount, SeekOrigin.Begin);
                    int lengthRead = file.Read(bytcontent, 0, Convert.ToInt32(dataLengthToRead - bytcount));
                    string log = System.Text.Encoding.Default.GetString(bytcontent);
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

        private void buttonSize_Click(object sender, EventArgs e)
        {
            
            if((bool)buttonSize.Tag)
            {
                this.Height = 512;
                buttonSize.Tag = false;
                buttonSize.Text = "窗口变大";
            }
            else
            {
                this.Height = 1024;
                buttonSize.Tag = true;
                buttonSize.Text = "窗口缩小";
            }

            
        }

        private void buttonAutoReflash_Click(object sender, EventArgs e)
        {
            if(!timer1.Enabled)
            {
                buttonAutoReflash.Text = "停止刷新";
                timer1.Enabled = true;
            }
            else
            {
                buttonAutoReflash.Text = "自动刷新";
                timer1.Enabled = false;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(File.Exists(".\\tmp.txt"))
            {
                File.Delete(".\\tmp.txt");
            }
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
            openFileDlg.InitialDirectory = System.Environment.CurrentDirectory;
            openFileDlg.Filter = "所有文件(*.*)|*.*";
            openFileDlg.ShowDialog();
            
            if (openFileDlg.FileName != "")
            {
                textBoxFilePath.Text = openFileDlg.FileName;
                //richTextBoxLog.Text = "";
                //bytcount = 0;
            }
        }

        private void textBoxFilePath_TextChanged(object sender, EventArgs e)
        {
            richTextBoxLog.Text = "";
            bytcount = 0;
        }

    }
}
