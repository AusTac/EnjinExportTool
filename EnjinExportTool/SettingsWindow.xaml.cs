﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EnjinExportTool
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();



            string appfolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string ApplicationPath = System.IO.Path.Combine(appfolder, "EnjinExportTool/Application");
            if (!Directory.Exists(ApplicationPath)) Directory.CreateDirectory(ApplicationPath);
            INIFile inif = new INIFile(ApplicationPath + @"\Settings.ini");


            if (System.IO.File.Exists(ApplicationPath + @"\Settings.ini"))
            {
                if (!string.IsNullOrEmpty(inif.Read("Application", "base_url")) &&
                    !string.IsNullOrEmpty(inif.Read("Application", "api_slug")) &&
                    !string.IsNullOrEmpty(inif.Read("Application", "tags_api_slug")) &&
                    !string.IsNullOrEmpty(inif.Read("Application", "site_id")) &&
                    !string.IsNullOrEmpty(inif.Read("Application", "galley_preset_id")) &&
                    !string.IsNullOrEmpty(inif.Read("Application", "email")) &&
                    !string.IsNullOrEmpty(inif.Read("Application", "password")) &&
                    !string.IsNullOrEmpty(inif.Read("Application", "migration_folder_path")) &&
                    !string.IsNullOrEmpty(inif.Read("Application", "exportType")) &&
                    Directory.Exists(inif.Read("Application", "migration_folder_path")))
                {

                    base_url.Text = inif.Read("Application", "base_url");
                    api_slug.Text = inif.Read("Application", "api_slug");
                    tags_api_slug.Text = inif.Read("Application", "tags_api_slug");
                    site_id.Text = inif.Read("Application", "site_id");
                    galley_preset_id.Text = inif.Read("Application", "galley_preset_id");
                    email.Text = inif.Read("Application", "email");
                    password.Password = inif.Read("Application", "password");
                    migrationFolder.Text = inif.Read("Application", "migration_folder_path");


                    //forumData
                    if (inif.Read("Application", "forumData") == "true")
                    {

                        forumData.IsChecked = true;

                    }
                    else
                    {
                        forumData.IsChecked = false;

                    }



                    //galleryData
                    if (inif.Read("Application", "galleryData") == "true")
                    {

                        galleryData.IsChecked = true;

                    }
                    else
                    {
                        galleryData.IsChecked = false;

                    }


                    //dlOrg.
                    if (inif.Read("Application", "dlOriginal") == "true")
                    {

                        dlOriginal.IsChecked = true;

                    }
                    else
                    {
                        dlOriginal.IsChecked = false;

                    }



                    //dlThumb.
                    if (inif.Read("Application", "dlThumbnails") == "true")
                    {

                        dlThumbnails.IsChecked = true;

                    }
                    else
                    {
                        dlThumbnails.IsChecked = false;

                    }




                    //dlImg.
                    if (inif.Read("Application", "dlImages") == "true")
                    {

                        dlImages.IsChecked = true;

                    }
                    else
                    {
                        dlImages.IsChecked = false;

                    }


                    //backup.
                    if (inif.Read("Application", "backupApiData") == "true")
                    {

                        dlData.IsChecked = true;

                    }
                    else
                    {
                        dlData.IsChecked = false;

                    }


                    if (inif.Read("Application", "exportType") == "xml")
                    {
                        exportType.SelectedIndex = 0;

                    }

                    if (inif.Read("Application", "exportType") == "json")
                    {
                        exportType.SelectedIndex = 1;

                    }

                    if (inif.Read("Application", "exportType") == "csv")
                    {
                        exportType.SelectedIndex = 2;

                    }

                }
                else
                {

                    base_url.Text = inif.Read("Application", "base_url");
                    api_slug.Text = inif.Read("Application", "api_slug");
                    tags_api_slug.Text = inif.Read("Application", "tags_api_slug");
                    site_id.Text = inif.Read("Application", "site_id");
                    galley_preset_id.Text = inif.Read("Application", "galley_preset_id");
                    email.Text = inif.Read("Application", "email");
                    password.Password = inif.Read("Application", "password");
                    migrationFolder.Text = inif.Read("Application", "migration_folder_path");

                }

            }
            else
            {




            }

                



        }

        private void closeProcess_Click(object sender, RoutedEventArgs e)
        {

            /*
             * 
             * Close application, should wrap in exception, update the UI on error TODO
             * 
            */
            
            Application.Current.Shutdown();
        }

        private void saveProcess_Click(object sender, RoutedEventArgs e)
        {

            /*
             * 
             * Do save work with UI procgress using async
             * 
            */

            doSave();         
            
        }

        private async void doSave()
        {
            
            /*
             * 
             * UI Progress
             * 
            */

            feedback.Visibility = Visibility.Visible;
            feedback.Text = "Saving Settings ";

            loaderToolbar.Visibility = Visibility.Visible;
            await Task.Delay(200);

            feedback.Text = "Saving Settings .";
            await Task.Delay(200);
            feedback.Text = "Saving Settings ..";
            await Task.Delay(200);
            feedback.Text = "Saving Settings ...";
            await Task.Delay(200);

            /*
             * 
             * Folder checks
             * 
            */

            string appfolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string ApplicationPath = System.IO.Path.Combine(appfolder, "EnjinExportTool/Application");
            if (!Directory.Exists(ApplicationPath)) Directory.CreateDirectory(ApplicationPath);
            INIFile inif = new INIFile(ApplicationPath + @"\Settings.ini");

            /*
             * 
             * INI Writes
             * 
            */

            inif.Write("Application", "base_url", base_url.Text.ToString());
            inif.Write("Application", "api_slug", api_slug.Text.ToString());
            inif.Write("Application", "tags_api_slug", tags_api_slug.Text.ToString());
            inif.Write("Application", "site_id", site_id.Text.ToString());
            inif.Write("Application", "galley_preset_id", galley_preset_id.Text.ToString());
            inif.Write("Application", "email", email.Text.ToString());
            inif.Write("Application", "password", password.Password.ToString());
            inif.Write("Application", "migration_folder_path", migrationFolder.Text.ToString());

            if (exportType.SelectedIndex == 0)
            {
                inif.Write("Application", "exportType", "xml");

            }

            if (exportType.SelectedIndex == 1)
            {
                inif.Write("Application", "exportType", "json");

            }

            if (exportType.SelectedIndex == 2)
            {
                inif.Write("Application", "exportType", "csv");

            }

            if (forumData.IsChecked == true)
            {

                inif.Write("Application", "forumData", "true");

            }
            else
            {

                inif.Write("Application", "forumData", "");

            }

            if (galleryData.IsChecked == true)
            {

                inif.Write("Application", "galleryData", "true");

            }
            else
            {

                inif.Write("Application", "galleryData", "");

            }

            if (dlThumbnails.IsChecked == true)
            {

                inif.Write("Application", "dlThumbnails", "true");

            }
            else
            {

                inif.Write("Application", "dlThumbnails", "");

            }

            if (dlImages.IsChecked == true)
            {

                inif.Write("Application", "dlImages", "true");

            }
            else
            {

                inif.Write("Application", "dlImages", "");

            }

            if (dlOriginal.IsChecked == true)
            {

                inif.Write("Application", "dlOriginal", "true");

            }
            else
            {

                inif.Write("Application", "dlOriginal", "");

            }

            if (dlData.IsChecked == true)
            {

                inif.Write("Application", "backupApiData", "true");

            }
            else
            {

                inif.Write("Application", "backupApiData", "");

            }

            /*
             * 
             * Settings save checks
             * 
            */

            if (System.IO.File.Exists(ApplicationPath + @"\Settings.ini"))
            {
                if (!string.IsNullOrEmpty(inif.Read("Application", "base_url")) &&
                    !string.IsNullOrEmpty(inif.Read("Application", "api_slug")) &&
                    !string.IsNullOrEmpty(inif.Read("Application", "tags_api_slug")) &&
                    !string.IsNullOrEmpty(inif.Read("Application", "site_id")) &&
                    !string.IsNullOrEmpty(inif.Read("Application", "galley_preset_id")) &&
                    !string.IsNullOrEmpty(inif.Read("Application", "email")) &&
                    !string.IsNullOrEmpty(inif.Read("Application", "password")) &&
                    !string.IsNullOrEmpty(inif.Read("Application", "migration_folder_path")) &&
                    !string.IsNullOrEmpty(inif.Read("Application", "exportType")) &&
                    Directory.Exists(inif.Read("Application", "migration_folder_path")))
                {


                    /*
                     * 
                     * UI Progress
                     * 
                    */

                    feedback.Text = "Settings Saved .. RTB";
                    feedback.Foreground = new SolidColorBrush(Colors.Green);
                    await Task.Delay(1200);

                    loaderToolbar.Visibility = Visibility.Hidden;
                    feedback.Visibility = Visibility.Hidden;
                    await Task.Delay(100);

                    /*
                     * 
                     * Head back to MainWindow, close this one ...
                     * 
                    */

                    MainWindow MainWindow = new MainWindow();
                    MainWindow.Show();
                    this.Close();


                }
                else
                {

                    /*
                     * 
                     * Missing or invalid fields data, should perform checks below then call Focus()
                     * 
                    */


                    feedback.Visibility = Visibility.Visible;
                    feedback.Text = "Check you fields";
                    feedback.ToolTip = "Check you fields";
                    feedback.Foreground = new SolidColorBrush(Colors.Red);

                    loaderToolbar.Visibility = Visibility.Collapsed;


                }

            }
            else
            {


                /*
                    * 
                    * Missing or invalid fields data, should perform checks below then call Focus()
                    * 
                */

                feedback.Visibility = Visibility.Visible;
                feedback.Text = "Check you fields";
                feedback.ToolTip = "Check you fields";
                feedback.Foreground = new SolidColorBrush(Colors.Red);


                loaderToolbar.Visibility = Visibility.Collapsed;


            }

                
            

        }

        private void openFolder_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog dlg = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();

            // Set filter for file extension and default file extension 
            //dlg.DefaultExt = ".png";
            //dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {

                

                string path = dlg.SelectedPath;
                migrationFolder.Text = path;
                

            }



        }

        private void historyGoBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            /*
                * 
                * Head back to MainWindow, close this one ...
                * 
            */

            MainWindow MainWindow = new MainWindow();
            MainWindow.Show();
            this.Close();


        }

    }
}
