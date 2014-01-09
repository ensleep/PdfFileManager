using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace pdf文件管理器
{
    public partial class AddFile : Form
    {
        public Form1 parentForm;
        public AddFile(Form1 form)
        {
            InitializeComponent();
            this.parentForm = form;
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;

                textBox2.Text = openFileDialog1.SafeFileName.Substring(0, openFileDialog1.SafeFileName.LastIndexOf('.'));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if(textBox2.Text.Trim()=="")
                {
                    throw new Exception("文件名不能为空");
                }
                string filename=textBox2.Text.Trim();
                if(parentForm.listFile.Exists(m=>m.Name==filename))
                {
                    throw new Exception("文件名已存在");

                }
                if(textBox1.Text.Trim()=="")
                {
                    throw new Exception("请选择文件");
                }
                if(!File.Exists(textBox1.Text.Trim()))
                {
                    throw new Exception("选择的文件不存在，请重新选择");

                }
                string filePath = DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".pdf";
                File.Copy(textBox1.Text.Trim(), @"File\" + filePath);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("File.xml");
                XmlNode filesNode = xmlDoc.SelectNodes("//Files")[0];
                XmlElement fileNode = xmlDoc.CreateElement("File");
                fileNode.SetAttribute("FilePath", filePath);
                fileNode.SetAttribute("AddDate", DateTime.Now.ToString("yyyy-MM-dd,hh:mm:ss"));
                fileNode.SetAttribute("FileName", filename);
                filesNode.AppendChild(fileNode);
                xmlDoc.Save("File.xml");
                parentForm.loadGate();
                this.Close();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
