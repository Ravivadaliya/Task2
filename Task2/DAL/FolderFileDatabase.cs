using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Task2.DAL
{
    internal class FolderFileDatabase
    {


        //database method
        private SqlConnection OpenConnection(string connectionString)
        {
            try
            {
               SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    MessageBox.Show("Error in establishing connection");
                    return null;
                }
                return connection;
            }
            catch (Exception e)
            {
                //MessageBox.Show("Error occurred while opening the connection: " + e.Message);
                Logger.Writelog("Open Connection "+e.Message);
                return null;
            }
        }



        //Folder DataEntry Method
        public void FolderDataEntry(string Folder_Name, int File_Count, int Folder_count, string Parent_Folder, string Orignal_Location, int Watch_Status, string connectionString)
        {
            SqlConnection connection = OpenConnection(connectionString);
            try
            {

                string Foldersqlquery = "INSERT INTO FolderDetails(Folder_Name,File_Count,Folder_Count,Parent_Folder,Orignal_Location,Watch_Status) VALUES(@V1,@V2,@V3,@V4,@V5,@V6)";
                using (SqlCommand command = new SqlCommand(Foldersqlquery, connection))
                {
                    command.Parameters.AddWithValue("@V1", Folder_Name);
                    command.Parameters.AddWithValue("@V2", File_Count);
                    command.Parameters.AddWithValue("@V3", Folder_count);
                    command.Parameters.AddWithValue("@V4", Parent_Folder);
                    command.Parameters.AddWithValue("@V5", Orignal_Location);
                    command.Parameters.AddWithValue("@V6", Watch_Status);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == -1)
                    {
                        MessageBox.Show("Error occure in insert folder data");
                    }
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show("Error occur in Folder Data entry : " + e.Message);
                Logger.Writelog("Open Connection " + e.Message);
            }

        }

        //File dataEntry Method
        public void FileDataentry(string foldername, string File_Names, string File_Extention, int File_Status, string connectionString)
        {
            SqlConnection connection = OpenConnection(connectionString);

            try
            {
                string Folderidsqlquery = "SELECT ID FROM FolderDetails WHERE Folder_Name = @FolderName";

                using (SqlCommand cd = new SqlCommand(Folderidsqlquery, connection))
                {
                    cd.Parameters.AddWithValue("@FolderName", foldername);
                    int Folder_Id = (int)(cd.ExecuteScalar() ?? 0);

                    string FilesqlQuery = "INSERT INTO FileDetails(Folder_Id, File_Names, File_Extention, File_Status) VALUES(@V1, @V2, @V3, @V4)";

                    using (SqlCommand command = new SqlCommand(FilesqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@V1", Folder_Id);
                        command.Parameters.AddWithValue("@V2", File_Names);
                        command.Parameters.AddWithValue("@V3", File_Extention);
                        command.Parameters.AddWithValue("@V4", File_Status);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == -1)
                        {
                            MessageBox.Show("Error occurred while inserting file data");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Writelog("File Entry error " + e.Message);
            }
        }

        //File status Update Method
        public void FileUpdate(string File_Names, int File_Status, string connectionString)
        {
            SqlConnection connection = OpenConnection(connectionString);
            try
            {
                string selectfileid = "SELECT ID FROM FILEDETAILS WHERE File_Names = @File_Names";

                using (SqlCommand cd = new SqlCommand(selectfileid, connection))
                {
                    cd.Parameters.AddWithValue("File_Names", File_Names);
                    int ID = (int)(cd.ExecuteScalar() ?? 0);

                    string filestatusupdate = "UPDATE FileDetails SET File_Status = @status where ID = @ID";
                    using (SqlCommand command = new SqlCommand(filestatusupdate, connection))
                    {
                        command.Parameters.AddWithValue("@status", File_Status);
                        command.Parameters.AddWithValue("@ID", ID);
                        int rowupdate = command.ExecuteNonQuery();
                        if (rowupdate == -1)
                        {
                            MessageBox.Show("Error occurred while inserting file data");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show("An error occurred in File Status Upadte : " + e.Message);
                Logger.Writelog("FileUpdate error " + e.Message);
            }
        }

    }
}
