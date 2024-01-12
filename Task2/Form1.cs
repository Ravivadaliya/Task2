using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using File = System.IO.File;

namespace Task2
{
    public partial class Form1 : Form
    {
        private static int Total_Files = 0;
        private static int Total_Folder = 1;
        private static string[] stringArray = {} ;
    
        BackgroundWorker worker = new BackgroundWorker();
       
        public Form1()
        {
            InitializeComponent();
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += Worker_ProgressChange;
            worker.DoWork += Worker_DoWork;
        }

        private void Worker_ProgressChange(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            label3.Text = progressBar1.Value.ToString()+"%";
            if(progressBar1.Value == 100)
            {
                label3.Text = progressBar1.Value.ToString();
                MessageBox.Show($"File copy completed with {Total_Files} Files and  {Total_Folder} Folders");
            }
        }
        private void Worker_DoWork(object sender,DoWorkEventArgs e)
        {
            button3.Invoke((MethodInvoker)delegate
            {
                button3.Enabled = false;
            });
            ClearButton.Invoke((MethodInvoker)delegate
            {
                ClearButton.Enabled = false;
            });
            button3.Enabled = false;
            CopyMethod(textsource.Text, textdestination.Text);
            button3.Invoke((MethodInvoker)delegate
            {
                button3.Enabled = true;
            });
            ClearButton.Invoke((MethodInvoker)delegate
            {
                ClearButton.Enabled = true;
            });
        }

        //submit button click
        private void button1_Click(object sender, EventArgs e)
        {
            worker.RunWorkerAsync();
        }

        static bool IsFilePath(string path)
        {
            // Check if the path is a valid file path and the file exists
            return !string.IsNullOrEmpty(path) && File.Exists(path);
        }
        private void CopyMethod(string target, string destination)
        {
            if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(destination))
            {   
                MessageBox.Show("Please select source and destination folders.");
                return;
            }

            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }
            // Get all files in the source folder
            if (string.IsNullOrEmpty(target))
            {
                return;
            }
            if (stringArray.Length > 0)
            {
                int totallength = 0;
                int filelength = 0;
                int folderlenght = 0;

                foreach (string item in stringArray)
                {
                    if (IsFilePath(item))
                    {
                        filelength++;
                    }
                    else
                    {
                        folderlenght++;
                    }
                }
                totallength = filelength + folderlenght;
                int processedItems = 0;
                DateTime startTime = DateTime.Now;
                foreach (string item in stringArray)
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        continue;
                    }
                    if (File.Exists(item))
                    {
                        processedItems++;
                        Total_Files++;
                        string fileName = Path.GetFileName(item);
                        string destinationFilePath = Path.Combine(destination, fileName);
                        File.Copy(item, destinationFilePath, true); // true overwrite existing files
                        int progressPercentage = (processedItems * 100) / totallength;
                        worker.ReportProgress(progressPercentage);
                    }
                    else
                    {
                        processedItems++;
                        Total_Folder++;
                        string directoryName = Path.GetFileName(item);
                        string destinationDirectoryPath = Path.Combine(destination, directoryName);
                        CopyDirectory(item, destinationDirectoryPath);
                        int progressPercentage = (processedItems * 100) / totallength;
                        worker.ReportProgress(progressPercentage);
                    }
                }
                DateTime endTime = DateTime.Now;
                TimeSpan duration = endTime - startTime;
                TimeLabel.Invoke((MethodInvoker)delegate
                {
                    TimeLabel.Text = duration.TotalSeconds.ToString() + "seconds";
                });
                progressBar1.Invoke((MethodInvoker)delegate
                {
                    progressBar1.Value = 100;
                });
                label3.Invoke((MethodInvoker)delegate
                {
                    label3.Text = "100%";
                });
            }
            else
            {
                string[] files = Directory.GetFiles(target);
                string[] directories = Directory.GetDirectories(target);

                int totalItems = files.Length + directories.Length;
                int processedItems = 0;

                DateTime startTime = DateTime.Now;
                foreach (string file in files)
                {
                    processedItems++;
                    Total_Files++;
                    string fileName = Path.GetFileName(file);
                    string destinationFilePath = Path.Combine(destination, fileName);
                    File.Copy(file, destinationFilePath, true); // true overwrite existing files

                    int progressPercentage = (processedItems * 100) / totalItems;
                    worker.ReportProgress(progressPercentage);
                }
                // subdirectories
                foreach (string directory in directories)
                {
                    processedItems++;
                    Total_Folder++;
                    string directoryName = Path.GetFileName(directory);
                    string destinationDirectoryPath = Path.Combine(destination, directoryName);
                    CopyDirectory(directory, destinationDirectoryPath);
                    int progressPercentage = (processedItems * 100) / totalItems;
                    worker.ReportProgress(progressPercentage);
                }
                DateTime endTime = DateTime.Now;
                TimeSpan duration = endTime - startTime;
                TimeLabel.Invoke((MethodInvoker)delegate
                {
                    TimeLabel.Text = duration.TotalSeconds.ToString() + "seconds";
                });
            }
        }

        static void CopyDirectory(string source, string destination)
        {
            // Create the destination directory if not exiest
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            string[] files = Directory.GetFiles(source);
            string[] directories = Directory.GetDirectories(source);

            // Copy files
            foreach (string file in files)
            {
                Total_Files++;
                string fileName = Path.GetFileName(file);
                string destinationFilePath = Path.Combine(destination, fileName);
                File.Copy(file, destinationFilePath, true);  // true overwrite existing filess
            }

            // recursion subdirectories
            foreach (string directory in directories)
            {
                Total_Folder++;
                string directoryName = Path.GetFileName(directory);
                string destinationDirectoryPath = Path.Combine(destination, directoryName);
                CopyDirectory(directory, destinationDirectoryPath);
            }
        }

        #region unnecessary methods
        private void label1_Click(object sender, EventArgs e)
        {
        }
        private void label2_Click(object sender, EventArgs e)
        {
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }
        private void progressBar1_Click(object sender, EventArgs e)
        {
        }
        #endregion


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string selectedPath = textsource.Text;

            // Check if the selected path is a folder
            if (Directory.Exists(selectedPath))
            {
                string[] filesAndFolders = Directory.GetFileSystemEntries(selectedPath);

                checkedListBox1.Items.Clear();
                foreach (string path in filesAndFolders)
                {
                    checkedListBox1.Items.Add(path);
                }
            }
            else
            {
                MessageBox.Show("Please select a valid folder path.");
            }   
        }
        //when you select all file then this method called
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, checkBox1.Checked);
            }
            checkedListBox1.Enabled = !checkBox1.Checked;
            textBox4.Text = "";
            stringArray = null;
            foreach (object item in checkedListBox1.CheckedItems)
            {
                string checkedItemValue = item.ToString();
                textBox4.Text += checkedItemValue + Environment.NewLine;
            }

            stringArray = textBox4.Lines;
        }

        #region inpute output select
        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            //OpenFileDialog op = new OpenFileDialog();
            //if (op.ShowDialog() == DialogResult.OK) {
            //    textsource.Text = op.FileName;
            //}
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                textsource.Text = folderDialog.SelectedPath;
            }
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            FolderBrowserDialog fdb = new FolderBrowserDialog();
            if (fdb.ShowDialog() == DialogResult.OK)
            {
                textdestination.Text= Path.Combine(fdb.SelectedPath,"");
            }
        }
        #endregion

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox4.Text = "";
            stringArray = null;
            foreach (object item in checkedListBox1.CheckedItems)
            {
                string checkedItemValue = item.ToString();
                textBox4.Text += checkedItemValue + Environment.NewLine;
            }
            stringArray = textBox4.Lines;
        }



        private void ClearButton_Click(object sender, EventArgs e)
        {

            textBox4.Text = "";
            checkBox1.Checked = false;
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, false);
            }
            progressBar1.Value = 0;
            label3.Text = "";
            TimeLabel.Text = "";

        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
        }
        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
        }

    }
}











