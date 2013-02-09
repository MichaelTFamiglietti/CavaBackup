using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.IO.Compression;
using CavaStudios.Update;

namespace CavaBackup
{
    public partial class MainWindow : Window
    {
        Settings settings;
        UpdateSystem updateSystem;

        public MainWindow()
        {
            updateSystem = new UpdateSystem("0.1.0");
            settings = new Settings();
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            checkOpenFolder.IsChecked = settings.OpenBackupLocation;
            textBackupLocation.Text = settings.DefaultBackupLocation;
            checkCompress.IsChecked = settings.CompressBackups;
            textFolderToBackup.Text = settings.FolderToBackup;
            textNickname.Text = settings.Nickname;

            textVersion.Text = updateSystem.Version.MyVersion;

            buttonApply.IsEnabled = false;
            updateSystem.CheckUpdate("https://dl.dropbox.com/u/50007489/CavaStudios/VersionFiles/CavaBackupVersion.txt", "http://cavastudios.wikidot.com/filebackup");
        }

        #region Main
        private void buttonBackup_Click(object sender, RoutedEventArgs e)
        {
            settings.Save();
            buttonApply.IsEnabled = false;
            string backupLoc = textBackupLocation.Text + "/" + textNickname.Text;
            Backup(textFolderToBackup.Text, backupLoc);
            WriteLine("Backup of " + textFolderToBackup.Text + " complete!");
            if((bool)checkOpenFolder.IsChecked)
                Process.Start(backupLoc);
        }

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {

        }

        private void textFolderToBackup_TextChanged(object sender, TextChangedEventArgs e)
        {
            settings.FolderToBackup = textFolderToBackup.Text;
        }

        private void textNickname_TextChanged(object sender, TextChangedEventArgs e)
        {
            settings.Nickname = textNickname.Text;
        }
        #endregion

        #region Settings
        private void checkOpenFolder_Checked(object sender, RoutedEventArgs e)
        {
            settings.OpenBackupLocation = (bool)checkOpenFolder.IsChecked;
            buttonApply.IsEnabled = true;
        }

        private void textBackupLocation_TextChanged(object sender, TextChangedEventArgs e)
        {
            settings.DefaultBackupLocation = textBackupLocation.Text;
            buttonApply.IsEnabled = true;
        }

        private void buttonApply_Click(object sender, RoutedEventArgs e)
        {
            settings.Save();
            buttonApply.IsEnabled = false;
        }

        private void checkCompress_Checked(object sender, RoutedEventArgs e)
        {
            settings.CompressBackups = (bool)checkCompress.IsChecked;
            buttonApply.IsEnabled = true;
        }
        #endregion

        #region My Methods
        private void WriteLine(string toWrite)
        {
            Write(toWrite + "\n");
        }

        private void Write(string toWrite)
        {
            textConsole.Text += toWrite;
            textConsole.ScrollToEnd();
        }

        private void Backup(string pathOfFiles, string pathOfBackup)
        {
            string path1 = pathOfFiles.Replace("/", "\\");
            string path2 = pathOfBackup.Replace("/", "\\");
            if (!Directory.Exists(path2))
                Directory.CreateDirectory(path2);
            foreach (string file in Directory.GetFiles(path1))
            {
                string scndLoc = path2 + file.Replace(path1, "");
                if (File.Exists(scndLoc))
                    File.Delete(scndLoc);
                File.Copy(file, scndLoc);
            }
            foreach (string dir in Directory.GetDirectories(path1))
                Backup(dir, path2 + dir.Replace(path1, ""));
            if ((bool)checkCompress.IsChecked)
            {
                //GZipStream gzip = new GZipStream();
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            Environment.Exit(1);
        }
        #endregion
    }
}
