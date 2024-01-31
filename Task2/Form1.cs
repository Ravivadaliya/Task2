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
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using Task2.DAL;

namespace Task2
{
    public partial class Form1 : Form
    {

        //declaretion require variable
        private static int Total_Files = 0;
        private static int Total_Folder = 0;
        private static string[] stringArray = { };
        string connectionString = "";


    
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
            label3.Text = progressBar1.Value.ToString() + "%";
            if (progressBar1.Value == 100)
            {
                label3.Text = progressBar1.Value.ToString();
                MessageBox.Show($"File copy completed with {Total_Files} Files and  {Total_Folder} Folders");
            }
        }
        
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
               label5.Text = "Copying....." ;
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
            ClearButton_Click(sender, e);
            worker.RunWorkerAsync();
            //CopyMethod(textsource.Text, textdestination.Text);
        }




        //Copy file folder method
        private void CopyMethod(string target, string destination)
        {

            FolderFileDatabase folderFileDatabase = new FolderFileDatabase();

            if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(destination))
            {
                MessageBox.Show("Please select source and destination folders.");
                return;
            }

            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }


            if (stringArray.Length > 0)
            {
                int totallength = 0;
                int filelength = 0;
                int folderlenght = 0;

                string mainfolder = Path.GetFileName(destination);

                int totalfile = 0;
                int totalfolder = 0;
                string parentpath = Path.GetDirectoryName(destination);
                string parent = Path.GetFileName(parentpath);


                foreach (string item in stringArray)
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        continue;
                    }
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

                folderFileDatabase.FolderDataEntry(mainfolder, filelength, folderlenght, parent, destination, 5,connectionString);
               
                foreach (string item in stringArray)
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        continue;
                    }
                    parentpath = Path.GetDirectoryName(destination);
                    parent = Path.GetFileName(parentpath);

                    if (File.Exists(item))
                    {
                        processedItems++;
                        Total_Files++;
                        totalfile++;
                        string fileName = Path.GetFileName(item);
                        string destinationFilePath = Path.Combine(destination, fileName);


                        string fileexe = Path.GetExtension(item);
                        int status = 1;
                        folderFileDatabase.FileDataentry(mainfolder, fileName, fileexe, status,connectionString);
                        try
                        {
                            File.Copy(item, destinationFilePath, true); // true overwrite existing files
                            folderFileDatabase.FileUpdate(fileName, 5,connectionString);
                        }
                        catch (Exception e)
                        {
                            folderFileDatabase.FileUpdate(fileName, 3, connectionString);
                            Logger.Writelog("Copy File Error"+e.Message);
                            //MessageBox.Show("Error in copy file " + e.Message);
                        }
                        int progressPercentage = (processedItems * 100) / totallength;
                        worker.ReportProgress(progressPercentage);
                    }
                    else
                    {
                        processedItems++;
                        Total_Folder++;
                        totalfolder++;
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
                string mainfolder = Path.GetFileName(destination);
                string[] files = Directory.GetFiles(target);
                string[] directories = Directory.GetDirectories(target);
                int totalItems = files.Length + directories.Length;
                int processedItems = 0;

                string parentpath = Path.GetDirectoryName(destination);
                string parent = Path.GetFileName(parentpath);
                DateTime startTime = DateTime.Now;

                folderFileDatabase.FolderDataEntry(mainfolder, files.Length, directories.Length, parent, destination, 5,connectionString);

                foreach (string file in files)
                {

                    processedItems++;
                    Total_Files++;
                    string fileName = Path.GetFileName(file);
                    string destinationFilePath = Path.Combine(destination, fileName);

                    string fileexe = Path.GetExtension(file);
                    int status = 1;
                    folderFileDatabase.FileDataentry(mainfolder, fileName, fileexe, status, connectionString);

                    try
                    {
                        File.Copy(file, destinationFilePath, true); // true overwrite existing files
                        folderFileDatabase.FileUpdate(fileName, 5, connectionString);
                    }
                    catch (Exception e)
                    {
                        folderFileDatabase.FileUpdate(fileName, 3, connectionString);
                        //MessageBox.Show("Error in copy file " + e.Message);
                        Logger.Writelog("Error in File Copy"+e.Message);
                    }

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



        //Recursion file folder copy
        public void CopyDirectory(string source, string destination)
        {
            int total_file = 0;
            int total_folder = 0;
            FolderFileDatabase folderFileDatabase = new FolderFileDatabase();
            string mainfolder = Path.GetFileName(destination);


            // Create the destination directory if not exiest
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }
            string[] files = Directory.GetFiles(source);
            string[] directories = Directory.GetDirectories(source);

            string parentpath = Path.GetDirectoryName(destination);
            string parent = Path.GetFileName(parentpath);


            folderFileDatabase.FolderDataEntry(mainfolder, files.Length, directories.Length, parent, destination, 5, connectionString);


            // Copy files
            foreach (string file in files)
            {
                total_file++;
                Total_Files++;
                string fileName = Path.GetFileName(file);
                string destinationFilePath = Path.Combine(destination, fileName);

                string fileexe = Path.GetExtension(file);
                int status = 1;

                folderFileDatabase.FileDataentry(mainfolder, fileName, fileexe, status,connectionString);
                try
                {
                    File.Copy(file, destinationFilePath, true);  // true overwrite existing filess
                    folderFileDatabase.FileUpdate(fileName, 5,connectionString);
                }
                catch (Exception e)
                {
                    folderFileDatabase.FileUpdate(fileName,3,connectionString);
                    Logger.Writelog(e.Message);
                    //MessageBox.Show("Error in file copy" + e.Message);
                }
            }

            if (directories.Length > 0)
            {
                foreach (string directory in directories)
                {
                    total_folder++;
                    Total_Folder++;
                    string directoryName = Path.GetFileName(directory);
                    string destinationDirectoryPath = Path.Combine(destination, directoryName);
                    CopyDirectory(directory, destinationDirectoryPath);
                }
            }
        }





        //All method of all element of form (submit button in above)

        //On Form load connection string establish
        private void Form1_Load(object sender, EventArgs e)
        {
            //database code
            connectionString = "Data Source=MSI\\SQLEXPRESS;Initial Catalog=Infinnium;Integrated Security=true;";
        }


        //fill check box on change source path
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
        
        //source select form pc 
        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                textsource.Text = folderDialog.SelectedPath;
            }
        }

        //destination select from pc
        private void button2_Click_1(object sender, EventArgs e)
        {
            FolderBrowserDialog fdb = new FolderBrowserDialog();
            if (fdb.ShowDialog() == DialogResult.OK)
            {
                textdestination.Text = Path.Combine(fdb.SelectedPath, "");
            }
        }
        #endregion


        //When you select checkbox then this method call
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


        //clear button method
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
            Total_Files = 0;
            Total_Folder = 0;

        }


        //check method file or not 
        static bool IsFilePath(string path)
        {
            // Check if the path is a valid file path and the file exists
            return !string.IsNullOrEmpty(path) && File.Exists(path);
        }



        #region unnecessary methods
        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
        }
        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
        }
        private void label1_Click(object sender, EventArgs e)
        {
        }
        private void label2_Click(object sender, EventArgs e)
        {
        }
        private void label3_Click(object sender, EventArgs e)
        {
        }
        private void progressBar1_Click(object sender, EventArgs e)
        {
        }
        #endregion

    }
}


