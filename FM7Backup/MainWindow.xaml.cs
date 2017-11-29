using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Exception = System.Exception;

namespace FM7Backup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Worker = new BackgroundWorker();
            Worker.DoWork += RunBackup;
            Worker.RunWorkerCompleted += CompleteBackup;

            Initialize();
        }

        private AppSettings Settings { get; set; }

        private BackgroundWorker Worker { get; set; }

        // Yeah I know, quick and dirty code in the view class

        private void Initialize()
        {
            Settings = SettingsDao.LoadSettings("fm7backupsettings.json");

            // Load was a failure, auto generate settings
            if (Settings == null)
            {
                Settings = new AppSettings();

                try
                {
                    Settings.SaveFileLocation =
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Packages\\Microsoft.ApolloBaseGame_8wekyb3d8bbwe\\SystemAppData";

                    Settings.BackupLocation = Environment.CurrentDirectory + "\\FM7Backups";
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            // Update view
            SaveLocationBox.Text = Settings.SaveFileLocation;
            BackupLocationBox.Text = Settings.BackupLocation;

            if (Settings.DateType == DateType.Readable)
            {
                NamingHumanButton.IsChecked = true;
            }
            else
            {
                NamingUnixButton.IsChecked = true;
            }

            return;
        }

        private void RunBackup(Object sender, DoWorkEventArgs e)
        {
            BackupArgs args = (BackupArgs) e.Argument;
            String status = "";
            bool success = false;

            try
            {
                // Copy dir (thanks https://stackoverflow.com/a/3822913)
                foreach (string dirPath in Directory.GetDirectories(args.SaveLocation, "*",
                    SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(args.SaveLocation, args.BackupLocation));

                foreach (string newPath in Directory.GetFiles(args.SaveLocation, "*.*",
                    SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(args.SaveLocation, args.BackupLocation), true);

                success = true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);

                status = "Error: " + exception.Message;
            }

            if (success)
            {
                status = "Backup complete.";
            }

            e.Result = status;

            return;
        }

        private void CompleteBackup(object sender, RunWorkerCompletedEventArgs e)
        {
            StatusText.Content = e.Result;

            BackupButton.IsEnabled = true;

            return;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.SaveFileLocation = SaveLocationBox.Text;
            Settings.BackupLocation = BackupLocationBox.Text;

            SettingsDao.SaveSettings("fm7backupsettings.json", Settings);
        }

        private void NamingHumanButton_Checked(object sender, RoutedEventArgs e)
        {
            Settings.DateType = DateType.Readable;
        }

        private void NamingUnixButton_Checked(object sender, RoutedEventArgs e)
        {
            Settings.DateType = DateType.Unix;
        }

        private void BackupButton_Click(object sender, RoutedEventArgs e)
        {
            // Don't run if worker is already busy
            if (Worker.IsBusy)
                return;

            String backupLocation = BackupLocationBox.Text + "\\";

            StatusText.Content = "Running backup...";

            if (Settings.DateType == DateType.Readable)
            {
                backupLocation += DateTime.Now.ToString("yyyy-M-d THH-mm-ss");
            }
            else
            {
                backupLocation += DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }

            BackupButton.IsEnabled = false;

            BackupArgs workerArgs = new BackupArgs(SaveLocationBox.Text, backupLocation);

            Worker.RunWorkerAsync(workerArgs);
        }

        private void SaveLocationBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    SaveLocationBox.Text = dialog.SelectedPath;
                }
            }
        }

        private void BackupLocationBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    BackupLocationBox.Text = dialog.SelectedPath;
                }
            }
        }
    }
}
