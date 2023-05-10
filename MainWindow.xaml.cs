using Microsoft.Extensions.Configuration;
using System;
using System.Windows;
using FileUtility.Model;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace FileUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        private bool IsLogWrite = false;
        IConfiguration _configuration;
        FileDetails fileDetails = new FileDetails();
        string Name = "Nimesh";
        private AppSetting appSetting {
            get{
                return new AppSetting();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppConfig");
            fileDetails.Name = _configuration.GetSection("Name").Value;
            fileDetails.DestinationPath = _configuration.GetSection("DestinationPath").Value;
            fileDetails.SourcePath = _configuration.GetSection("SourcePath").Value;
            NameTextBox.Text = fileDetails.Name;
            DestinationPathTextBox.Text = fileDetails.DestinationPath + "\\" + DateTime.Now.ToString("yyyyMMdd-") + fileDetails.Name + "-";
            SourcePathTextBox.Text = fileDetails.SourcePath;
            IsLogWrite = Boolean.Parse(string.IsNullOrEmpty(_configuration.GetSection("IsLogWrite").Value) ? "False" : _configuration.GetSection("IsLogWrite").Value);
            this.DataContext = this;
        }
        private void NameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(NameTextBox.Text))
            {
                fileDetails.Name = NameTextBox.Text;
                appSetting.AddOrUpdateAppSetting("Name", fileDetails.Name);
                DestinationPathTextBox.Text = fileDetails.DestinationPath + "\\" + DateTime.Now.ToString("yyyyMMdd-") + fileDetails.Name + "-";
            }
        }

        private void selectfilebutton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Multiselect = true;
            openFileDialog1.Title = "Select Photos";
            Nullable<bool> result = openFileDialog1.ShowDialog();
            if (result == true)
            {
                try
                {
                    List<string> oldFileList = (List<string>)FilelistView.ItemsSource;
                    List<string> FileList = new List<string>();

                    if (oldFileList != null)
                        FileList = oldFileList.ToList();

                    DestinationPathTextBox.Text = fileDetails.DestinationPath + "\\" + DateTime.Now.ToString("yyyyMMdd-") + Name + "-" + Path.GetFileNameWithoutExtension(openFileDialog1.FileNames.First());

                    foreach (String file in openFileDialog1.FileNames)
                    {
                        FileList.Add(file);
                    }
                    FilelistView.ItemsSource = FileList;
                }
                catch (Exception ex)
                {
                    //if (IsLogWrite)
                    //    logWriter.LogWrite(ex.Message, LogWriteMode.Error);
                    MessageBox.Show("Error: " + ex.Message);
                }
                
            }
        }

        private void FilelistView_Drop(object sender, DragEventArgs e)
        {
            List<string> oldFileList = (List<string>)FilelistView.ItemsSource;
            List<string> FileList = new List<string>();

            if (oldFileList != null)
                FileList = oldFileList.ToList();
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[]; // get all files droppeds  
            if (files != null && files.Any())
            {
                foreach (String file in files)
                {
                    FileList.Add(file);
                }
                FilelistView.ItemsSource= FileList;
            }
        }

        private void DestinationPathTextBox_Drop(object sender, DragEventArgs e)
        {
            List<string> oldFileList = (List<string>)FilelistView.ItemsSource;
            List<string> FileList = new List<string>();

            if (oldFileList != null)
                FileList = oldFileList.ToList();

            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[]; // get all files droppeds  
            if (files != null && files.Any())
            {
                DestinationPathTextBox.Text =fileDetails.DestinationPath + "\\" + DateTime.Now.ToString("yyyyMMdd-") + Name + "-" + Path.GetFileNameWithoutExtension(files.First());
                foreach (String file in files)
                {
                    FileList.Add(file);
                }
                FilelistView.ItemsSource= FileList;
            }
        }
        private void DestinationPathTextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void NavigationBarPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Window window = this as Window;
            if (window != null)
            {
                window.DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = this as Window;
            if (window != null)
            {
                window.WindowState = WindowState.Minimized;

            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = this as Window;
            if (window != null)
            {
                window.Close();
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            DestinationPathTextBox.Text = fileDetails.DestinationPath + "\\" + DateTime.Now.ToString("yyyyMMdd-") + Name + "-";
            FilelistView.ItemsSource = null;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> Files = (List<string>)FilelistView.ItemsSource;
                if (Files != null && Files.Count > 0)
                {
                   foreach (string file in Files)
                    {
                        string destfile = file.Replace(fileDetails.SourcePath, DestinationPathTextBox.Text);
                        System.IO.FileInfo destfileinfo = new System.IO.FileInfo(destfile);
                        destfileinfo.Directory.Create(); // If the directory already exists, this method does nothing.
                        File.Copy(file, destfile, true);
                    }

                    MessageBox.Show(this, "Copy SucsessFully...", "SucsessFully");
                }
                else
                {
                    MessageBox.Show("Select File First...");
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
