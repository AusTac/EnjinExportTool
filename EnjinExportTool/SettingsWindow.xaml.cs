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
                    !string.IsNullOrEmpty(inif.Read("Application", "password")))
                {

                    base_url.Text = inif.Read("Application", "base_url");
                    api_slug.Text = inif.Read("Application", "api_slug");
                    tags_api_slug.Text = inif.Read("Application", "tags_api_slug");
                    site_id.Text = inif.Read("Application", "site_id");
                    email.Text = inif.Read("Application", "email");
                    password.Password = inif.Read("Application", "password");

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


                }

            }
            else
            {




            }

                


        }

        private void closeProcess_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void saveProcess_Click(object sender, RoutedEventArgs e)
        {

            string appfolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string ApplicationPath = System.IO.Path.Combine(appfolder, "EnjinExportTool/Application");
            if (!Directory.Exists(ApplicationPath)) Directory.CreateDirectory(ApplicationPath);
            INIFile inif = new INIFile(ApplicationPath + @"\Settings.ini");

            inif.Write("Application", "base_url", base_url.Text.ToString());
            inif.Write("Application", "api_slug", api_slug.Text.ToString());
            inif.Write("Application", "tags_api_slug", tags_api_slug.Text.ToString());
            inif.Write("Application", "site_id", site_id.Text.ToString());
            inif.Write("Application", "email", email.Text.ToString());
            inif.Write("Application", "password", password.Password.ToString());

            if (forumData.IsChecked == true)
            {

                inif.Write("Application", "forumData", "true");

            }else{

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


            if (System.IO.File.Exists(ApplicationPath + @"\Settings.ini"))
            {
                if (!string.IsNullOrEmpty(inif.Read("Application", "base_url")) &&
                    !string.IsNullOrEmpty(inif.Read("Application", "api_slug")) &&
                    !string.IsNullOrEmpty(inif.Read("Application", "tags_api_slug")) &&
                    !string.IsNullOrEmpty(inif.Read("Application", "site_id")) &&
                    !string.IsNullOrEmpty(inif.Read("Application", "email")) &&
                    !string.IsNullOrEmpty(inif.Read("Application", "password")))
                {

                    MainWindow MainWindow = new MainWindow();
                    MainWindow.Show();
                    this.Close();

                }
                else
                {

                    
                    
                    
                    feedback.Visibility = Visibility.Visible;
                    feedback.Text = "Check you fields";
                    feedback.ToolTip = "Check you fields";
                    feedback.Foreground = new SolidColorBrush(Colors.Red);

                }

            }
            else
            {

                

                
                feedback.Visibility = Visibility.Visible;
                feedback.Text = "Check you fields";
                feedback.ToolTip = "Check you fields";
                feedback.Foreground = new SolidColorBrush(Colors.Red);

            }

                

         
            
        }
    }
}
