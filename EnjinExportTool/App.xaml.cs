using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EnjinExportTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

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

                    Application curApp = Application.Current;
                    curApp.StartupUri = new Uri("MainWindow.xaml", UriKind.RelativeOrAbsolute);

                }
                else
                {

                    Application curApp = Application.Current;
                    curApp.StartupUri = new Uri("SettingsWindow.xaml", UriKind.RelativeOrAbsolute);

                }
                
                

            }
            else
            {

                Application curApp = Application.Current;
                curApp.StartupUri = new Uri("SettingsWindow.xaml", UriKind.RelativeOrAbsolute);

            }

        }



    }
}
