using System;
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
                    !string.IsNullOrEmpty(inif.Read("Application", "email")) &&
                    !string.IsNullOrEmpty(inif.Read("Application", "password"))  &&
                    !string.IsNullOrEmpty(inif.Read("Application", "migration_folder_path")) &&
                    Directory.Exists(inif.Read("Application", "migration_folder_path")))
                {

                    base_url.Text = inif.Read("Application", "base_url");
                    api_slug.Text = inif.Read("Application", "api_slug");
                    tags_api_slug.Text = inif.Read("Application", "tags_api_slug");
                    site_id.Text = inif.Read("Application", "site_id");
                    email.Text = inif.Read("Application", "email");
                    password.Password = inif.Read("Application", "password");
                    migrationFolder.Text = inif.Read("Application", "migration_folder_path");

                    if (inif.Read("Application", "forumData") == "")
                    {

                        forumData.IsChecked = false;

                    }


                    if (inif.Read("Application", "galleryData") == "")
                    {

                        galleryData.IsChecked = false;

                    }

                }
                else
                {

                    base_url.Text = inif.Read("Application", "base_url");
                    api_slug.Text = inif.Read("Application", "api_slug");
                    tags_api_slug.Text = inif.Read("Application", "tags_api_slug");
                    site_id.Text = inif.Read("Application", "site_id");
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
            inif.Write("Application", "email", email.Text.ToString());
            inif.Write("Application", "password", password.Password.ToString());
            inif.Write("Application", "migration_folder_path", migrationFolder.Text.ToString());

            if (forumData.IsChecked == true)
            {

                inif.Write("Application", "forumData", "true");

            }
            else
            {

                inif.Write("Application", "forumData", "false");

            }

            if (galleryData.IsChecked == true)
            {

                inif.Write("Application", "galleryData", "true");

            }
            else
            {

                inif.Write("Application", "galleryData", "false");

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
                    !string.IsNullOrEmpty(inif.Read("Application", "email")) &&
                    !string.IsNullOrEmpty(inif.Read("Application", "password")) &&
                    !string.IsNullOrEmpty(inif.Read("Application", "migration_folder_path")) &&
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
    }
}
