using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Collections;

namespace pdf文件管理器
{
    public partial class Form1 : Form
    {
        public List<FileInfo> listFile = new List<FileInfo>();
        public Form1()
        {
            InitializeComponent();
            if(!Directory.Exists("File"))
            {
                Directory.CreateDirectory("File");
                File.Delete("File.xml");
            }
            if(!File.Exists("File.xml"))
            {
                XmlDocument xmlDoc = new XmlDocument();
                // Write down the XML declaration
                XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                // Create the root element
                XmlElement rootNode = xmlDoc.CreateElement("FileXml");
                xmlDoc.InsertBefore(xmlDeclaration, xmlDoc.DocumentElement);
                xmlDoc.AppendChild(rootNode);
                // Create a new <Category> element and add it to the root node
                XmlElement parentNode = xmlDoc.CreateElement("Category");
                // Set attribute name and value!
                parentNode.SetAttribute("CreateDate", DateTime.Now.ToString("yyyy-MM-dd,hh:mm:ss"));
                xmlDoc.DocumentElement.PrependChild(parentNode);
                // Create the required nodes
                XmlElement filesNode = xmlDoc.CreateElement("Files");
                rootNode.AppendChild(filesNode);
                // Save to the XML file
                xmlDoc.Save("File.xml");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //axPdfShow.LoadFile("readMe.txt");
            loadGate();
        }
        public void loadGate()
        {
            listFile = new List<FileInfo>();
            listBox1.Items.Clear();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("File.xml");
            //xmlDoc.SelectNodes("descendant::File[author/last-name='Austen']")
            foreach (XmlNode xn in xmlDoc.SelectNodes("descendant::File"))
            {
                FileInfo fi = new FileInfo();
                fi.Name = xn.Attributes["FileName"].Value;
                fi.Path = xn.Attributes["FilePath"].Value;
                DateTime dt=new DateTime();
                if(!DateTime.TryParse(xn.Attributes["AddDate"].Value,out dt))
                {
                    fi.AddDate=null;
                }else{
                    fi.AddDate=dt;
                }
                listBox1.Items.Add(fi.Name);
                listFile.Add(fi);
            }
            listBox2.Items.Clear();
            string wantfile = textBox1.Text.Trim();
            foreach (var item in listFile.Where(m => m.Name.Contains(wantfile)))
            {
                listBox2.Items.Add(item.Name);
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if(listBox1.SelectedItems.Count>0)
            {
                string fileName = listBox1.SelectedItems[0].ToString();
                FileInfo fi=listFile.Where(m => m.Name == fileName).FirstOrDefault();
                axPdfShow.LoadFile(@"File\" + fi.Path);

            }
        }

        private void 添加文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFile af = new AddFile(this);
            af.Show();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            string wantfile = textBox1.Text.Trim();
            foreach(var item in listFile.Where(m => m.Name.Contains(wantfile)))
            {
                listBox2.Items.Add(item.Name);
            }
        }

        private void listBox2_DoubleClick(object sender, EventArgs e)
        {
            if (listBox2.SelectedItems.Count > 0)
            {
                string fileName = listBox2.SelectedItems[0].ToString();
                FileInfo fi = listFile.Where(m => m.Name == fileName).FirstOrDefault();
                axPdfShow.LoadFile(@"File\" + fi.Path);

            }
        }

        private void listBox1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)
            {
                string fileName = listBox1.SelectedItems[0].ToString();
                FileInfo fi = listFile.Where(m => m.Name == fileName).FirstOrDefault();
                label1.Text = "文件ID:"+fi.Path+"     "+"添加时间:"+fi.AddDate;

            }

        }

        private void listBox2_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedItems.Count > 0)
            {
                string fileName = listBox2.SelectedItems[0].ToString();
                FileInfo fi = listFile.Where(m => m.Name == fileName).FirstOrDefault();
                label1.Text = "文件ID:" + fi.Path + "     " + "添加时间:" + fi.AddDate;

            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Insert)
            {
                AddFile af = new AddFile(this);
                af.Show();
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(tabControl1.SelectedIndex==0)
            {
                if(listBox1.SelectedItems.Count>0)
                {
                    delXmlFile(listBox1.SelectedItems[0].ToString());
                }
            }
            else
            {
                if (listBox2.SelectedItems.Count > 0)
                {
                    delXmlFile(listBox2.SelectedItems[0].ToString());
                }
            }
            loadGate();
        }

        private void delXmlFile(string p)
        {
            try { 
                string filepath = listFile.Where(m => m.Name == p).FirstOrDefault().Path;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("File.xml");
                //XmlNode fileNode = xmlDoc.SelectNodes("//Files/File/@FilePath='" + filepath + "'")[0];
                XmlNode filesNode = xmlDoc.SelectNodes("//Files")[0];

                filesNode.RemoveChild(filesNode.SelectNodes("//File[@FilePath='" + filepath + "']")[0]);
                File.Delete(@"File\" + filepath);
                xmlDoc.Save("File.xml");
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void 导出所有文件到文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            if(fbd.ShowDialog()==DialogResult.OK)
            {
                try
                {
                    foreach(var m in listFile)
                    {
                        File.Copy(@"File\"+m.Path,fbd.SelectedPath+@"\"+m.Name+".pdf",true);
                    }
                    MessageBox.Show("导出成功！");
                    System.Diagnostics.Process.Start("explorer.exe", fbd.SelectedPath);
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
                finally
                {

                }
            }
        }

        private void 打开文件目录ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpAbout ha = new HelpAbout();
            ha.Show();
        }
    }
    public class FileInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime? AddDate { get; set; }
    }
}