//private void textBox1_TextChanged_1(object sender, EventArgs e)
//{

//}

//private void button1_Click_1(object sender, EventArgs e)
//{
//    if (string.IsNullOrEmpty(textsource.Text))
//    {
//        MessageBox.Show("Select source path");
//        return;
//    }
//    // Get all files and directories in the source folder
//    string[] filesAndDirectories = Directory.GetFileSystemEntries(textsource.Text);

//    // Concatenate the names into a single string
//    string result = string.Join(Environment.NewLine, filesAndDirectories);

//    // Set the result to the TextBox
//    textBox3.Text = result;
//}




//// Add Button method
//private void button1_Click_1(object sender, EventArgs e)
//{
//    if (string.IsNullOrEmpty(textsource.Text))
//    {
//        MessageBox.Show("Select source path");
//        return;
//    }

//    string selectedPath = textsource.Text;

//    // Check if the selected path is a folder
//    if (Directory.Exists(selectedPath))
//    {
//        string[] filesAndFolders = Directory.GetFileSystemEntries(selectedPath);

//        checkedListBox1.Items.Clear();
//        foreach (string path in filesAndFolders)
//        {
//            checkedListBox1.Items.Add(path);
//        }
//    }
//    else
//    {
//        MessageBox.Show("Please select a valid folder path.");
//    }
//}




////convert textbox4 and create string array for file transfer (transfer button)
//private void checkeditem_Click(object sender, EventArgs e)
//{
//    textBox4.Text = "";

//    foreach (object item in checkedListBox1.CheckedItems)
//    {
//        string checkedItemValue = item.ToString();
//        textBox4.Text += checkedItemValue + Environment.NewLine;
//    }
//    stringArray = textBox4.Lines;
//}

//simple method to copy file and folder