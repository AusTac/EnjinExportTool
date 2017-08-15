using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
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
using System.Xml;

namespace EnjinExportTool
{

     
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        private BackgroundWorker backgroundWorker = new BackgroundWorker();

        
        public MainWindow()
        {
            InitializeComponent();

            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            backgroundWorker.DoWork += DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

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

                    //ok to launch

                }
                else
                {

                    SettingsWindow SettingsWindow = new SettingsWindow();
                    SettingsWindow.Show();
                    this.Close();

                }

            }
            else
            {

                SettingsWindow SettingsWindow = new SettingsWindow();
                SettingsWindow.Show();
                this.Close();

            }

            

        }

        private async void ImageSlider()
        {
            await Task.Delay(5000);
            Random r = new Random();

            while (true)
            {

                int[] numbers = new int[8] { 1, 2, 3, 5, 5, 6, 7, 8 };
                Random rd = new Random();
                int randomIndex = rd.Next(1, 8);
                int randomNumber = numbers[randomIndex];

                image.Source = new BitmapImage(new Uri(@"/assets/images/slider/slider_image_" + randomNumber + ".jpg", UriKind.Relative));

                await Task.Delay(5500);

            }
        }

        private async void ClearMessages()
        {

            loaderToolbar.Visibility = Visibility.Collapsed;
            await Task.Delay(5000);
            feedback.Text = "";
            feedback.ToolTip = "";

        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {

            
            Thread.Sleep(200);

            string[] processState = new string[50];
            try
            {

                BackgroundWorker worker = sender as BackgroundWorker;
                Object[] arg = e.Argument as Object[];
                //string siteId = (string)arg[0];
                //string forumProcessing = (string)arg[1];
                //string galleryProcessing = (string)arg[2];

                string start_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);


                if (backgroundWorker.CancellationPending == true)
                {
                    e.Cancel = true;
                    return;
                }

                string appfolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string ApplicationPath = System.IO.Path.Combine(appfolder, "EnjinExportTool/Application/");
                if (!Directory.Exists(ApplicationPath)) Directory.CreateDirectory(ApplicationPath);

                string JsonFolderPath = System.IO.Path.Combine(appfolder, "EnjinExportTool/Json/Data/");
                if (!Directory.Exists(JsonFolderPath)) Directory.CreateDirectory(JsonFolderPath);

                string JsonFolderUsersPath = System.IO.Path.Combine(appfolder, JsonFolderPath + "/Users/");
                if (!Directory.Exists(JsonFolderUsersPath)) Directory.CreateDirectory(JsonFolderUsersPath);

                string JsonFolderGalleryPath = System.IO.Path.Combine(appfolder, JsonFolderPath + "/Galleries/");
                if (!Directory.Exists(JsonFolderGalleryPath)) Directory.CreateDirectory(JsonFolderGalleryPath);

                string LogPath = System.IO.Path.Combine(appfolder, "EnjinExportTool/Logging");
                if (!Directory.Exists(LogPath)) Directory.CreateDirectory(LogPath);

                INIFile inif = new INIFile(ApplicationPath + @"\Settings.ini");

                string base_enjin_url = "";
                string base_api_slug = "";
                string tags_api_slug = "api/get-tags";
                string site_id = "";
                string galley_preset_id = "";
                string email = "";
                string password = "";
                string session_id = "";

                string migration_folder_path = "";
                string migration_folder_path_root = "";
                string migration_folder_json_path = "";
                string migration_folder_images_path = "";
                string migration_folder_xml_path = "";
                string migration_folder_csv_path = "";
                string migration_folder_temp_path = "";
                string exportType = "";

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

                        base_enjin_url = inif.Read("Application", "base_url");
                        base_api_slug = inif.Read("Application", "api_slug");
                        tags_api_slug = inif.Read("Application", "tags_api_slug");
                        site_id = inif.Read("Application", "site_id");
                        galley_preset_id = inif.Read("Application", "galley_preset_id");
                        email = inif.Read("Application", "email");
                        password = inif.Read("Application", "password");
                        session_id = inif.Read("Application", "session_id");
                        migration_folder_path = inif.Read("Application", "migration_folder_path");
                        exportType = inif.Read("Application", "exportType");

                        //internal migration folder

                        //base
                        migration_folder_path_root = migration_folder_path + @"\EnjinExportTool\migration\latest\";
                        if (!Directory.Exists(migration_folder_path_root)) Directory.CreateDirectory(migration_folder_path_root);

                        //json
                        migration_folder_json_path = migration_folder_path_root + @"\data\json\";
                        if (!Directory.Exists(migration_folder_json_path)) Directory.CreateDirectory(migration_folder_json_path);

                        //xml
                        migration_folder_xml_path = migration_folder_path_root + @"\data\xml\";
                        if (!Directory.Exists(migration_folder_xml_path)) Directory.CreateDirectory(migration_folder_xml_path);

                        //csv
                        migration_folder_csv_path = migration_folder_path_root + @"\data\csv\";
                        if (!Directory.Exists(migration_folder_csv_path)) Directory.CreateDirectory(migration_folder_csv_path);

                        //media/images
                        migration_folder_images_path = migration_folder_path_root + @"\media\images\";
                        if (!Directory.Exists(migration_folder_images_path)) Directory.CreateDirectory(migration_folder_images_path);



                    }


                }


                #region start checks
                //do some folder creations & checks before moving on ...

                try
                {

                    if(Directory.Exists(migration_folder_path_root) &&
                        Directory.Exists(migration_folder_json_path) &&                        
                        Directory.Exists(migration_folder_xml_path) &&
                        Directory.Exists(migration_folder_csv_path) &&
                        Directory.Exists(migration_folder_images_path))
                    {

                        #region try login
                        try
                        {

                            #region do execute script

                            try
                            {

                                if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory.ToString() + @"phpscripts\wordpress.php"))
                                {

                                    string fileName = "wordpress.php";
                                    string newfileName = "index.php";
                                    string sourcePath = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + @"phpscripts\";
                                    string targetPath = migration_folder_path_root;

                                    // Use Path class to manipulate file and directory paths.
                                    string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
                                    string destFile = System.IO.Path.Combine(targetPath, newfileName);
                                    System.IO.File.Copy(sourceFile, destFile, true);

                                    if (File.Exists(migration_folder_path_root + "index.php"))
                                    {

                                        Thread.Sleep(3200);

                                        processState[0] = "event";
                                        processState[1] = "Validating execute script";
                                        processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                        processState[3] = "";
                                        processState[4] = "";
                                        processState[5] = "";
                                        processState[6] = "";
                                        processState[7] = "";
                                        processState[8] = "";
                                        processState[9] = "";
                                        processState[10] = "";
                                        backgroundWorker.ReportProgress(25, processState);

                                        if (backgroundWorker.CancellationPending == true)
                                        {
                                            e.Cancel = true;
                                            return;
                                        }


                                    }
                                    else
                                    {

                                        //Throw FATAL as we done have access to the new index.php
                                        #region error ui
                                        processState[0] = "error";
                                        processState[1] = "Error validating execute script";
                                        processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                        backgroundWorker.ReportProgress(100, processState);

                                        backgroundWorker.CancelAsync();
                                        backgroundWorker.Dispose();

                                        if (backgroundWorker.CancellationPending == true)
                                        {
                                            e.Cancel = true;
                                            return;
                                        }

                                        #endregion error ui

                                        #region logging

                                        try
                                        {


                                            //Logging                        

                                            using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                            {
                                                string time = DateTime.Now.ToString();
                                                sw.WriteLine("LogStart -------------------------------------------------------------------");
                                                sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                                sw.WriteLine("LogType NullRef");
                                                sw.WriteLine("LogMessage Could not validate the php execute script.");
                                                sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                            }
                                        }
                                        catch (Exception log_error)
                                        {
                                            //super fail

                                        }

                                        #endregion logging

                                    }

                                }
                                else
                                {

                                    Console.WriteLine("error here ---------------------------------------------->>>>>>>>>>>>>>>>>>>>>");
                                    //Throw FATAL as we dont have access to the php, is it a resource or content item?
                                    #region error ui
                                    processState[0] = "error";
                                    processState[1] = "Error validating execute script";
                                    processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                    backgroundWorker.ReportProgress(100, processState);

                                    backgroundWorker.CancelAsync();
                                    backgroundWorker.Dispose();

                                    if (backgroundWorker.CancellationPending == true)
                                    {
                                        e.Cancel = true;
                                        return;
                                    }

                                    #endregion error ui

                                    #region logging

                                    try
                                    {


                                        //Logging                        

                                        using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                        {
                                            string time = DateTime.Now.ToString();
                                            sw.WriteLine("LogStart -------------------------------------------------------------------");
                                            sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                            sw.WriteLine("LogType NullRef");
                                            sw.WriteLine("LogMessage Could not validate the php execute script. Might not be added as a resource during build/compile time.");
                                            sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                        }
                                    }
                                    catch (Exception log_error)
                                    {
                                        //super fail

                                    }

                                    #endregion logging


                                }

                            }catch(Exception error) {

                                Console.WriteLine(error);
                                //Throw FATAL as we dont have access to the php, is it a resource or content item?
                                #region error ui
                                processState[0] = "error";
                                processState[1] = "Error validating execute script";
                                processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                backgroundWorker.ReportProgress(100, processState);

                                backgroundWorker.CancelAsync();
                                backgroundWorker.Dispose();

                                if (backgroundWorker.CancellationPending == true)
                                {
                                    e.Cancel = true;
                                    return;
                                }

                                #endregion error ui

                                #region logging

                                try
                                {


                                    //Logging                        

                                    using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                    {
                                        string time = DateTime.Now.ToString();
                                        sw.WriteLine("LogStart -------------------------------------------------------------------");
                                        sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                        sw.WriteLine("LogType Exception");
                                        sw.WriteLine("LogMessage " + error);
                                        sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                    }
                                }
                                catch (Exception log_error)
                                {
                                    //super fail

                                }

                                #endregion logging


                            }

                            #endregion do execute script


                        if (backgroundWorker.CancellationPending == true)
                        {
                            e.Cancel = true;
                            return;
                        }

                        processState[0] = "event";
                        processState[1] = "Connecting to Enjin ...";
                        processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                        processState[3] = "";
                        processState[4] = "";
                        processState[5] = "";
                        processState[6] = "";
                        processState[7] = "";
                        processState[8] = "";
                        processState[9] = "";
                        processState[10] = "";
                        backgroundWorker.ReportProgress(5, processState);

                        string responseJSON = null;


                        #region session auth
                        //enjin login json post data format

                        JObject sessionJson =
                        new JObject(
                            new JProperty("jsonrpc", "2.0"),
                            new JProperty("id", "123456789"),
                            new JProperty("method", "User.checkSession"),
                            new JProperty("params",
                                new JObject(new JProperty("session_id", session_id)))
                            );

                        JObject loginJson =
                        new JObject(
                            new JProperty("jsonrpc", "2.0"),
                            new JProperty("id", "123456789"),
                            new JProperty("method", "User.login"),
                            new JProperty("params",
                                new JObject(new JProperty("email", email),
                                            new JProperty("password", password)))
                          );


                        //setup cookies for use later
                        CookieCollection cookies = new CookieCollection();
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(base_enjin_url);
                        request.CookieContainer = new CookieContainer();
                        request.CookieContainer.Add(cookies);
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        cookies = response.Cookies;

                        if (session_id == null || session_id == "")
                        {

                            #region new session
                            //login to Enjin API
                            HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(base_enjin_url + base_api_slug);
                            getRequest.CookieContainer = new CookieContainer();
                            getRequest.CookieContainer.Add(cookies);
                            getRequest.Method = WebRequestMethods.Http.Post;
                            getRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                            getRequest.AllowWriteStreamBuffering = true;
                            getRequest.ProtocolVersion = HttpVersion.Version11;
                            getRequest.AllowAutoRedirect = true;
                            getRequest.ContentType = "application/json";
                            getRequest.KeepAlive = true;
                            cookies = response.Cookies;
                            byte[] byteArray = Encoding.ASCII.GetBytes(loginJson.ToString());
                            getRequest.ContentLength = loginJson.ToString().Length;
                            Stream newStream = getRequest.GetRequestStream();
                            newStream.Write(byteArray, 0, loginJson.ToString().Length);
                            newStream.Close();

                            HttpWebResponse getResponse = (HttpWebResponse)getRequest.GetResponse();
                            using (StreamReader sr = new StreamReader(getResponse.GetResponseStream()))
                            {

                                processState[0] = "event";
                                processState[1] = "New Authenticating ...";
                                processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                processState[3] = "";
                                processState[4] = "";
                                processState[5] = "";
                                processState[6] = "";
                                processState[7] = "";
                                processState[8] = "";
                                processState[9] = "";
                                processState[10] = "";
                                Thread.Sleep(1200);

                                backgroundWorker.ReportProgress(10, processState);

                                responseJSON = sr.ReadToEnd();

                            }

                            if (responseJSON == null || responseJSON == "")
                            {

                                #region error ui
                                processState[0] = "error";
                                processState[1] = "Authenticating failed";
                                processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                backgroundWorker.ReportProgress(100, processState);

                                backgroundWorker.CancelAsync();
                                backgroundWorker.Dispose();

                                if (backgroundWorker.CancellationPending == true)
                                {
                                    e.Cancel = true;
                                    return;
                                }

                                #endregion error ui

                                #region logging

                                try
                                {


                                    //Logging                        

                                    using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                    {
                                        string time = DateTime.Now.ToString();
                                        sw.WriteLine("LogStart -------------------------------------------------------------------");
                                        sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                        sw.WriteLine("LogType Exception");
                                        sw.WriteLine("LogMessage Authentication failed the handshake between the enjin platform and your user details");
                                        sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                    }
                                }
                                catch (Exception log_error)
                                {
                                    //super fail

                                }

                                #endregion logging

                            }
                            else
                            {

                                if (backgroundWorker.CancellationPending == true)
                                {
                                    e.Cancel = true;
                                    return;
                                }




                                try
                                {

                                    String jsonResponse = "[" + responseJSON.ToString() + "]";
                                    JArray userModelArray = JArray.Parse(jsonResponse);

                                    try
                                    {

                                        IList<EnjinUserModel.Result> EnjinUserModel = userModelArray.Select(p => new EnjinUserModel.Result
                                        {
                                            session_id = (string)p["result"]["session_id"],
                                            id = (string)p["result"]["id"],
                                            user_id = (string)p["result"]["user_id"],
                                            site_id = site_id

                                        }).ToList();

                                        session_id = EnjinUserModel[0].session_id.ToString();
                                        inif.Write("Application", "session_id", session_id);

                                        processState[0] = "event";
                                        processState[1] = "Session Authenticated...";
                                        processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                        processState[3] = "";
                                        processState[4] = "";
                                        processState[5] = "";
                                        processState[6] = "";
                                        processState[7] = "";
                                        processState[8] = "";
                                        processState[9] = "";
                                        processState[10] = "";
                                        Thread.Sleep(1200);


                                    }
                                    catch (Exception error)
                                    {

                                        IList<EnjinErrorModel.Error> EnjinErrorModel = userModelArray.Select(p => new EnjinErrorModel.Error
                                        {
                                            message = (string)p["error"]["message"]

                                        }).ToList();



                                        #region error ui
                                        processState[0] = "error";
                                        processState[1] = "Error processing the API";
                                        processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                        backgroundWorker.ReportProgress(100, processState);

                                        backgroundWorker.CancelAsync();
                                        backgroundWorker.Dispose();

                                        if (backgroundWorker.CancellationPending == true)
                                        {
                                            e.Cancel = true;
                                            return;
                                        }

                                        #endregion error ui

                                        #region logging

                                        try
                                        {


                                            //Logging                        

                                            using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                            {
                                                string time = DateTime.Now.ToString();
                                                sw.WriteLine("LogStart -------------------------------------------------------------------");
                                                sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                                sw.WriteLine("LogType Exception");
                                                sw.WriteLine("LogMessage " + error);
                                                sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                            }
                                        }
                                        catch (Exception log_error)
                                        {
                                            //super fail

                                        }

                                        #endregion logging


                                    }





                                }
                                catch (Exception errorJSONParse)
                                {

                                    Console.WriteLine(errorJSONParse);

                                    #region error ui
                                    processState[0] = "error";
                                    processState[1] = "processing failed";
                                    processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                    backgroundWorker.ReportProgress(100, processState);

                                    backgroundWorker.CancelAsync();
                                    backgroundWorker.Dispose();

                                    if (backgroundWorker.CancellationPending == true)
                                    {
                                        e.Cancel = true;
                                        return;
                                    }

                                    #endregion error ui

                                    #region logging

                                    try
                                    {


                                        //Logging                        

                                        using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                        {
                                            string time = DateTime.Now.ToString();
                                            sw.WriteLine("LogStart -------------------------------------------------------------------");
                                            sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                            sw.WriteLine("LogType Exception");
                                            sw.WriteLine("LogMessage " + errorJSONParse);
                                            sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                        }
                                    }
                                    catch (Exception log_error)
                                    {
                                        //super fail

                                    }

                                    #endregion logging

                                }


                            }
                            #endregion new session

                        }
                        else
                        {

                            string validAuthJSON = "";
                            //validate session
                            //login to Enjin API
                            HttpWebRequest validAuthRequest = (HttpWebRequest)WebRequest.Create(base_enjin_url + base_api_slug);
                            validAuthRequest.CookieContainer = new CookieContainer();
                            validAuthRequest.CookieContainer.Add(cookies);
                            validAuthRequest.Method = WebRequestMethods.Http.Post;
                            validAuthRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                            validAuthRequest.AllowWriteStreamBuffering = true;
                            validAuthRequest.ProtocolVersion = HttpVersion.Version11;
                            validAuthRequest.AllowAutoRedirect = true;
                            validAuthRequest.ContentType = "application/json";
                            validAuthRequest.KeepAlive = true;
                            cookies = response.Cookies;
                            byte[] validAuthArray = Encoding.ASCII.GetBytes(sessionJson.ToString());
                            validAuthRequest.ContentLength = sessionJson.ToString().Length;
                            Stream validAuthStream = validAuthRequest.GetRequestStream();
                            validAuthStream.Write(validAuthArray, 0, sessionJson.ToString().Length);
                            validAuthStream.Close();

                            HttpWebResponse validAuthResponse = (HttpWebResponse)validAuthRequest.GetResponse();
                            using (StreamReader validAuthResponseReader = new StreamReader(validAuthResponse.GetResponseStream()))
                            {


                                validAuthJSON = validAuthResponseReader.ReadToEnd();
                                if (validAuthJSON != null)
                                {

                                    processState[0] = "event";
                                    processState[1] = "Authenticating Session...";
                                    processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                    processState[3] = "";
                                    processState[4] = "";
                                    processState[5] = "";
                                    processState[6] = "";
                                    processState[7] = "";
                                    processState[8] = "";
                                    processState[9] = "";
                                    processState[10] = "";
                                    Thread.Sleep(1200);

                                    backgroundWorker.ReportProgress(10, processState);


                                    try
                                    {


                                        String _validAuthJSONData = "[" + validAuthJSON.ToString() + "]";
                                        JArray validAuthJSONDataModelArray = JArray.Parse(_validAuthJSONData);

                                        try
                                        {

                                            IList<EnjinUserModel.Result> EnjinUserModel = validAuthJSONDataModelArray.Select(p => new EnjinUserModel.Result
                                            {
                                                hasIdentity = (string)p["result"]["hasIdentity"],
                                                session_id = (string)p["result"]["session_id"],
                                                id = (string)p["result"]["id"],
                                                user_id = (string)p["result"]["user_id"],
                                                site_id = site_id

                                            }).ToList();

                                            Console.WriteLine(EnjinUserModel[0].hasIdentity.ToString());


                                            if (EnjinUserModel[0].hasIdentity.ToString() == "True")
                                            {

                                                processState[0] = "event";
                                                processState[1] = "Session Authenticated...";
                                                processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                                processState[3] = "";
                                                processState[4] = "";
                                                processState[5] = "";
                                                processState[6] = "";
                                                processState[7] = "";
                                                processState[8] = "";
                                                processState[9] = "";
                                                processState[10] = "";
                                                Thread.Sleep(1200);

                                            }
                                            else
                                            {

                                                //do now session
                                                #region new session
                                                //login to Enjin API
                                                HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(base_enjin_url + base_api_slug);
                                                getRequest.CookieContainer = new CookieContainer();
                                                getRequest.CookieContainer.Add(cookies);
                                                getRequest.Method = WebRequestMethods.Http.Post;
                                                getRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                                                getRequest.AllowWriteStreamBuffering = true;
                                                getRequest.ProtocolVersion = HttpVersion.Version11;
                                                getRequest.AllowAutoRedirect = true;
                                                getRequest.ContentType = "application/json";
                                                getRequest.KeepAlive = true;
                                                cookies = response.Cookies;
                                                byte[] byteArray = Encoding.ASCII.GetBytes(loginJson.ToString());
                                                getRequest.ContentLength = loginJson.ToString().Length;
                                                Stream newStream = getRequest.GetRequestStream();
                                                newStream.Write(byteArray, 0, loginJson.ToString().Length);
                                                newStream.Close();

                                                HttpWebResponse getResponse = (HttpWebResponse)getRequest.GetResponse();
                                                using (StreamReader sr = new StreamReader(getResponse.GetResponseStream()))
                                                {

                                                    processState[0] = "event";
                                                    processState[1] = "New Authenticating ...";
                                                    processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                                    processState[3] = "";
                                                    processState[4] = "";
                                                    processState[5] = "";
                                                    processState[6] = "";
                                                    processState[7] = "";
                                                    processState[8] = "";
                                                    processState[9] = "";
                                                    processState[10] = "";
                                                    Thread.Sleep(1200);

                                                    backgroundWorker.ReportProgress(10, processState);

                                                    responseJSON = sr.ReadToEnd();

                                                }

                                                if (responseJSON == null || responseJSON == "")
                                                {

                                                    #region error ui
                                                    processState[0] = "error";
                                                    processState[1] = "Authenticating failed";
                                                    processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                                    backgroundWorker.ReportProgress(100, processState);

                                                    backgroundWorker.CancelAsync();
                                                    backgroundWorker.Dispose();

                                                    if (backgroundWorker.CancellationPending == true)
                                                    {
                                                        e.Cancel = true;
                                                        return;
                                                    }

                                                    #endregion error ui

                                                    #region logging

                                                    try
                                                    {


                                                        //Logging                        

                                                        using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                                        {
                                                            string time = DateTime.Now.ToString();
                                                            sw.WriteLine("LogStart -------------------------------------------------------------------");
                                                            sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                                            sw.WriteLine("LogType Exception");
                                                            sw.WriteLine("LogMessage Could not authenticate the user");
                                                            sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                                        }
                                                    }
                                                    catch (Exception log_error)
                                                    {
                                                        //super fail

                                                    }

                                                    #endregion logging

                                                }
                                                else
                                                {

                                                    if (backgroundWorker.CancellationPending == true)
                                                    {
                                                        e.Cancel = true;
                                                        return;
                                                    }




                                                    try
                                                    {

                                                        String jsonResponse = "[" + responseJSON.ToString() + "]";
                                                        JArray userModelArray = JArray.Parse(jsonResponse);

                                                        try
                                                        {

                                                            IList<EnjinUserModel.Result> _EnjinUserModel = userModelArray.Select(p => new EnjinUserModel.Result
                                                            {
                                                                session_id = (string)p["result"]["session_id"],
                                                                id = (string)p["result"]["id"],
                                                                user_id = (string)p["result"]["user_id"],
                                                                site_id = site_id

                                                            }).ToList();

                                                            session_id = _EnjinUserModel[0].session_id.ToString();
                                                            inif.Write("Application", "session_id", session_id);

                                                            processState[0] = "event";
                                                            processState[1] = "Session Authenticated...";
                                                            processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                                            processState[3] = "";
                                                            processState[4] = "";
                                                            processState[5] = "";
                                                            processState[6] = "";
                                                            processState[7] = "";
                                                            processState[8] = "";
                                                            processState[9] = "";
                                                            processState[10] = "";
                                                            Thread.Sleep(1200);


                                                        }
                                                        catch (Exception error)
                                                        {

                                                            IList<EnjinErrorModel.Error> EnjinErrorModel = userModelArray.Select(p => new EnjinErrorModel.Error
                                                            {
                                                                message = (string)p["error"]["message"]

                                                            }).ToList();



                                                            #region error ui
                                                            processState[0] = "error";
                                                            processState[1] = EnjinErrorModel[0].message.ToString();
                                                            processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                                            backgroundWorker.ReportProgress(100, processState);

                                                            backgroundWorker.CancelAsync();
                                                            backgroundWorker.Dispose();

                                                            if (backgroundWorker.CancellationPending == true)
                                                            {
                                                                e.Cancel = true;
                                                                return;
                                                            }

                                                            #endregion error ui

                                                            #region logging

                                                            try
                                                            {


                                                                //Logging                        

                                                                using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                                                {
                                                                    string time = DateTime.Now.ToString();
                                                                    sw.WriteLine("LogStart -------------------------------------------------------------------");
                                                                    sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                                                    sw.WriteLine("LogType Exception");
                                                                    sw.WriteLine("LogMessage " + error);
                                                                    sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                                                }
                                                            }
                                                            catch (Exception log_error)
                                                            {
                                                                //super fail

                                                            }

                                                            #endregion logging


                                                        }





                                                    }
                                                    catch (Exception errorJSONParse)
                                                    {

                                                        Console.WriteLine(errorJSONParse);

                                                        #region error ui
                                                        processState[0] = "error";
                                                        processState[1] = "processing failed";
                                                        processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                                        backgroundWorker.ReportProgress(100, processState);

                                                        backgroundWorker.CancelAsync();
                                                        backgroundWorker.Dispose();

                                                        if (backgroundWorker.CancellationPending == true)
                                                        {
                                                            e.Cancel = true;
                                                            return;
                                                        }

                                                        #endregion error ui

                                                        #region logging

                                                        try
                                                        {


                                                            //Logging                        

                                                            using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                                            {
                                                                string time = DateTime.Now.ToString();
                                                                sw.WriteLine("LogStart -------------------------------------------------------------------");
                                                                sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                                                sw.WriteLine("LogType Exception");
                                                                sw.WriteLine("LogMessage " + errorJSONParse);
                                                                sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                                            }
                                                        }
                                                        catch (Exception log_error)
                                                        {
                                                            //super fail

                                                        }

                                                        #endregion logging

                                                    }


                                                }
                                                #endregion new session

                                            }

                                        }
                                        catch (Exception error)
                                        {

                                            IList<EnjinErrorModel.Error> EnjinErrorModel = validAuthJSONDataModelArray.Select(p => new EnjinErrorModel.Error
                                            {
                                                message = (string)p["error"]["message"]

                                            }).ToList();



                                            #region error ui
                                            processState[0] = "error";
                                            processState[1] = EnjinErrorModel[0].message.ToString();
                                            processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                            backgroundWorker.ReportProgress(100, processState);

                                            backgroundWorker.CancelAsync();
                                            backgroundWorker.Dispose();

                                            if (backgroundWorker.CancellationPending == true)
                                            {
                                                e.Cancel = true;
                                                return;
                                            }

                                            #endregion error ui

                                            #region logging

                                            try
                                            {


                                                //Logging                        

                                                using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                                {
                                                    string time = DateTime.Now.ToString();
                                                    sw.WriteLine("LogStart -------------------------------------------------------------------");
                                                    sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                                    sw.WriteLine("LogType Exception");
                                                    sw.WriteLine("LogMessage " + error);
                                                    sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                                }
                                            }
                                            catch (Exception log_error)
                                            {
                                                //super fail

                                            }

                                            #endregion logging


                                        }



                                    }
                                    catch (Exception error)
                                    {

                                        #region error ui
                                        processState[0] = "error";
                                        processState[1] = "Authentication failed";
                                        processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                        backgroundWorker.ReportProgress(100, processState);

                                        backgroundWorker.CancelAsync();
                                        backgroundWorker.Dispose();

                                        if (backgroundWorker.CancellationPending == true)
                                        {
                                            e.Cancel = true;
                                            return;
                                        }

                                        #endregion error ui

                                        #region logging

                                        try
                                        {


                                            //Logging                        

                                            using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                            {
                                                string time = DateTime.Now.ToString();
                                                sw.WriteLine("LogStart -------------------------------------------------------------------");
                                                sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                                sw.WriteLine("LogType Exception");
                                                sw.WriteLine("LogMessage " + error);
                                                sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                            }
                                        }
                                        catch (Exception log_error)
                                        {
                                            //super fail

                                        }

                                        #endregion logging

                                    }


                                }
                                else
                                {

                                    #region error ui
                                    processState[0] = "error";
                                    processState[1] = "Authentication failed";
                                    processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                    backgroundWorker.ReportProgress(100, processState);

                                    backgroundWorker.CancelAsync();
                                    backgroundWorker.Dispose();

                                    if (backgroundWorker.CancellationPending == true)
                                    {
                                        e.Cancel = true;
                                        return;
                                    }

                                    #endregion error ui

                                    #region logging

                                    try
                                    {


                                        //Logging                        

                                        using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                        {
                                            string time = DateTime.Now.ToString();
                                            sw.WriteLine("LogStart -------------------------------------------------------------------");
                                            sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                            sw.WriteLine("LogType Exception");
                                            sw.WriteLine("LogMessage Authentication failed");
                                            sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                        }
                                    }
                                    catch (Exception log_error)
                                    {
                                        //super fail

                                    }

                                    #endregion logging


                                }

                            }

                        }

                        #endregion session auth

                        #region do api session check

                        if (session_id != null)
                        {

                            #region do api gallery

                            if (inif.Read("Application", "galleryData") == "true")
                            {

                                if (inif.Read("Application", "dlOriginal") == "true" ||
                                inif.Read("Application", "dlThumbnails") == "true" ||
                                inif.Read("Application", "dlImages") == "true")
                                {

                                    #region do gallery work

                                    if (backgroundWorker.CancellationPending == true)
                                    {
                                        e.Cancel = true;
                                        return;
                                    }

                                    if (galley_preset_id != null)
                                    {

                                        #region do try api gallery

                                        try
                                        {

                                            List<EnjinExportTool.ExportGalleryModels.GalleryCategoryModel> GalleryCategoryModelList = new List<EnjinExportTool.ExportGalleryModels.GalleryCategoryModel>();
                                            List<EnjinExportTool.ExportGalleryModels.GalleryItemModel> GalleryCategoryItemModelList = new List<EnjinExportTool.ExportGalleryModels.GalleryItemModel>();
                                            List<EnjinExportTool.ExportGalleryModels.ErrorEventModel> GalleryErrorEventModelList = new List<EnjinExportTool.ExportGalleryModels.ErrorEventModel>();


                                            #region do api gallery inner region

                                            

                                            if (backgroundWorker.CancellationPending == true)
                                            {
                                                e.Cancel = true;
                                                return;
                                            }



                                            JObject galleryJson =
                                            new JObject(
                                                new JProperty("jsonrpc", "2.0"),
                                                new JProperty("id", "123456789"),
                                                new JProperty("method", "Gallery.getAlbums"),
                                                new JProperty("params",
                                                    new JObject(new JProperty("session_id", session_id.ToString())))
                                                );

                                            int galleryCount = 0;
                                            string galleryJsonRequestJSON = null;
                                            HttpWebRequest galleryJsonRequest = (HttpWebRequest)WebRequest.Create(base_enjin_url + base_api_slug);
                                            galleryJsonRequest.CookieContainer = new CookieContainer();
                                            galleryJsonRequest.Method = WebRequestMethods.Http.Post;
                                            galleryJsonRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                                            galleryJsonRequest.AllowWriteStreamBuffering = true;
                                            galleryJsonRequest.ProtocolVersion = HttpVersion.Version11;
                                            galleryJsonRequest.AllowAutoRedirect = true;
                                            galleryJsonRequest.ContentType = "application/json";
                                            galleryJsonRequest.KeepAlive = true;
                                            byte[] galleryJsonRequestArray = Encoding.ASCII.GetBytes(galleryJson.ToString());
                                            galleryJsonRequest.ContentLength = galleryJson.ToString().Length;
                                            Stream galleryJsonRequestStream = galleryJsonRequest.GetRequestStream(); //open connection
                                            galleryJsonRequestStream.Write(galleryJsonRequestArray, 0, galleryJson.ToString().Length); // Send the data.
                                            galleryJsonRequestStream.Close();

                                            if (backgroundWorker.CancellationPending == true)
                                            {
                                                e.Cancel = true;
                                                return;
                                            }

                                            HttpWebResponse galleryJsonRequestResponse = (HttpWebResponse)galleryJsonRequest.GetResponse();
                                            using (StreamReader galleryJsonRequestReader = new StreamReader(galleryJsonRequestResponse.GetResponseStream()))
                                            {

                                                if (backgroundWorker.CancellationPending == true)
                                                {
                                                    e.Cancel = true;
                                                    return;
                                                }

                                                processState[0] = "event";
                                                processState[1] = "Exporting Gallery API... ";
                                                processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                                processState[3] = "";
                                                processState[4] = "";
                                                processState[5] = "";
                                                processState[6] = "";
                                                processState[7] = "";
                                                processState[8] = "";
                                                processState[9] = "";
                                                processState[10] = "";
                                                backgroundWorker.ReportProgress(60, processState);

                                                galleryJsonRequestJSON = galleryJsonRequestReader.ReadToEnd();
                                                galleryJsonRequestReader.Close();

                                                // Write the string to a file.
                                                System.IO.StreamWriter fileGalleryJson = new System.IO.StreamWriter(JsonFolderGalleryPath + "galleries" + ".json");
                                                fileGalleryJson.WriteLine(galleryJsonRequestJSON);
                                                fileGalleryJson.Close();

                                                Console.WriteLine("API Gallery Response: ----------------------- " + galleryJsonRequestJSON);

                                                if (inif.Read("Application", "backupApiData") == "true")
                                                {

                                                    System.IO.StreamWriter gallery_api_file = new System.IO.StreamWriter(migration_folder_json_path + "galleries" + ".json");
                                                    gallery_api_file.WriteLine(galleryJsonRequestJSON);
                                                    gallery_api_file.Close();

                                                }


                                                if (backgroundWorker.CancellationPending == true)
                                                {
                                                    e.Cancel = true;
                                                    return;
                                                }


                                                #region do json response work

                                                
                                                string galleryJsonRequestJSONData = "[" + galleryJsonRequestJSON.ToString() + "]";
                                                JArray galleryJsonRequestJSONArray = JArray.Parse(galleryJsonRequestJSONData);

                                                foreach (JObject galleryJsonRequestJSONContent in galleryJsonRequestJSONArray.Children<JObject>())
                                                {

                                                    if (backgroundWorker.CancellationPending == true)
                                                    {
                                                        e.Cancel = true;
                                                        return;
                                                    }
                                                    
                                                    foreach (JProperty galleryJsonRequestJSONNode in galleryJsonRequestJSONContent.Properties())
                                                    {


                                                        if (backgroundWorker.CancellationPending == true)
                                                        {
                                                            e.Cancel = true;
                                                            return;
                                                        }
                                                        
                                                        foreach (JObject galleryJsonRequestJSONNodeContent in galleryJsonRequestJSONNode.Children<JObject>()["Pictures"])
                                                        {


                                                            if (backgroundWorker.CancellationPending == true)
                                                            {
                                                                e.Cancel = true;
                                                                return;
                                                            }
                                                            
                                                            foreach (JProperty galleryInnerNode in galleryJsonRequestJSONNodeContent.Properties())
                                                            {

                                                                if (backgroundWorker.CancellationPending == true)
                                                                {
                                                                    e.Cancel = true;
                                                                    return;
                                                                }

                                                                #region add to gallery model

                                                                #region do GalleryCatergoryModelList checks

                                                                if (backgroundWorker.CancellationPending == true)
                                                                {
                                                                    e.Cancel = true;
                                                                    return;
                                                                }

                                                                if (GalleryCategoryModelList.Count() == 0 ||
                                                                    GalleryCategoryModelList.Count() == null)
                                                                {
                                                                    //empty UserModelList, add to Model
                                                                    GalleryCategoryModelList.Add(new EnjinExportTool.ExportGalleryModels.GalleryCategoryModel()
                                                                    {
                                                                        id = galleryInnerNode.Name.ToString(),
                                                                        name = galleryInnerNode.Value.ToString(),
                                                                        sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)
                                                                    });
                                                                }
                                                                else
                                                                {

                                                                    //UserModelList has data, check forum id match so we can skip or add ...
                                                                    //this may get painful with large lists ...
                                                                    var matchingId = GalleryCategoryModelList.FirstOrDefault(_Item => (_Item.id == galleryInnerNode.Name.ToString()));
                                                                    if (matchingId != null)
                                                                    {
                                                                        //match, lets skip ...
                                                                    }
                                                                    else
                                                                    {

                                                                        //nothing found, lets add new forum top level category...
                                                                        GalleryCategoryModelList.Add(new EnjinExportTool.ExportGalleryModels.GalleryCategoryModel()
                                                                        {
                                                                            id = galleryInnerNode.Name.ToString(),
                                                                            name = galleryInnerNode.Value.ToString(),
                                                                            sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)
                                                                        });

                                                                    }

                                                                }

                                                                #endregion do GalleryCatergoryModelList checks

                                                                if (backgroundWorker.CancellationPending == true)
                                                                {
                                                                    e.Cancel = true;
                                                                    return;
                                                                }

                                                                #region add to GalleryItemModelList model

                                                                JObject galleryItemJson =
                                                                new JObject(
                                                                    new JProperty("jsonrpc", "2.0"),
                                                                    new JProperty("id", "123456789"),
                                                                    new JProperty("method", "Gallery.getAlbum"),
                                                                    new JProperty("params",
                                                                        new JObject(new JProperty("session_id", session_id.ToString()),
                                                                            new JProperty("preset_id", galley_preset_id.ToString()),
                                                                            new JProperty("album_id", galleryInnerNode.Name.ToString())))
                                                                    );

                                                                int galleryItemCount = 0;
                                                                string galleryItemJsonRequestJSON = null;
                                                                HttpWebRequest galleryItemJsonRequest = (HttpWebRequest)WebRequest.Create(base_enjin_url + base_api_slug);
                                                                galleryItemJsonRequest.CookieContainer = new CookieContainer();
                                                                galleryItemJsonRequest.Method = WebRequestMethods.Http.Post;
                                                                galleryItemJsonRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                                                                galleryItemJsonRequest.AllowWriteStreamBuffering = true;
                                                                galleryItemJsonRequest.ProtocolVersion = HttpVersion.Version11;
                                                                galleryItemJsonRequest.AllowAutoRedirect = true;
                                                                galleryItemJsonRequest.ContentType = "application/json";
                                                                galleryItemJsonRequest.KeepAlive = true;
                                                                byte[] galleryItemJsonRequestArray = Encoding.ASCII.GetBytes(galleryItemJson.ToString());
                                                                galleryItemJsonRequest.ContentLength = galleryItemJson.ToString().Length;
                                                                Stream galleryItemJsonRequestStream = galleryItemJsonRequest.GetRequestStream(); //open connection
                                                                galleryItemJsonRequestStream.Write(galleryItemJsonRequestArray, 0, galleryItemJson.ToString().Length); // Send the data.
                                                                galleryItemJsonRequestStream.Close();

                                                                if (backgroundWorker.CancellationPending == true)
                                                                {
                                                                    e.Cancel = true;
                                                                    return;
                                                                }

                                                                HttpWebResponse galleryItemJsonRequestResponse = (HttpWebResponse)galleryItemJsonRequest.GetResponse();
                                                                using (StreamReader galleryItemJsonRequestReader = new StreamReader(galleryItemJsonRequestResponse.GetResponseStream()))
                                                                {

                                                                    if (backgroundWorker.CancellationPending == true)
                                                                    {
                                                                        e.Cancel = true;
                                                                        return;
                                                                    }

                                                                    processState[0] = "event";
                                                                    processState[1] = "Exporting " + galleryInnerNode.Value.ToString() + " Gallery";
                                                                    processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                                                    processState[3] = "";
                                                                    processState[4] = "";
                                                                    processState[5] = "";
                                                                    processState[6] = "";
                                                                    processState[7] = "";
                                                                    processState[8] = "";
                                                                    processState[9] = "";
                                                                    processState[10] = "";
                                                                    backgroundWorker.ReportProgress(68, processState);

                                                                    galleryItemJsonRequestJSON = galleryItemJsonRequestReader.ReadToEnd();
                                                                    galleryItemJsonRequestReader.Close();

                                                                    // Write the string to a file.
                                                                    System.IO.StreamWriter galleryItemJsonFile = new System.IO.StreamWriter(JsonFolderGalleryPath + galleryInnerNode.Name.ToString() + ".json");
                                                                    galleryItemJsonFile.WriteLine(galleryItemJsonRequestJSON);
                                                                    galleryItemJsonFile.Close();

                                                                    Console.WriteLine("API Gallery Item Response: ----------------------- " + galleryItemJsonRequestJSON);

                                                                    if (inif.Read("Application", "backupApiData") == "true")
                                                                    {

                                                                        System.IO.StreamWriter gallery_item_api_file = new System.IO.StreamWriter(migration_folder_json_path + "gallery_" + galleryInnerNode.Name.ToString() + ".json");
                                                                        gallery_item_api_file.WriteLine(galleryItemJsonRequestJSON);
                                                                        gallery_item_api_file.Close();

                                                                    }


                                                                    if (backgroundWorker.CancellationPending == true)
                                                                    {
                                                                        e.Cancel = true;
                                                                        return;
                                                                    }


                                                                    dynamic dynObj = JsonConvert.DeserializeObject(galleryItemJsonRequestJSON.ToString());
                                                                    dynamic dynObj2 = JsonConvert.DeserializeObject(dynObj.result.images.ToString());
                                                                    JArray jarray = JArray.Parse(dynObj2.ToString());


                                                                    //setup folders for album
                                                                    string migration_folder_images_path_gallery = migration_folder_path_root + @"media\images\galleries\" + galleryInnerNode.Name.ToString() + @"\";
                                                                    if (!Directory.Exists(migration_folder_images_path_gallery)) Directory.CreateDirectory(migration_folder_images_path_gallery);


                                                                    #region foreach

                                                                    foreach (var item in jarray)
                                                                    {

                                                                        if (backgroundWorker.CancellationPending == true)
                                                                        {
                                                                            e.Cancel = true;
                                                                            return;
                                                                        }
                                                                        /*
                                                                        Json Rsponse
                                                                
                                                                        "image_id": "",
                                                                        "preset_id": "",
                                                                        "title": "",
                                                                        "description": "",
                                                                        "created": "",
                                                                        "user_id": "",
                                                                        "views": "",
                                                                        "album_id": "",
                                                                        "have_original": "",
                                                                        "ordering": "",
                                                                        "number_comments": "",
                                                                        "comment_cid": "",
                                                                        "url": "",
                                                                        "url_full": "",
                                                                        "url_original": "",
                                                                        "can_modify": 
                                                                 
                                                                        */

                                                                        string image_id = "";
                                                                        string preset_id = "";
                                                                        string title = "";
                                                                        string description = "";
                                                                        string created = "";
                                                                        string user_id = "";
                                                                        string views = "";
                                                                        string album_id = "";
                                                                        string have_original = "";
                                                                        string ordering = "";
                                                                        string number_comments = "";
                                                                        string comment_cid = "";
                                                                        string url = "";
                                                                        string url_full = "";
                                                                        string url_original = "";
                                                                        string can_modify = "";

                                                                        string migration_folder_images_path_thumb_image = "";
                                                                        string migration_folder_images_path_image = "";
                                                                        string migration_folder_images_path_original = "";

                                                                        string responseThumbnailFile = "";
                                                                        string responseMedFile = "";
                                                                        string responseOrgFile = "";

                                                                        string extensionUrlThumb = "";
                                                                        string extensionUrlMed = "";
                                                                        string extensionUrlOrg = "";

                                                                        #region do foreach

                                                                        JObject jObject = JObject.Parse(item.ToString());
                                                                        foreach (var itemjObject in jObject)
                                                                        {


                                                                            if (backgroundWorker.CancellationPending == true)
                                                                            {
                                                                                e.Cancel = true;
                                                                                return;
                                                                            }

                                                                            if (itemjObject.Key.ToString() == "image_id")
                                                                            {

                                                                                image_id = itemjObject.Value.ToString();

                                                                                //setup images for album
                                                                                migration_folder_images_path_thumb_image = migration_folder_images_path_gallery + image_id + @"\thumbnail\";
                                                                                if (!Directory.Exists(migration_folder_images_path_thumb_image)) Directory.CreateDirectory(migration_folder_images_path_thumb_image);

                                                                                //setup url_full for album
                                                                                migration_folder_images_path_image = migration_folder_images_path_gallery + image_id + @"\image\";
                                                                                if (!Directory.Exists(migration_folder_images_path_image)) Directory.CreateDirectory(migration_folder_images_path_image);

                                                                                //setup url_full for album
                                                                                migration_folder_images_path_original = migration_folder_images_path_gallery + image_id + @"\original\";
                                                                                if (!Directory.Exists(migration_folder_images_path_original)) Directory.CreateDirectory(migration_folder_images_path_original);


                                                                            };


                                                                            if (itemjObject.Key.ToString() == "preset_id")
                                                                            {
                                                                                preset_id = itemjObject.Value.ToString();
                                                                            };
                                                                            if (itemjObject.Key.ToString() == "title")
                                                                            {
                                                                                title = itemjObject.Value.ToString();
                                                                            };
                                                                            if (itemjObject.Key.ToString() == "description")
                                                                            {
                                                                                description = itemjObject.Value.ToString();
                                                                            };
                                                                            if (itemjObject.Key.ToString() == "created")
                                                                            {
                                                                                created = itemjObject.Value.ToString();
                                                                            };

                                                                            //owner id
                                                                            if (itemjObject.Key.ToString() == "user_id")
                                                                            {
                                                                                user_id = itemjObject.Value.ToString();
                                                                            };
                                                                            if (itemjObject.Key.ToString() == "views")
                                                                            {
                                                                                views = itemjObject.Value.ToString();
                                                                            };
                                                                            if (itemjObject.Key.ToString() == "album_id")
                                                                            {
                                                                                album_id = itemjObject.Value.ToString();
                                                                            };
                                                                            if (itemjObject.Key.ToString() == "have_original")
                                                                            {
                                                                                have_original = itemjObject.Value.ToString();
                                                                            };
                                                                            if (itemjObject.Key.ToString() == "ordering")
                                                                            {
                                                                                ordering = itemjObject.Value.ToString();
                                                                            };
                                                                            if (itemjObject.Key.ToString() == "number_comments")
                                                                            {
                                                                                number_comments = itemjObject.Value.ToString();
                                                                            };

                                                                            //commenst id link, if extracting comments api data
                                                                            if (itemjObject.Key.ToString() == "comment_cid")
                                                                            {
                                                                                comment_cid = itemjObject.Value.ToString();
                                                                            };

                                                                            //images

                                                                            //thumb
                                                                            if (itemjObject.Key.ToString() == "url")
                                                                            {

                                                                                url = itemjObject.Value.ToString();

                                                                            };

                                                                            //meduim
                                                                            if (itemjObject.Key.ToString() == "url_full")
                                                                            {
                                                                                url_full = itemjObject.Value.ToString();

                                                                            };

                                                                            //original
                                                                            if (itemjObject.Key.ToString() == "url_original")
                                                                            {
                                                                                url_original = itemjObject.Value.ToString();
                                                                            };


                                                                            if (itemjObject.Key.ToString() == "can_modify")
                                                                            {
                                                                                can_modify = itemjObject.Value.ToString();
                                                                            };

                                                                            if (backgroundWorker.CancellationPending == true)
                                                                            {
                                                                                e.Cancel = true;
                                                                                return;
                                                                            }

                                                                        }

                                                                        #endregion foreach


                                                                        #region do images work
                                                                        //do checks here, but for now it works ....

                                                                        if (backgroundWorker.CancellationPending == true)
                                                                        {
                                                                            e.Cancel = true;
                                                                            return;
                                                                        }

                                                                        #region do if
                                                                        if (inif.Read("Application", "dlOriginal") == "true" ||
                                                                            inif.Read("Application", "dlThumbnails") == "true" ||
                                                                            inif.Read("Application", "dlImages") == "true")
                                                                        {


                                                                            if (inif.Read("Application", "dlThumbnails") == "true")
                                                                            {

                                                                                #region dl thumb
                                                                                string newUrlThumb = url.ToString().Replace(@"\", "");
                                                                                extensionUrlThumb = System.IO.Path.GetExtension(newUrlThumb).ToString();
                                                                                responseThumbnailFile = migration_folder_images_path_thumb_image.ToString() + image_id.ToString() + extensionUrlThumb.ToString();

                                                                                using (WebClient client = new WebClient())
                                                                                {

                                                                                    #region event ui
                                                                                    processState[0] = "event";
                                                                                    processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                                                                    processState[3] = "";
                                                                                    processState[4] = "";
                                                                                    processState[5] = "";
                                                                                    processState[6] = "";
                                                                                    processState[7] = "";
                                                                                    processState[8] = "";
                                                                                    processState[9] = "";
                                                                                    processState[10] = "Downloading " + image_id.ToString() + extensionUrlThumb + " (Thumbnail)";
                                                                                    backgroundWorker.ReportProgress(68, processState);

                                                                                    #endregion event ui

                                                                                    client.DownloadFile(newUrlThumb.ToString(), responseThumbnailFile.ToString());
                                                                                    

                                                                                }

                                                                                #endregion dl thumb

                                                                                if (backgroundWorker.CancellationPending == true)
                                                                                {
                                                                                    e.Cancel = true;
                                                                                    return;
                                                                                }

                                                                            }

                                                                            if (inif.Read("Application", "dlImages") == "true")
                                                                            {

                                                                                #region dl med
                                                                                string newUrlMed = url_full.ToString().Replace(@"\", "");
                                                                                extensionUrlMed = System.IO.Path.GetExtension(newUrlMed).ToString();
                                                                                responseMedFile = migration_folder_images_path_image.ToString() + image_id.ToString() + extensionUrlMed.ToString();

                                                                                using (WebClient client = new WebClient())
                                                                                {

                                                                                    #region event ui
                                                                                    processState[0] = "event";
                                                                                    processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                                                                    processState[3] = "";
                                                                                    processState[4] = "";
                                                                                    processState[5] = "";
                                                                                    processState[6] = "";
                                                                                    processState[7] = "";
                                                                                    processState[8] = "";
                                                                                    processState[9] = "";
                                                                                    processState[10] = "Downloading " + image_id.ToString() + extensionUrlMed + " (Meduim Image)";
                                                                                    backgroundWorker.ReportProgress(68, processState);

                                                                                    


                                                                                    #endregion event ui

                                                                                    client.DownloadFile(newUrlMed.ToString(), responseMedFile.ToString());
                                                                                    
                                                                                }

                                                                                #endregion dl med

                                                                                if (backgroundWorker.CancellationPending == true)
                                                                                {
                                                                                    e.Cancel = true;
                                                                                    return;
                                                                                }

                                                                            }

                                                                            if (inif.Read("Application", "dlOriginal") == "true")
                                                                            {

                                                                                #region dl org
                                                                                string newUrlOrg = url_original.ToString().Replace(@"\", "");
                                                                                extensionUrlOrg = System.IO.Path.GetExtension(newUrlOrg).ToString();
                                                                                responseOrgFile = migration_folder_images_path_image.ToString() + image_id.ToString() + extensionUrlOrg.ToString();

                                                                                using (WebClient client = new WebClient())
                                                                                {

                                                                                    #region event ui
                                                                                    processState[0] = "event";
                                                                                    processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                                                                    processState[3] = "";
                                                                                    processState[4] = "";
                                                                                    processState[5] = "";
                                                                                    processState[6] = "";
                                                                                    processState[7] = "";
                                                                                    processState[8] = "";
                                                                                    processState[9] = "";
                                                                                    processState[10] = "Downloading " + image_id.ToString() + extensionUrlOrg + " (Original)";
                                                                                    backgroundWorker.ReportProgress(68, processState);



                                                                                    #endregion event ui

                                                                                    client.DownloadFile(newUrlOrg.ToString(), responseOrgFile.ToString());
                                                                                    
                                                                                }

                                                                                #endregion dl org

                                                                                if (backgroundWorker.CancellationPending == true)
                                                                                {
                                                                                    e.Cancel = true;
                                                                                    return;
                                                                                }

                                                                            }


                                                                            processState[0] = "event";
                                                                            processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                                                            processState[3] = "";
                                                                            processState[4] = "";
                                                                            processState[5] = "";
                                                                            processState[6] = "";
                                                                            processState[7] = "";
                                                                            processState[8] = "";
                                                                            processState[9] = "";
                                                                            processState[10] = "";
                                                                            backgroundWorker.ReportProgress(68, processState);

                                                                            if (backgroundWorker.CancellationPending == true)
                                                                            {
                                                                                e.Cancel = true;
                                                                                return;
                                                                            }

                                                                        }
                                                                        else
                                                                        {


                                                                            #region error ui
                                                                            processState[0] = "error";
                                                                            processState[1] = "Skipped Gallery Download ... Check your settings";
                                                                            processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                                                            backgroundWorker.ReportProgress(70, processState);

                                                                            Thread.Sleep(1000);

                                                                            #endregion error ui

                                                                            #region logging

                                                                            try
                                                                            {


                                                                                //Logging                        

                                                                                using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                                                                {
                                                                                    string time = DateTime.Now.ToString();
                                                                                    sw.WriteLine("LogStart -------------------------------------------------------------------");
                                                                                    sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                                                                    sw.WriteLine("LogType Skip");
                                                                                    sw.WriteLine("LogMessage Skipped Gallery Export due to settings");
                                                                                    sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                                                                }
                                                                            }
                                                                            catch (Exception log_error)
                                                                            {
                                                                                //super fail

                                                                            }

                                                                            #endregion logging


                                                                        }
                                                                        #endregion do if

                                                                        #endregion do images work


                                                                        #region do GalleryItemModelList checks

                                                                        //add to GalleryItemModelList after id checks
                                                                        //image_id

                                                                        if (GalleryCategoryItemModelList.Count() == 0 ||
                                                                            GalleryCategoryItemModelList.Count() == null)
                                                                        {
                                                                            //empty UserModelList, add to Model
                                                                            GalleryCategoryItemModelList.Add(new EnjinExportTool.ExportGalleryModels.GalleryItemModel()
                                                                            {
                                                                                image_id = image_id,
                                                                                preset_id = preset_id,
                                                                                title = title,
                                                                                description = description,
                                                                                created = created,
                                                                                user_id = user_id,
                                                                                views = views,
                                                                                album_id = album_id,
                                                                                have_original = have_original,
                                                                                ordering = ordering,
                                                                                number_comments = number_comments,
                                                                                comment_cid = comment_cid,
                                                                                url = url,
                                                                                url_full = url_full,
                                                                                url_original = url_original,
                                                                                can_modify = false,
                                                                                thumbImagePath = "media/images/galleries/" + album_id + "/" + image_id.ToString() + "/thumbnail/" + image_id.ToString() + extensionUrlThumb,
                                                                                originalImagePath = "media/images/galleries/" + album_id + "/" + image_id.ToString() + "/original/" + image_id.ToString() + extensionUrlOrg,
                                                                                meduimImagePath = "media/images/galleries/" + album_id + "/" + image_id.ToString() + "/image/" + image_id.ToString() + extensionUrlMed,
                                                                                sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)
                                                                            });
                                                                        }
                                                                        else
                                                                        {

                                                                            //GalleryCategoryItemModelList has data, check id match so we can skip or add ...
                                                                            //this may get painful with large lists ...
                                                                            var matchingId = GalleryCategoryItemModelList.FirstOrDefault(_Item => (_Item.image_id == image_id));
                                                                            if (matchingId != null)
                                                                            {
                                                                                //match, lets skip ...
                                                                            }
                                                                            else
                                                                            {

                                                                                //nothing found, lets add new image...
                                                                                GalleryCategoryItemModelList.Add(new EnjinExportTool.ExportGalleryModels.GalleryItemModel()
                                                                                {
                                                                                    image_id = image_id,
                                                                                    preset_id = preset_id,
                                                                                    title = title,
                                                                                    description = description,
                                                                                    created = created,
                                                                                    user_id = user_id,
                                                                                    views = views,
                                                                                    album_id = album_id,
                                                                                    have_original = have_original,
                                                                                    ordering = ordering,
                                                                                    number_comments = number_comments,
                                                                                    comment_cid = comment_cid,
                                                                                    url = url,
                                                                                    url_full = url_full,
                                                                                    url_original = url_original,
                                                                                    can_modify = false,
                                                                                    thumbImagePath = "media/images/galleries/" + album_id + "/" + image_id.ToString() + "/thumbnail/" + image_id.ToString() + extensionUrlThumb,
                                                                                    originalImagePath = "media/images/galleries/" + album_id + "/" + image_id.ToString() + "/original/" + image_id.ToString() + extensionUrlOrg,
                                                                                    meduimImagePath = "media/images/galleries/" + album_id + "/" + image_id.ToString() + "/image/" + image_id.ToString() + extensionUrlMed,
                                                                                    sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)
                                                                                });

                                                                            }

                                                                        }

                                                                        #endregion do GalleryItemModelList checks



                                                                    }


                                                                    #endregion foreach


                                                                }


                                                                #endregion add to GalleryItemModelList model


                                                                #endregion add to gallery model

                                                            }

                                                        }

                                                    }

                                                }


                                                #endregion do json response work

                                                


                                            }

                                            #endregion do api gallery inner region

                                            #region do model processing & checks

                                            #region model valid items

                                            if (GalleryCategoryModelList.Count() == 0 &&
                                                GalleryCategoryItemModelList.Count() == 0)
                                            {

                                                //All list need at least 1 count on each ...
                                                //Update UI with error message state
                                                #region logging

                                                try
                                                {


                                                    //Logging                        

                                                    using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                                    {
                                                        string time = DateTime.Now.ToString();
                                                        sw.WriteLine("LogStart -------------------------------------------------------------------");
                                                        sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                                        sw.WriteLine("LogType Exception");
                                                        sw.WriteLine("LogMessage Gallery Catergory Model Lists are empty");
                                                        sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                                    }
                                                }
                                                catch (Exception log_error)
                                                {
                                                    //super fail

                                                }

                                                #endregion logging


                                            }
                                            else
                                            {

                                                XmlWriter xmlWriter = XmlWriter.Create(migration_folder_xml_path + "galleries.xml");
                                                xmlWriter.WriteStartDocument();
                                                xmlWriter.WriteStartElement("galleries");

                                                #region do xml writes for GalleryCategoryModelList items

                                                xmlWriter.WriteStartElement("categories");

                                                foreach (var GalleryCategoryModelListItem in GalleryCategoryModelList)
                                                {

                                                    xmlWriter.WriteStartElement("category");
                                                    xmlWriter.WriteStartElement("id");
                                                    xmlWriter.WriteString(GalleryCategoryModelListItem.id);
                                                    xmlWriter.WriteEndElement();
                                                    xmlWriter.WriteStartElement("name");
                                                    xmlWriter.WriteString(GalleryCategoryModelListItem.name);
                                                    xmlWriter.WriteEndElement();
                                                    xmlWriter.WriteEndElement();
                                                }

                                                xmlWriter.WriteEndElement();

                                                #endregion do xml writes for GalleryCategoryModelList items

                                                #region do xml writes for GalleryCategoryItemModelList items

                                                xmlWriter.WriteStartElement("images");

                                                foreach (var GalleryCategoryItemModelListItem in GalleryCategoryItemModelList)
                                                {


                                                    /*
                                                    Model Rsponse
                                                                
                                                    "image_id": "",
                                                    "preset_id": "",
                                                    "title": "",
                                                    "description": "",
                                                    "created": "",
                                                    "user_id": "",
                                                    "views": "",
                                                    "album_id": "",
                                                    "have_original": "",
                                                    "ordering": "",
                                                    "number_comments": "",
                                                    "comment_cid": "",
                                                    "url": "",
                                                    "url_full": "",
                                                    "url_original": "",
                                                    "can_modify": 
                                                                 
                                                    */


                                                    xmlWriter.WriteStartElement("image");

                                                    xmlWriter.WriteStartElement("image_id");
                                                    xmlWriter.WriteString(GalleryCategoryItemModelListItem.image_id);
                                                    xmlWriter.WriteEndElement();

                                                    xmlWriter.WriteStartElement("preset_id");
                                                    xmlWriter.WriteString(GalleryCategoryItemModelListItem.preset_id);
                                                    xmlWriter.WriteEndElement();

                                                    xmlWriter.WriteStartElement("title");
                                                    xmlWriter.WriteString(GalleryCategoryItemModelListItem.title);
                                                    xmlWriter.WriteEndElement();

                                                    xmlWriter.WriteStartElement("description");
                                                    xmlWriter.WriteString(GalleryCategoryItemModelListItem.description);
                                                    xmlWriter.WriteEndElement();

                                                    xmlWriter.WriteStartElement("created");
                                                    xmlWriter.WriteString(GalleryCategoryItemModelListItem.created);
                                                    xmlWriter.WriteEndElement();

                                                    xmlWriter.WriteStartElement("user_id");
                                                    xmlWriter.WriteString(GalleryCategoryItemModelListItem.user_id);
                                                    xmlWriter.WriteEndElement();

                                                    xmlWriter.WriteStartElement("views");
                                                    xmlWriter.WriteString(GalleryCategoryItemModelListItem.views);
                                                    xmlWriter.WriteEndElement();

                                                    xmlWriter.WriteStartElement("album_id");
                                                    xmlWriter.WriteString(GalleryCategoryItemModelListItem.album_id);
                                                    xmlWriter.WriteEndElement();

                                                    xmlWriter.WriteStartElement("have_original");
                                                    xmlWriter.WriteString(GalleryCategoryItemModelListItem.have_original);
                                                    xmlWriter.WriteEndElement();

                                                    xmlWriter.WriteStartElement("ordering");
                                                    xmlWriter.WriteString(GalleryCategoryItemModelListItem.ordering);
                                                    xmlWriter.WriteEndElement();

                                                    xmlWriter.WriteStartElement("number_comments");
                                                    xmlWriter.WriteString(GalleryCategoryItemModelListItem.number_comments);
                                                    xmlWriter.WriteEndElement();

                                                    xmlWriter.WriteStartElement("comment_cid");
                                                    xmlWriter.WriteString(GalleryCategoryItemModelListItem.comment_cid);
                                                    xmlWriter.WriteEndElement();

                                                    xmlWriter.WriteStartElement("url");
                                                    xmlWriter.WriteString(GalleryCategoryItemModelListItem.url);
                                                    xmlWriter.WriteEndElement();

                                                    xmlWriter.WriteStartElement("url_full");
                                                    xmlWriter.WriteString(GalleryCategoryItemModelListItem.url_full);
                                                    xmlWriter.WriteEndElement();

                                                    xmlWriter.WriteStartElement("url_original");
                                                    xmlWriter.WriteString(GalleryCategoryItemModelListItem.url_original);
                                                    xmlWriter.WriteEndElement();

                                                    xmlWriter.WriteStartElement("thumbImagePath");
                                                    xmlWriter.WriteString(GalleryCategoryItemModelListItem.thumbImagePath);
                                                    xmlWriter.WriteEndElement();

                                                    xmlWriter.WriteStartElement("meduimImagePath");
                                                    xmlWriter.WriteString(GalleryCategoryItemModelListItem.meduimImagePath);
                                                    xmlWriter.WriteEndElement();

                                                    xmlWriter.WriteStartElement("originalImagePath");
                                                    xmlWriter.WriteString(GalleryCategoryItemModelListItem.originalImagePath);
                                                    xmlWriter.WriteEndElement();

                                                    xmlWriter.WriteEndElement();
                                                }

                                                xmlWriter.WriteEndElement();

                                                #endregion do xml writes for GalleryCategoryItemModelList items


                                                xmlWriter.WriteEndDocument();
                                                xmlWriter.Close();



                                            }

                                            #endregion model valid items

                                            #region do error model processing & checks
                                            if (GalleryErrorEventModelList.Count() == 0)
                                            {


                                                //no errors reported ... hmmm...
                                                #region logging

                                                try
                                                {


                                                    //Logging                        

                                                    using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                                    {
                                                        string time = DateTime.Now.ToString();
                                                        sw.WriteLine("LogStart -------------------------------------------------------------------");
                                                        sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                                        sw.WriteLine("LogType Exception");
                                                        sw.WriteLine("LogMessage No errors reported when exporting the gallery");
                                                        sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                                    }
                                                }
                                                catch (Exception log_error)
                                                {
                                                    //super fail

                                                }

                                                #endregion logging


                                            }
                                            else
                                            {


                                                XmlWriter xmlWriter = XmlWriter.Create(migration_folder_xml_path + "gallery_errors.xml");
                                                xmlWriter.WriteStartDocument();
                                                xmlWriter.WriteStartElement("errors");

                                                #region do xml writes for GalleryErrorEventModelList items

                                                xmlWriter.WriteStartElement("error");

                                                foreach (var ErrorEventModelListItem in GalleryErrorEventModelList)
                                                {

                                                    xmlWriter.WriteStartElement("report");
                                                    xmlWriter.WriteAttributeString("type", ErrorEventModelListItem.type);
                                                    xmlWriter.WriteString(ErrorEventModelListItem.message);
                                                    xmlWriter.WriteEndElement();

                                                }

                                                xmlWriter.WriteEndElement();


                                                xmlWriter.WriteEndDocument();
                                                xmlWriter.Close();



                                            }
                                                #endregion do error model processing & checks

                                            #endregion do model processing & checks

                                            #endregion do model processing & checks
                                            

                                        }
                                        catch (Exception apiGalleryError)
                                        {

                                            #region error ui
                                            processState[0] = "error";
                                            processState[1] = "Export Gallery failed";
                                            processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                            backgroundWorker.ReportProgress(100, processState);

                                            backgroundWorker.CancelAsync();
                                            backgroundWorker.Dispose();

                                            if (backgroundWorker.CancellationPending == true)
                                            {
                                                e.Cancel = true;
                                                return;
                                            }

                                            #endregion error ui

                                            #region logging

                                            try
                                            {


                                                //Logging                        

                                                using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                                {
                                                    string time = DateTime.Now.ToString();
                                                    sw.WriteLine("LogStart -------------------------------------------------------------------");
                                                    sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                                    sw.WriteLine("LogType Exception");
                                                    sw.WriteLine("LogMessage " + apiGalleryError);
                                                    sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                                }
                                            }
                                            catch (Exception log_error)
                                            {
                                                //super fail

                                            }

                                            #endregion logging

                                        }

                                        #endregion do try api gallery

                                    }
                                    else
                                    {

                                        #region error ui
                                        processState[0] = "error";
                                        processState[1] = "Gallery Preset ID invalid ... skipping ...";
                                        processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                        backgroundWorker.ReportProgress(80, processState);

                                        backgroundWorker.CancelAsync();
                                        backgroundWorker.Dispose();

                                        if (backgroundWorker.CancellationPending == true)
                                        {
                                            e.Cancel = true;
                                            return;
                                        }

                                        #endregion error ui

                                        #region logging

                                        try
                                        {


                                            //Logging                        

                                            using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                            {
                                                string time = DateTime.Now.ToString();
                                                sw.WriteLine("LogStart -------------------------------------------------------------------");
                                                sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                                sw.WriteLine("LogType Exception");
                                                sw.WriteLine("LogMessage Gallery failed ...");
                                                sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                            }
                                        }
                                        catch (Exception log_error)
                                        {
                                            //super fail

                                        }

                                        #endregion logging

                                    }

                                    #endregion do gallery work

                                }
                                else
                                {

                                    #region error ui
                                    processState[0] = "error";
                                    processState[1] = "No Download Types configured";
                                    processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                    backgroundWorker.ReportProgress(70, processState);

                                    Thread.Sleep(1200);

                                    if (backgroundWorker.CancellationPending == true)
                                    {
                                        e.Cancel = true;
                                        return;
                                    }

                                    #endregion error ui

                                    #region logging

                                    try
                                    {


                                        //Logging                        

                                        using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                        {
                                            string time = DateTime.Now.ToString();
                                            sw.WriteLine("LogStart -------------------------------------------------------------------");
                                            sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                            sw.WriteLine("LogType Exception");
                                            sw.WriteLine("LogMessage Gallery failed ...");
                                            sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                        }
                                    }
                                    catch (Exception log_error)
                                    {
                                        //super fail

                                    }

                                    #endregion logging

                                }

                            }


                            #endregion do api gallery
                            
                            #region do api user posts work

                            if (backgroundWorker.CancellationPending == true)
                            {
                                e.Cancel = true;
                                return;
                            }

                            string _usersJson = null;
                            HttpWebRequest pageRequest = (HttpWebRequest)WebRequest.Create(base_enjin_url + tags_api_slug);
                            HttpWebResponse pageresponse = (HttpWebResponse)pageRequest.GetResponse();
                            using (StreamReader pageresponseData = new StreamReader(pageresponse.GetResponseStream()))
                            {
                                _usersJson = pageresponseData.ReadToEnd();
                                pageresponseData.Close();

                                System.IO.StreamWriter file = new System.IO.StreamWriter(ApplicationPath + "/users.json");
                                file.WriteLine(_usersJson);
                                file.Close();


                                if (inif.Read("Application", "backupApiData") == "true")
                                {

                                    System.IO.StreamWriter users_api_json_file = new System.IO.StreamWriter(migration_folder_json_path + "users.json");
                                    users_api_json_file.WriteLine(_usersJson);
                                    users_api_json_file.Close();

                                }


                                JObject results = JObject.Parse(_usersJson);

                                string usersCount = results["users"].Count().ToString();
                                int userCount = 0;

                                processState[0] = "event";
                                processState[1] = usersCount + " Users to process";
                                processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                processState[3] = "";
                                processState[4] = "";
                                processState[5] = "";
                                processState[6] = "";
                                processState[7] = "";
                                processState[8] = "";
                                processState[9] = "";
                                processState[10] = "";
                                backgroundWorker.ReportProgress(12, processState);
                                Thread.Sleep(2500);

                                if (backgroundWorker.CancellationPending == true)
                                {
                                    e.Cancel = true;
                                    return;
                                }

                                if (usersCount == "0" || usersCount == null)
                                {

                                    #region error ui
                                    processState[0] = "error";
                                    processState[1] = "User Data was empty...";
                                    processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                    backgroundWorker.ReportProgress(100, processState);

                                    backgroundWorker.CancelAsync();
                                    backgroundWorker.Dispose();

                                    if (backgroundWorker.CancellationPending == true)
                                    {
                                        e.Cancel = true;
                                        return;
                                    }

                                    #endregion error ui

                                    #region logging

                                    try
                                    {


                                        //Logging                        

                                        using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                        {
                                            string time = DateTime.Now.ToString();
                                            sw.WriteLine("LogStart -------------------------------------------------------------------");
                                            sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                            sw.WriteLine("LogType Exception");
                                            sw.WriteLine("LogMessage User count was 0");
                                            sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                        }
                                    }
                                    catch (Exception log_error)
                                    {
                                        //super fail

                                    }

                                    #endregion logging

                                }
                                else
                                {

                                    //int Lists (data holders)
                                    List<EnjinExportTool.ExportModels.UserModel> UserModelList = new List<EnjinExportTool.ExportModels.UserModel>();
                                    List<EnjinExportTool.ExportModels.PostModel> PostModelList = new List<EnjinExportTool.ExportModels.PostModel>();
                                    List<EnjinExportTool.ExportModels.ForumModel> ForumModelList = new List<EnjinExportTool.ExportModels.ForumModel>();
                                    List<EnjinExportTool.ExportModels.CategoryModel> CategoryModelList = new List<EnjinExportTool.ExportModels.CategoryModel>();
                                    List<EnjinExportTool.ExportModels.ThreadModel> ThreadModelList = new List<EnjinExportTool.ExportModels.ThreadModel>();
                                    List<EnjinExportTool.ExportModels.ErrorEventModel> ErrorEventModelList = new List<EnjinExportTool.ExportModels.ErrorEventModel>();

                                    #region foreach user
                                    foreach (var result in results["users"])
                                    {

                                        if (backgroundWorker.CancellationPending == true)
                                        {
                                            e.Cancel = true;
                                            return;
                                        }

                                        string resultData = "[{" + result.ToString() + "}]";
                                        JArray array = JArray.Parse(resultData);

                                        foreach (JObject content in array.Children<JObject>())
                                        {
                                            foreach (JProperty prop in content.Properties())
                                            {

                                                if (backgroundWorker.CancellationPending == true)
                                                {
                                                    e.Cancel = true;
                                                    return;
                                                }


                                                //set username for linking
                                                string userName = "";
                                                foreach (JObject userNameNode in prop.Children<JObject>())
                                                {
                                                    foreach (JProperty userNameNodeItem in userNameNode.Properties())
                                                    {

                                                        //check if user name string
                                                        if (userNameNodeItem.Name == "username")
                                                        {

                                                            userName = userNameNodeItem.Value.ToString();

                                                        }


                                                    }

                                                }


                                                if (userName != null)
                                                {

                                                    #region do userPostsJson work


                                                    #region add user model

                                                    #region do UserModelList checks

                                                    //add to UserModelList after id checks
                                                    //user_id 
                                                    //user_name

                                                    if (UserModelList.Count() == 0 ||
                                                        UserModelList.Count() == null)
                                                    {
                                                        //empty UserModelList, add to Model
                                                        UserModelList.Add(new EnjinExportTool.ExportModels.UserModel()
                                                        {
                                                            user_id = prop.Name,
                                                            user_name = userName,
                                                            sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)
                                                        });
                                                    }
                                                    else
                                                    {

                                                        //UserModelList has data, check forum id match so we can skip or add ...
                                                        //this may get painful with large lists ...
                                                        var matchingId = UserModelList.FirstOrDefault(_Item => (_Item.user_name == userName));
                                                        if (matchingId != null)
                                                        {
                                                            //match, lets skip ...
                                                        }
                                                        else
                                                        {

                                                            //nothing found, lets add new forum top level category...
                                                            UserModelList.Add(new EnjinExportTool.ExportModels.UserModel()
                                                            {
                                                                user_id = prop.Name,
                                                                user_name = userName,
                                                                sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)
                                                            });

                                                        }

                                                    }

                                                    #endregion do ForumModelList checks

                                                    #endregion add user model

                                                    if (backgroundWorker.CancellationPending == true)
                                                    {
                                                        e.Cancel = true;
                                                        return;
                                                    }

                                                    JObject userPostsJson =
                                                    new JObject(
                                                        new JProperty("id", "123456789"),
                                                        new JProperty("jsonrpc", "2.0"),
                                                        new JProperty("params",
                                                        new JObject(new JProperty("session_id", session_id.ToString()),
                                                            new JProperty("user_id", prop.Name))),
                                                        new JProperty("method", "Profile.getPosts"));


                                                    string userPostsJsonRequestJSON = null;
                                                    HttpWebRequest userPostsJsonRequest = (HttpWebRequest)WebRequest.Create(base_enjin_url + base_api_slug);
                                                    userPostsJsonRequest.CookieContainer = new CookieContainer();
                                                    userPostsJsonRequest.Method = WebRequestMethods.Http.Post;
                                                    userPostsJsonRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                                                    userPostsJsonRequest.AllowWriteStreamBuffering = true;
                                                    userPostsJsonRequest.ProtocolVersion = HttpVersion.Version11;
                                                    userPostsJsonRequest.AllowAutoRedirect = true;
                                                    userPostsJsonRequest.ContentType = "application/json";
                                                    userPostsJsonRequest.KeepAlive = true;
                                                    byte[] userPostsJsonRequestArray = Encoding.ASCII.GetBytes(userPostsJson.ToString());
                                                    userPostsJsonRequest.ContentLength = userPostsJson.ToString().Length;
                                                    Stream userPostsJsonRequestStream = userPostsJsonRequest.GetRequestStream(); //open connection
                                                    userPostsJsonRequestStream.Write(userPostsJsonRequestArray, 0, userPostsJson.ToString().Length); // Send the data.
                                                    userPostsJsonRequestStream.Close();

                                                    HttpWebResponse userPostsJsonRequestResponse = (HttpWebResponse)userPostsJsonRequest.GetResponse();
                                                    using (StreamReader userPostsJsonRequestReader = new StreamReader(userPostsJsonRequestResponse.GetResponseStream()))
                                                    {

                                                        userCount++;

                                                        processState[0] = "event";
                                                        processState[1] = "Exporting Data for " + userName + " (" + userCount.ToString() + " / " + usersCount + ") ... ";
                                                        processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                                        processState[3] = "";
                                                        processState[4] = "";
                                                        processState[5] = "";
                                                        processState[6] = "";
                                                        processState[7] = "";
                                                        processState[8] = "";
                                                        processState[9] = "";
                                                        processState[10] = "";
                                                        backgroundWorker.ReportProgress(20, processState);

                                                        userPostsJsonRequestJSON = userPostsJsonRequestReader.ReadToEnd();
                                                        userPostsJsonRequestReader.Close();

                                                        // Write the string to a file.
                                                        System.IO.StreamWriter fileUserJsonPosts = new System.IO.StreamWriter(JsonFolderUsersPath + prop.Name + ".json");
                                                        fileUserJsonPosts.WriteLine(userPostsJsonRequestJSON);
                                                        fileUserJsonPosts.Close();

                                                        if (inif.Read("Application", "backupApiData") == "true")
                                                        {                                                            

                                                            System.IO.StreamWriter users_api_json_user_file = new System.IO.StreamWriter(migration_folder_json_path + prop.Name + ".json");
                                                            users_api_json_user_file.WriteLine(userPostsJsonRequestJSON);
                                                            users_api_json_user_file.Close();

                                                        }


                                                        if (backgroundWorker.CancellationPending == true)
                                                        {
                                                            e.Cancel = true;
                                                            return;
                                                        }

                                                        try
                                                        {

                                                            string userPostsJsonRequestJSONData = "[" + userPostsJsonRequestJSON.ToString() + "]";
                                                            JArray userPostsJsonRequestJSONArray = JArray.Parse(userPostsJsonRequestJSONData);

                                                            foreach (JObject userPostsJsonContent in userPostsJsonRequestJSONArray.Children<JObject>())
                                                            {
                                                                foreach (JProperty userPostsJsonNode in userPostsJsonContent.Properties())
                                                                {

                                                                    //Console.WriteLine(userPostsJsonNode.Children()["communities"].ToString());
                                                                    foreach (JObject communitiesNodeContent in userPostsJsonNode.Children<JObject>()["communities"])
                                                                    {
                                                                        foreach (JProperty communitiesNode in communitiesNodeContent.Properties())
                                                                        {

                                                                            if (communitiesNode.Name.ToString() == site_id)
                                                                            {

                                                                                //verified user, can process futher. dont wate time if non existent site user
                                                                                //find pages count ... may need to to iterate over a few pages (more url requests)

                                                                                try
                                                                                {


                                                                                    if (backgroundWorker.CancellationPending == true)
                                                                                    {
                                                                                        e.Cancel = true;
                                                                                        return;
                                                                                    }


                                                                                    int pagesCount = 0;
                                                                                    IList<EnjinUserPostsModel.Result> _EnjinUserPostsModel = userPostsJsonRequestJSONArray.Select(p => new EnjinUserPostsModel.Result
                                                                                    {
                                                                                        pages = (string)p["result"]["pages"]

                                                                                    }).ToList();



                                                                                    try
                                                                                    {

                                                                                        pagesCount = Convert.ToInt32(_EnjinUserPostsModel[0].pages.ToString());

                                                                                        if (pagesCount > 1)
                                                                                        {


                                                                                            for (int i = 0; i < pagesCount; i++)
                                                                                            {

                                                                                                if (backgroundWorker.CancellationPending == true)
                                                                                                {
                                                                                                    e.Cancel = true;
                                                                                                    return;
                                                                                                }

                                                                                                //check if first page and use the data from userPostsJsonRequestJSONArray
                                                                                                if (i == 0)
                                                                                                {


                                                                                                    if (backgroundWorker.CancellationPending == true)
                                                                                                    {
                                                                                                        e.Cancel = true;
                                                                                                        return;
                                                                                                    }

                                                                                                    #region do work on first request using userPostsJsonRequestJSONArray
                                                                                                    //already got the data to inside userPostsJsonRequestJSONArray

                                                                                                    int posts = 0;

                                                                                                    if (backgroundWorker.CancellationPending == true)
                                                                                                    {
                                                                                                        e.Cancel = true;
                                                                                                        return;
                                                                                                    }

                                                                                                    dynamic dynObj = JsonConvert.DeserializeObject(userPostsJsonRequestJSON.ToString());
                                                                                                    dynamic dynObj2 = JsonConvert.DeserializeObject(dynObj.result.posts.ToString());
                                                                                                    JArray jarray = JArray.Parse(dynObj2.ToString());

                                                                                                    #region foreach

                                                                                                    foreach (var item in jarray)
                                                                                                    {


                                                                                                        if (backgroundWorker.CancellationPending == true)
                                                                                                        {
                                                                                                            e.Cancel = true;
                                                                                                            return;
                                                                                                        }

                                                                                                        #region result

                                                                                                            /*
                                                                                            
                                                                                                                    string preset_id = "";
                                                                                                                    string is_thread = "";
                                                                                                                    string post_content = "";
                                                                                                                    string post_time = "";
                                                                                                                    string post_votes = "";
                                                                                                                    string post_id = "";
                                                                                                                    string total_posts = "";
                                                                                                                    string thread_id = "";
                                                                                                                    string thread_subject = "";
                                                                                                                    string forum_id = "";
                                                                                                                    string thread_user_id = "";
                                                                                                                    string enable_voting = "";
                                                                                                                    string site_id = "";
                                                                                                                    string name = "";
                                                                                                                    string forum_name = "";
                                                                                                                    string disable_voting = "";
                                                                                                                    string users_see_own_threads = "";
                                                                                                                    string forum_preset_id = "";
                                                                                                                    string category_id = "";
                                                                                                                    string category_name = "";
                                                                                                                    string domain = "";
                                                                                                                    string page = "";
                                                                                                                    string url = "";
                                                                                            
                                                                                                                    if(itemjObject.Key.ToString() == "preset_id"){
                                                                                                                    }else if(itemjObject.Key.ToString() == "is_thread"){
                                                                                                                    }else if(itemjObject.Key.ToString() == "post_content"){
                                                                                                                    }else if(itemjObject.Key.ToString() == "post_time"){
                                                                                                                    }else if(itemjObject.Key.ToString() == "post_votes"){
                                                                                                                    }else if(itemjObject.Key.ToString() == "post_id"){
                                                                                                                    }else if(itemjObject.Key.ToString() == "total_posts"){
                                                                                                                    }else if(itemjObject.Key.ToString() == "thread_id"){
                                                                                                                    }else if(itemjObject.Key.ToString() == "thread_subject"){
                                                                                                                    }else if(itemjObject.Key.ToString() == "forum_id"){
                                                                                                                    }else if(itemjObject.Key.ToString() == "thread_user_id"){
                                                                                                                    }else if(itemjObject.Key.ToString() == "enable_voting"){
                                                                                                                    }else if(itemjObject.Key.ToString() == "site_id"){
                                                                                                                    }else if(itemjObject.Key.ToString() == "name"){
                                                                                                                    }else if(itemjObject.Key.ToString() == "forum_name"){
                                                                                                                    }else if(itemjObject.Key.ToString() == "disable_voting"){
                                                                                                                    }else if(itemjObject.Key.ToString() == "users_see_own_threads"){
                                                                                                                    }else if(itemjObject.Key.ToString() == "forum_preset_id"){
                                                                                                                    }else if(itemjObject.Key.ToString() == "category_id"){
                                                                                                                    }else if(itemjObject.Key.ToString() == "category_name"){
                                                                                                                    }else if(itemjObject.Key.ToString() == "domain"){
                                                                                                                    }else if(itemjObject.Key.ToString() == "page"){
                                                                                                                    }else if(itemjObject.Key.ToString() == "url"){
                                                                                                                    }
                                                                                            
                                                                                                                */

                                                                                                                /* 
                                                                                                                preset_id
                                                                                                                1148623
                                                                                                                is_thread
                                                                                                                0
                                                                                                                post_content
                                                                                                                post content will be rendered here this is just sample text
                                                                                                                post_time
                                                                                                                1298698176
                                                                                                                post_votes
                                                                                                                0
                                                                                                                post_id
                                                                                                                2735183
                                                                                                                total_posts
                                                                                                                28
                                                                                                                thread_id
                                                                                                                574659
                                                                                                                thread_subject
                                                                                                                Server Is Offline
                                                                                                                forum_id
                                                                                                                316857
                                                                                                                thread_user_id
                                                                                                                201737
                                                                                                                enable_voting
                                                                                                                1
                                                                                                                site_id
                                                                                                                55483
                                                                                                                name
                                                                                                                Minecraft Oblivion
                                                                                                                forum_name
                                                                                                                News & Announcements
                                                                                                                disable_voting
                                                                                                                0
                                                                                                                users_see_own_threads
                                                                                                                0
                                                                                                                forum_preset_id
                                                                                                                1148623
                                                                                                                category_id
                                                                                                                132280
                                                                                                                category_name
                                                                                                                Server
                                                                                                                domain
                                                                                                                http://www.theurl.com
                                                                                                                page
                                                                                                                forum
                                                                                                                url
                                                                                                                http://www.theurl.com/
                                                                                                        */


                                                                                                        #endregion  result

                                                                                                        string preset_id = "";
                                                                                                        string is_thread = "";
                                                                                                        string post_content = "";
                                                                                                        string post_time = "";
                                                                                                        string post_votes = "";
                                                                                                        string post_id = "";
                                                                                                        string total_posts = "";
                                                                                                        string thread_id = "";
                                                                                                        string thread_subject = "";
                                                                                                        string forum_id = "";
                                                                                                        string thread_user_id = "";
                                                                                                        string enable_voting = "";
                                                                                                        string _site_id = "";
                                                                                                        string name = "";
                                                                                                        string forum_name = "";
                                                                                                        string disable_voting = "";
                                                                                                        string users_see_own_threads = "";
                                                                                                        string forum_preset_id = "";
                                                                                                        string category_id = "";
                                                                                                        string category_name = "";
                                                                                                        string domain = "";
                                                                                                        string page = "";
                                                                                                        string url = "";


                                                                                                        #region do foreach

                                                                                                        JObject jObject = JObject.Parse(item.ToString());
                                                                                                        foreach (var itemjObject in jObject)
                                                                                                        {

                                                                                                            if (backgroundWorker.CancellationPending == true)
                                                                                                            {
                                                                                                                e.Cancel = true;
                                                                                                                return;
                                                                                                            }

                                                                                                            #region do foreach
                                                                                                            if (itemjObject.Key.ToString() == "preset_id")
                                                                                                            {
                                                                                                                preset_id = itemjObject.Value.ToString();
                                                                                                            }
                                                                                                            else if (itemjObject.Key.ToString() == "is_thread")
                                                                                                            {
                                                                                                                is_thread = itemjObject.Value.ToString();
                                                                                                            }
                                                                                                            else if (itemjObject.Key.ToString() == "post_content")
                                                                                                            {
                                                                                                                post_content = itemjObject.Value.ToString();
                                                                                                            }
                                                                                                            else if (itemjObject.Key.ToString() == "post_time")
                                                                                                            {
                                                                                                                post_time = itemjObject.Value.ToString();
                                                                                                            }
                                                                                                            else if (itemjObject.Key.ToString() == "post_votes")
                                                                                                            {
                                                                                                                post_votes = itemjObject.Value.ToString();
                                                                                                            }
                                                                                                            else if (itemjObject.Key.ToString() == "post_id")
                                                                                                            {
                                                                                                                post_id = itemjObject.Value.ToString();
                                                                                                            }
                                                                                                            else if (itemjObject.Key.ToString() == "total_posts")
                                                                                                            {
                                                                                                                total_posts = itemjObject.Value.ToString();
                                                                                                            }
                                                                                                            else if (itemjObject.Key.ToString() == "thread_id")
                                                                                                            {
                                                                                                                thread_id = itemjObject.Value.ToString();
                                                                                                            }
                                                                                                            else if (itemjObject.Key.ToString() == "thread_subject")
                                                                                                            {
                                                                                                                thread_subject = itemjObject.Value.ToString();
                                                                                                            }
                                                                                                            else if (itemjObject.Key.ToString() == "forum_id")
                                                                                                            {
                                                                                                                forum_id = itemjObject.Value.ToString();
                                                                                                            }
                                                                                                            else if (itemjObject.Key.ToString() == "thread_user_id")
                                                                                                            {
                                                                                                                thread_user_id = itemjObject.Value.ToString();
                                                                                                            }
                                                                                                            else if (itemjObject.Key.ToString() == "enable_voting")
                                                                                                            {
                                                                                                                enable_voting = itemjObject.Value.ToString();
                                                                                                            }
                                                                                                            else if (itemjObject.Key.ToString() == "site_id")
                                                                                                            {
                                                                                                                _site_id = itemjObject.Value.ToString();
                                                                                                            }
                                                                                                            else if (itemjObject.Key.ToString() == "name")
                                                                                                            {
                                                                                                                name = itemjObject.Value.ToString();
                                                                                                            }
                                                                                                            else if (itemjObject.Key.ToString() == "forum_name")
                                                                                                            {
                                                                                                                forum_name = itemjObject.Value.ToString();
                                                                                                            }
                                                                                                            else if (itemjObject.Key.ToString() == "disable_voting")
                                                                                                            {
                                                                                                                disable_voting = itemjObject.Value.ToString();
                                                                                                            }
                                                                                                            else if (itemjObject.Key.ToString() == "users_see_own_threads")
                                                                                                            {
                                                                                                                users_see_own_threads = itemjObject.Value.ToString();
                                                                                                            }
                                                                                                            else if (itemjObject.Key.ToString() == "forum_preset_id")
                                                                                                            {
                                                                                                                forum_preset_id = itemjObject.Value.ToString();
                                                                                                            }
                                                                                                            else if (itemjObject.Key.ToString() == "category_id")
                                                                                                            {
                                                                                                                category_id = itemjObject.Value.ToString();
                                                                                                            }
                                                                                                            else if (itemjObject.Key.ToString() == "category_name")
                                                                                                            {
                                                                                                                category_name = itemjObject.Value.ToString();
                                                                                                            }
                                                                                                            else if (itemjObject.Key.ToString() == "domain")
                                                                                                            {
                                                                                                                domain = itemjObject.Value.ToString();
                                                                                                            }
                                                                                                            else if (itemjObject.Key.ToString() == "page")
                                                                                                            {
                                                                                                                page = itemjObject.Value.ToString();
                                                                                                            }
                                                                                                            else if (itemjObject.Key.ToString() == "url")
                                                                                                            {
                                                                                                                url = itemjObject.Value.ToString();
                                                                                                            }

                                                                                                            //Console.WriteLine(itemjObject.Key.ToString());
                                                                                                            //Console.WriteLine(itemjObject.Value.ToString());
                                                                                                            #endregion do foreach

                                                                                                        }

                                                                                                        #endregion foreach

                                                                                                        #region validate site_id

                                                                                                        //check if required values are set
                                                                                                        if (site_id == _site_id)
                                                                                                        {

                                                                                                            #region validate params

                                                                                                            if (post_content != null &&
                                                                                                                category_name != null &&
                                                                                                                forum_name != null &&
                                                                                                                name != null &&
                                                                                                                thread_subject != null &&
                                                                                                                post_time != null)
                                                                                                            {

                                                                                                                //add to model that we will do a foreach on later when writing to data file format ie: .csv or .json or even inside a php execute script
                                                                                                                if (backgroundWorker.CancellationPending == true)
                                                                                                                {
                                                                                                                    e.Cancel = true;
                                                                                                                    return;
                                                                                                                }

                                                                                                                PostModelList.Add(new EnjinExportTool.ExportModels.PostModel()
                                                                                                                {

                                                                                                                    preset_id = preset_id,
                                                                                                                    is_thread = is_thread,
                                                                                                                    post_content = post_content,
                                                                                                                    post_time = post_time,
                                                                                                                    post_votes = post_votes,
                                                                                                                    post_id = post_id,
                                                                                                                    total_posts = total_posts,
                                                                                                                    thread_id = thread_id,
                                                                                                                    thread_subject = thread_subject,
                                                                                                                    forum_id = forum_id,
                                                                                                                    thread_user_id = thread_user_id,
                                                                                                                    enable_voting = enable_voting,
                                                                                                                    site_id = _site_id,
                                                                                                                    name = name,
                                                                                                                    forum_name = forum_name,
                                                                                                                    disable_voting = disable_voting,
                                                                                                                    users_see_own_threads = users_see_own_threads,
                                                                                                                    forum_preset_id = forum_preset_id,
                                                                                                                    category_id = category_id,
                                                                                                                    category_name = category_name,
                                                                                                                    domain = domain,
                                                                                                                    page = page,
                                                                                                                    url = url,
                                                                                                                    sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)

                                                                                                                });

                                                                                                                if (backgroundWorker.CancellationPending == true)
                                                                                                                {
                                                                                                                    e.Cancel = true;
                                                                                                                    return;
                                                                                                                }

                                                                                                                #region do ForumModelList checks

                                                                                                                //add to ForumModelList after id checks
                                                                                                                //forum_id 
                                                                                                                //forum_preset_id 
                                                                                                                //forum_name

                                                                                                                if (ForumModelList.Count() == 0 ||
                                                                                                                    ForumModelList.Count() == null)
                                                                                                                {
                                                                                                                    //empty ForumModelList, add to Model
                                                                                                                    ForumModelList.Add(new EnjinExportTool.ExportModels.ForumModel()
                                                                                                                    {
                                                                                                                        category_id = category_id,
                                                                                                                        forum_id = forum_id,
                                                                                                                        forum_preset_id = forum_preset_id,
                                                                                                                        forum_name = forum_name,
                                                                                                                        sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)
                                                                                                                    });
                                                                                                                }
                                                                                                                else
                                                                                                                {

                                                                                                                    //ForumModelList has data, check forum id match so we can skip or add ...
                                                                                                                    //this may get painful with large lists ...
                                                                                                                    var matchingId = ForumModelList.FirstOrDefault(_Item => (_Item.forum_id == forum_id));
                                                                                                                    if (matchingId != null)
                                                                                                                    {
                                                                                                                        //match, lets skip ...
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {

                                                                                                                        //nothing found, lets add new forum top level category...
                                                                                                                        ForumModelList.Add(new EnjinExportTool.ExportModels.ForumModel()
                                                                                                                        {
                                                                                                                            category_id = category_id,
                                                                                                                            forum_id = forum_id,
                                                                                                                            forum_preset_id = forum_preset_id,
                                                                                                                            forum_name = forum_name,
                                                                                                                            sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)
                                                                                                                        });

                                                                                                                    }

                                                                                                                }

                                                                                                                #endregion do ForumModelList checks

                                                                                                                if (backgroundWorker.CancellationPending == true)
                                                                                                                {
                                                                                                                    e.Cancel = true;
                                                                                                                    return;
                                                                                                                }

                                                                                                                #region do CategoryModel checks

                                                                                                                //add to CategoryModel after id checks
                                                                                                                //category_id
                                                                                                                //category_name

                                                                                                                if (CategoryModelList.Count() == 0 ||
                                                                                                                    CategoryModelList.Count() == null)
                                                                                                                {
                                                                                                                    //empty CategoryModelList, add to Model
                                                                                                                    CategoryModelList.Add(new EnjinExportTool.ExportModels.CategoryModel()
                                                                                                                    {
                                                                                                                        category_id = category_id,
                                                                                                                        category_name = category_name,
                                                                                                                        sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)
                                                                                                                    });
                                                                                                                }
                                                                                                                else
                                                                                                                {

                                                                                                                    //CategoryModel has data, check forum id match so we can skip or add ...
                                                                                                                    //this may get painful with large lists ...
                                                                                                                    var matchingId = CategoryModelList.FirstOrDefault(_Item => (_Item.category_id == category_id));
                                                                                                                    if (matchingId != null)
                                                                                                                    {
                                                                                                                        //match, lets skip ...
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {

                                                                                                                        //nothing found, lets add new category...
                                                                                                                        CategoryModelList.Add(new EnjinExportTool.ExportModels.CategoryModel()
                                                                                                                        {
                                                                                                                            category_id = category_id,
                                                                                                                            category_name = category_name,
                                                                                                                            sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)

                                                                                                                        });

                                                                                                                    }

                                                                                                                }

                                                                                                                #endregion do CategoryModel checks

                                                                                                                if (backgroundWorker.CancellationPending == true)
                                                                                                                {
                                                                                                                    e.Cancel = true;
                                                                                                                    return;
                                                                                                                }

                                                                                                                #region do ThreadModel checks

                                                                                                                //add to ThreadModel after id checks
                                                                                                                //thread_id
                                                                                                                //thread_subject

                                                                                                                if (ThreadModelList.Count() == 0 ||
                                                                                                                    ThreadModelList.Count() == null)
                                                                                                                {
                                                                                                                    //empty ThreadModelList, add to Model
                                                                                                                    ThreadModelList.Add(new EnjinExportTool.ExportModels.ThreadModel()
                                                                                                                    {
                                                                                                                        category_id = category_id,
                                                                                                                        forum_id = forum_id,
                                                                                                                        thread_id = category_id,
                                                                                                                        thread_subject = thread_subject,
                                                                                                                        sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)
                                                                                                                    });
                                                                                                                }
                                                                                                                else
                                                                                                                {

                                                                                                                    //ThreadModel has data, check forum id match so we can skip or add ...
                                                                                                                    //this may get painful with large lists ...
                                                                                                                    var matchingId = ThreadModelList.FirstOrDefault(_Item => (_Item.thread_id == thread_id));
                                                                                                                    if (matchingId != null)
                                                                                                                    {
                                                                                                                        //match, lets skip ...
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {

                                                                                                                        //nothing found, lets add new category...
                                                                                                                        ThreadModelList.Add(new EnjinExportTool.ExportModels.ThreadModel()
                                                                                                                        {
                                                                                                                            category_id = category_id,
                                                                                                                            forum_id = forum_id,
                                                                                                                            thread_id = thread_id,
                                                                                                                            thread_subject = thread_subject,
                                                                                                                            sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)

                                                                                                                        });

                                                                                                                    }

                                                                                                                }

                                                                                                                #endregion do ThreadModel checks

                                                                                                                if (backgroundWorker.CancellationPending == true)
                                                                                                                {
                                                                                                                    e.Cancel = true;
                                                                                                                    return;
                                                                                                                }

                                                                                                            }
                                                                                                            else
                                                                                                            {

                                                                                                                //report missed post item
                                                                                                                //show UI update error message - NON FATAL

                                                                                                                if (backgroundWorker.CancellationPending == true)
                                                                                                                {
                                                                                                                    e.Cancel = true;
                                                                                                                    return;
                                                                                                                }

                                                                                                                ErrorEventModelList.Add(new EnjinExportTool.ExportModels.ErrorEventModel()
                                                                                                                {

                                                                                                                    id = "",
                                                                                                                    type = "post_params",
                                                                                                                    message = "Post item was not exported due to missing required data fields",
                                                                                                                    sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)

                                                                                                                });

                                                                                                            }

                                                                                                            #endregion validate params

                                                                                                        }
                                                                                                        else
                                                                                                        {

                                                                                                            //non site id match
                                                                                                            //do UI update error message - NON FATAL

                                                                                                            ErrorEventModelList.Add(new EnjinExportTool.ExportModels.ErrorEventModel()
                                                                                                            {

                                                                                                                id = "",
                                                                                                                type = "site_id_mismatch",
                                                                                                                message = "The Configured Site ID does not matched with the post items site id " + _site_id + "",
                                                                                                                sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)

                                                                                                            });

                                                                                                        }

                                                                                                        #endregion validate site_id

                                                                                                    }

                                                                                                    #endregion foreach

                                                                                                    #endregion do work on first request using userPostsJsonRequestJSONArray

                                                                                                }
                                                                                                else
                                                                                                {

                                                                                                    //do new request,processState json repsonse

                                                                                                    if (backgroundWorker.CancellationPending == true)
                                                                                                    {
                                                                                                        e.Cancel = true;
                                                                                                        return;
                                                                                                    }

                                                                                                    #region do userPostsPageJson work

                                                                                                    /*
                                                                                                    {
                                                                                                             "id="123456789",
                                                                                                             "jsonrpc":"2.0",
                                                                                                             "method":"Profile.getPosts",
                                                                                                             "params" : {
                                                                                                                 "user_id": "",
                                                                                                                 "session_id" : "",
                                                                                                                 "page" : "3"
                                                                                                             }

                                                                                                    }
                                                                                                    */
                                                                                                    if (backgroundWorker.CancellationPending == true)
                                                                                                    {
                                                                                                        e.Cancel = true;
                                                                                                        return;
                                                                                                    }

                                                                                                    JObject userPostsPageJson =
                                                                                                    new JObject(
                                                                                                        new JProperty("id", "123456789"),
                                                                                                        new JProperty("jsonrpc", "2.0"),
                                                                                                        new JProperty("method", "Profile.getPosts"),
                                                                                                        new JProperty("params",
                                                                                                        new JObject(new JProperty("session_id", session_id.ToString()),
                                                                                                            new JProperty("user_id", prop.Name.ToString()),
                                                                                                            new JProperty("page", i.ToString())))
                                                                                                        );


                                                                                                    string userPostsPageJsonRequestJSONString = null;

                                                                                                    HttpWebRequest userPostsPageRequest = (HttpWebRequest)WebRequest.Create(base_enjin_url + base_api_slug);
                                                                                                    userPostsPageRequest.CookieContainer = new CookieContainer();
                                                                                                    userPostsPageRequest.Method = WebRequestMethods.Http.Post;
                                                                                                    userPostsPageRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                                                                                                    userPostsPageRequest.AllowWriteStreamBuffering = true;
                                                                                                    userPostsPageRequest.ProtocolVersion = HttpVersion.Version11;
                                                                                                    userPostsPageRequest.AllowAutoRedirect = true;
                                                                                                    userPostsPageRequest.ContentType = "application/json";
                                                                                                    userPostsPageRequest.KeepAlive = true;
                                                                                                    byte[] userPostsPageRequestArray = Encoding.ASCII.GetBytes(userPostsPageJson.ToString());
                                                                                                    userPostsPageRequest.ContentLength = userPostsPageJson.ToString().Length;
                                                                                                    Stream userPostsPageRequestStream = userPostsPageRequest.GetRequestStream(); //open connection
                                                                                                    userPostsPageRequestStream.Write(userPostsPageRequestArray, 0, userPostsPageJson.ToString().Length); // Send the data.
                                                                                                    userPostsPageRequestStream.Close();

                                                                                                    HttpWebResponse userPostsPageRequestResponse = (HttpWebResponse)userPostsPageRequest.GetResponse();
                                                                                                    using (StreamReader userPostsPageRequestReader = new StreamReader(userPostsPageRequestResponse.GetResponseStream()))
                                                                                                    {

                                                                                                        userPostsPageJsonRequestJSONString = userPostsPageRequestReader.ReadToEnd();
                                                                                                        Console.WriteLine(userPostsPageJsonRequestJSONString);
                                                                                                        userPostsPageRequestReader.Close();

                                                                                                        if (inif.Read("Application", "backupApiData") == "true")
                                                                                                        {

                                                                                                            System.IO.StreamWriter users_api_json_user_page_file = new System.IO.StreamWriter(migration_folder_json_path + prop.Name + "_page_" + i.ToString() + ".json");
                                                                                                            users_api_json_user_page_file.WriteLine(userPostsPageJsonRequestJSONString);
                                                                                                            users_api_json_user_page_file.Close();

                                                                                                        }

                                                                                                    }

                                                                                                    if (userPostsPageJsonRequestJSONString == null || userPostsPageJsonRequestJSONString == "")
                                                                                                    {

                                                                                                        ErrorEventModelList.Add(new EnjinExportTool.ExportModels.ErrorEventModel()
                                                                                                        {

                                                                                                            id = "",
                                                                                                            type = "json_error",
                                                                                                            message = "Processing Error:" + Environment.NewLine + "the parsed json was empty" + "",
                                                                                                            sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)

                                                                                                        });

                                                                                                    }
                                                                                                    else
                                                                                                    {

                                                                                                        #region do work on first request using userPostsPageJsonRequestJSONString
                                                                                                        //already got the data to inside userPostsJsonRequestJSONArray

                                                                                                        int posts = 0;

                                                                                                        if (backgroundWorker.CancellationPending == true)
                                                                                                        {
                                                                                                            e.Cancel = true;
                                                                                                            return;
                                                                                                        }

                                                                                                        dynamic dynObj = JsonConvert.DeserializeObject(userPostsPageJsonRequestJSONString.ToString());
                                                                                                        dynamic dynObj2 = JsonConvert.DeserializeObject(dynObj.result.posts.ToString());
                                                                                                        JArray jarray = JArray.Parse(dynObj2.ToString());

                                                                                                        #region foreach

                                                                                                        foreach (var item in jarray)
                                                                                                        {


                                                                                                            if (backgroundWorker.CancellationPending == true)
                                                                                                            {
                                                                                                                e.Cancel = true;
                                                                                                                return;
                                                                                                            }

                                                                                                            #region result

                                                                                                            /*
                                                                                            
                                                                                                    string preset_id = "";
                                                                                                    string is_thread = "";
                                                                                                    string post_content = "";
                                                                                                    string post_time = "";
                                                                                                    string post_votes = "";
                                                                                                    string post_id = "";
                                                                                                    string total_posts = "";
                                                                                                    string thread_id = "";
                                                                                                    string thread_subject = "";
                                                                                                    string forum_id = "";
                                                                                                    string thread_user_id = "";
                                                                                                    string enable_voting = "";
                                                                                                    string site_id = "";
                                                                                                    string name = "";
                                                                                                    string forum_name = "";
                                                                                                    string disable_voting = "";
                                                                                                    string users_see_own_threads = "";
                                                                                                    string forum_preset_id = "";
                                                                                                    string category_id = "";
                                                                                                    string category_name = "";
                                                                                                    string domain = "";
                                                                                                    string page = "";
                                                                                                    string url = "";
                                                                                            
                                                                                                    if(itemjObject.Key.ToString() == "preset_id"){
                                                                                                    }else if(itemjObject.Key.ToString() == "is_thread"){
                                                                                                    }else if(itemjObject.Key.ToString() == "post_content"){
                                                                                                    }else if(itemjObject.Key.ToString() == "post_time"){
                                                                                                    }else if(itemjObject.Key.ToString() == "post_votes"){
                                                                                                    }else if(itemjObject.Key.ToString() == "post_id"){
                                                                                                    }else if(itemjObject.Key.ToString() == "total_posts"){
                                                                                                    }else if(itemjObject.Key.ToString() == "thread_id"){
                                                                                                    }else if(itemjObject.Key.ToString() == "thread_subject"){
                                                                                                    }else if(itemjObject.Key.ToString() == "forum_id"){
                                                                                                    }else if(itemjObject.Key.ToString() == "thread_user_id"){
                                                                                                    }else if(itemjObject.Key.ToString() == "enable_voting"){
                                                                                                    }else if(itemjObject.Key.ToString() == "site_id"){
                                                                                                    }else if(itemjObject.Key.ToString() == "name"){
                                                                                                    }else if(itemjObject.Key.ToString() == "forum_name"){
                                                                                                    }else if(itemjObject.Key.ToString() == "disable_voting"){
                                                                                                    }else if(itemjObject.Key.ToString() == "users_see_own_threads"){
                                                                                                    }else if(itemjObject.Key.ToString() == "forum_preset_id"){
                                                                                                    }else if(itemjObject.Key.ToString() == "category_id"){
                                                                                                    }else if(itemjObject.Key.ToString() == "category_name"){
                                                                                                    }else if(itemjObject.Key.ToString() == "domain"){
                                                                                                    }else if(itemjObject.Key.ToString() == "page"){
                                                                                                    }else if(itemjObject.Key.ToString() == "url"){
                                                                                                    }
                                                                                            
                                                                                                */

                                                                                                            /* 
                                                                                                                    preset_id
                                                                                                                    1148623
                                                                                                                    is_thread
                                                                                                                    0
                                                                                                                    post_content
                                                                                                                    post content will be rendered here this is just sample text
                                                                                                                    post_time
                                                                                                                    1298698176
                                                                                                                    post_votes
                                                                                                                    0
                                                                                                                    post_id
                                                                                                                    2735183
                                                                                                                    total_posts
                                                                                                                    28
                                                                                                                    thread_id
                                                                                                                    574659
                                                                                                                    thread_subject
                                                                                                                    Server Is Offline
                                                                                                                    forum_id
                                                                                                                    316857
                                                                                                                    thread_user_id
                                                                                                                    201737
                                                                                                                    enable_voting
                                                                                                                    1
                                                                                                                    site_id
                                                                                                                    55483
                                                                                                                    name
                                                                                                                    Minecraft Oblivion
                                                                                                                    forum_name
                                                                                                                    News & Announcements
                                                                                                                    disable_voting
                                                                                                                    0
                                                                                                                    users_see_own_threads
                                                                                                                    0
                                                                                                                    forum_preset_id
                                                                                                                    1148623
                                                                                                                    category_id
                                                                                                                    132280
                                                                                                                    category_name
                                                                                                                    Server
                                                                                                                    domain
                                                                                                                    http://www.theurl.com
                                                                                                                    page
                                                                                                                    forum
                                                                                                                    url
                                                                                                                    http://www.theurl.com/
                                                                                                            */


                                                                                                            #endregion  result

                                                                                                            string preset_id = "";
                                                                                                            string is_thread = "";
                                                                                                            string post_content = "";
                                                                                                            string post_time = "";
                                                                                                            string post_votes = "";
                                                                                                            string post_id = "";
                                                                                                            string total_posts = "";
                                                                                                            string thread_id = "";
                                                                                                            string thread_subject = "";
                                                                                                            string forum_id = "";
                                                                                                            string thread_user_id = "";
                                                                                                            string enable_voting = "";
                                                                                                            string _site_id = "";
                                                                                                            string name = "";
                                                                                                            string forum_name = "";
                                                                                                            string disable_voting = "";
                                                                                                            string users_see_own_threads = "";
                                                                                                            string forum_preset_id = "";
                                                                                                            string category_id = "";
                                                                                                            string category_name = "";
                                                                                                            string domain = "";
                                                                                                            string page = "";
                                                                                                            string url = "";


                                                                                                            #region do foreach

                                                                                                            JObject jObject = JObject.Parse(item.ToString());
                                                                                                            foreach (var itemjObject in jObject)
                                                                                                            {

                                                                                                                if (backgroundWorker.CancellationPending == true)
                                                                                                                {
                                                                                                                    e.Cancel = true;
                                                                                                                    return;
                                                                                                                }

                                                                                                                #region do foreach
                                                                                                                if (itemjObject.Key.ToString() == "preset_id")
                                                                                                                {
                                                                                                                    preset_id = itemjObject.Value.ToString();
                                                                                                                }
                                                                                                                else if (itemjObject.Key.ToString() == "is_thread")
                                                                                                                {
                                                                                                                    is_thread = itemjObject.Value.ToString();
                                                                                                                }
                                                                                                                else if (itemjObject.Key.ToString() == "post_content")
                                                                                                                {
                                                                                                                    post_content = itemjObject.Value.ToString();
                                                                                                                }
                                                                                                                else if (itemjObject.Key.ToString() == "post_time")
                                                                                                                {
                                                                                                                    post_time = itemjObject.Value.ToString();
                                                                                                                }
                                                                                                                else if (itemjObject.Key.ToString() == "post_votes")
                                                                                                                {
                                                                                                                    post_votes = itemjObject.Value.ToString();
                                                                                                                }
                                                                                                                else if (itemjObject.Key.ToString() == "post_id")
                                                                                                                {
                                                                                                                    post_id = itemjObject.Value.ToString();
                                                                                                                }
                                                                                                                else if (itemjObject.Key.ToString() == "total_posts")
                                                                                                                {
                                                                                                                    total_posts = itemjObject.Value.ToString();
                                                                                                                }
                                                                                                                else if (itemjObject.Key.ToString() == "thread_id")
                                                                                                                {
                                                                                                                    thread_id = itemjObject.Value.ToString();
                                                                                                                }
                                                                                                                else if (itemjObject.Key.ToString() == "thread_subject")
                                                                                                                {
                                                                                                                    thread_subject = itemjObject.Value.ToString();
                                                                                                                }
                                                                                                                else if (itemjObject.Key.ToString() == "forum_id")
                                                                                                                {
                                                                                                                    forum_id = itemjObject.Value.ToString();
                                                                                                                }
                                                                                                                else if (itemjObject.Key.ToString() == "thread_user_id")
                                                                                                                {
                                                                                                                    thread_user_id = itemjObject.Value.ToString();
                                                                                                                }
                                                                                                                else if (itemjObject.Key.ToString() == "enable_voting")
                                                                                                                {
                                                                                                                    enable_voting = itemjObject.Value.ToString();
                                                                                                                }
                                                                                                                else if (itemjObject.Key.ToString() == "site_id")
                                                                                                                {
                                                                                                                    _site_id = itemjObject.Value.ToString();
                                                                                                                }
                                                                                                                else if (itemjObject.Key.ToString() == "name")
                                                                                                                {
                                                                                                                    name = itemjObject.Value.ToString();
                                                                                                                }
                                                                                                                else if (itemjObject.Key.ToString() == "forum_name")
                                                                                                                {
                                                                                                                    forum_name = itemjObject.Value.ToString();
                                                                                                                }
                                                                                                                else if (itemjObject.Key.ToString() == "disable_voting")
                                                                                                                {
                                                                                                                    disable_voting = itemjObject.Value.ToString();
                                                                                                                }
                                                                                                                else if (itemjObject.Key.ToString() == "users_see_own_threads")
                                                                                                                {
                                                                                                                    users_see_own_threads = itemjObject.Value.ToString();
                                                                                                                }
                                                                                                                else if (itemjObject.Key.ToString() == "forum_preset_id")
                                                                                                                {
                                                                                                                    forum_preset_id = itemjObject.Value.ToString();
                                                                                                                }
                                                                                                                else if (itemjObject.Key.ToString() == "category_id")
                                                                                                                {
                                                                                                                    category_id = itemjObject.Value.ToString();
                                                                                                                }
                                                                                                                else if (itemjObject.Key.ToString() == "category_name")
                                                                                                                {
                                                                                                                    category_name = itemjObject.Value.ToString();
                                                                                                                }
                                                                                                                else if (itemjObject.Key.ToString() == "domain")
                                                                                                                {
                                                                                                                    domain = itemjObject.Value.ToString();
                                                                                                                }
                                                                                                                else if (itemjObject.Key.ToString() == "page")
                                                                                                                {
                                                                                                                    page = itemjObject.Value.ToString();
                                                                                                                }
                                                                                                                else if (itemjObject.Key.ToString() == "url")
                                                                                                                {
                                                                                                                    url = itemjObject.Value.ToString();
                                                                                                                }

                                                                                                                //Console.WriteLine(itemjObject.Key.ToString());
                                                                                                                //Console.WriteLine(itemjObject.Value.ToString());
                                                                                                                #endregion do foreach

                                                                                                            }

                                                                                                            #endregion foreach

                                                                                                            #region validate site_id

                                                                                                            //check if required values are set
                                                                                                            if (site_id == _site_id)
                                                                                                            {

                                                                                                                #region validate params

                                                                                                                if (post_content != null &&
                                                                                                                    category_name != null &&
                                                                                                                    forum_name != null &&
                                                                                                                    name != null &&
                                                                                                                    thread_subject != null &&
                                                                                                                    post_time != null)
                                                                                                                {

                                                                                                                    //add to model that we will do a foreach on later when writing to data file format ie: .csv or .json or even inside a php execute script
                                                                                                                    if (backgroundWorker.CancellationPending == true)
                                                                                                                    {
                                                                                                                        e.Cancel = true;
                                                                                                                        return;
                                                                                                                    }

                                                                                                                    PostModelList.Add(new EnjinExportTool.ExportModels.PostModel()
                                                                                                                    {

                                                                                                                        preset_id = preset_id,
                                                                                                                        is_thread = is_thread,
                                                                                                                        post_content = post_content,
                                                                                                                        post_time = post_time,
                                                                                                                        post_votes = post_votes,
                                                                                                                        post_id = post_id,
                                                                                                                        total_posts = total_posts,
                                                                                                                        thread_id = thread_id,
                                                                                                                        thread_subject = thread_subject,
                                                                                                                        forum_id = forum_id,
                                                                                                                        thread_user_id = thread_user_id,
                                                                                                                        enable_voting = enable_voting,
                                                                                                                        site_id = _site_id,
                                                                                                                        name = name,
                                                                                                                        forum_name = forum_name,
                                                                                                                        disable_voting = disable_voting,
                                                                                                                        users_see_own_threads = users_see_own_threads,
                                                                                                                        forum_preset_id = forum_preset_id,
                                                                                                                        category_id = category_id,
                                                                                                                        category_name = category_name,
                                                                                                                        domain = domain,
                                                                                                                        page = page,
                                                                                                                        url = url,
                                                                                                                        sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)

                                                                                                                    });

                                                                                                                    if (backgroundWorker.CancellationPending == true)
                                                                                                                    {
                                                                                                                        e.Cancel = true;
                                                                                                                        return;
                                                                                                                    }

                                                                                                                    #region do ForumModelList checks

                                                                                                                    //add to ForumModelList after id checks
                                                                                                                    //forum_id 
                                                                                                                    //forum_preset_id 
                                                                                                                    //forum_name

                                                                                                                    if (ForumModelList.Count() == 0 ||
                                                                                                                        ForumModelList.Count() == null)
                                                                                                                    {
                                                                                                                        //empty ForumModelList, add to Model
                                                                                                                        ForumModelList.Add(new EnjinExportTool.ExportModels.ForumModel()
                                                                                                                        {
                                                                                                                            category_id = category_id,
                                                                                                                            forum_id = forum_id,
                                                                                                                            forum_preset_id = forum_preset_id,
                                                                                                                            forum_name = forum_name,
                                                                                                                            sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)
                                                                                                                        });
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {

                                                                                                                        //ForumModelList has data, check forum id match so we can skip or add ...
                                                                                                                        //this may get painful with large lists ...
                                                                                                                        var matchingId = ForumModelList.FirstOrDefault(_Item => (_Item.forum_id == forum_id));
                                                                                                                        if (matchingId != null)
                                                                                                                        {
                                                                                                                            //match, lets skip ...
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {

                                                                                                                            //nothing found, lets add new forum top level category...
                                                                                                                            ForumModelList.Add(new EnjinExportTool.ExportModels.ForumModel()
                                                                                                                            {
                                                                                                                                category_id = category_id,
                                                                                                                                forum_id = forum_id,
                                                                                                                                forum_preset_id = forum_preset_id,
                                                                                                                                forum_name = forum_name,
                                                                                                                                sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)
                                                                                                                            });

                                                                                                                        }

                                                                                                                    }

                                                                                                                    #endregion do ForumModelList checks

                                                                                                                    if (backgroundWorker.CancellationPending == true)
                                                                                                                    {
                                                                                                                        e.Cancel = true;
                                                                                                                        return;
                                                                                                                    }

                                                                                                                    #region do CategoryModel checks

                                                                                                                    //add to CategoryModel after id checks
                                                                                                                    //category_id
                                                                                                                    //category_name

                                                                                                                    if (CategoryModelList.Count() == 0 ||
                                                                                                                        CategoryModelList.Count() == null)
                                                                                                                    {
                                                                                                                        //empty CategoryModelList, add to Model
                                                                                                                        CategoryModelList.Add(new EnjinExportTool.ExportModels.CategoryModel()
                                                                                                                        {
                                                                                                                            category_id = category_id,
                                                                                                                            category_name = category_name,
                                                                                                                            sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)
                                                                                                                        });
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {

                                                                                                                        //CategoryModel has data, check forum id match so we can skip or add ...
                                                                                                                        //this may get painful with large lists ...
                                                                                                                        var matchingId = CategoryModelList.FirstOrDefault(_Item => (_Item.category_id == category_id));
                                                                                                                        if (matchingId != null)
                                                                                                                        {
                                                                                                                            //match, lets skip ...
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {

                                                                                                                            //nothing found, lets add new category...
                                                                                                                            CategoryModelList.Add(new EnjinExportTool.ExportModels.CategoryModel()
                                                                                                                            {
                                                                                                                                category_id = category_id,
                                                                                                                                category_name = category_name,
                                                                                                                                sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)

                                                                                                                            });

                                                                                                                        }

                                                                                                                    }

                                                                                                                    #endregion do CategoryModel checks

                                                                                                                    if (backgroundWorker.CancellationPending == true)
                                                                                                                    {
                                                                                                                        e.Cancel = true;
                                                                                                                        return;
                                                                                                                    }

                                                                                                                    #region do ThreadModel checks

                                                                                                                    //add to ThreadModel after id checks
                                                                                                                    //thread_id
                                                                                                                    //thread_subject

                                                                                                                    if (ThreadModelList.Count() == 0 ||
                                                                                                                        ThreadModelList.Count() == null)
                                                                                                                    {
                                                                                                                        //empty ThreadModelList, add to Model
                                                                                                                        ThreadModelList.Add(new EnjinExportTool.ExportModels.ThreadModel()
                                                                                                                        {
                                                                                                                            category_id = category_id,
                                                                                                                            forum_id = forum_id,
                                                                                                                            thread_id = category_id,
                                                                                                                            thread_subject = thread_subject,
                                                                                                                            sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)
                                                                                                                        });
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {

                                                                                                                        //ThreadModel has data, check forum id match so we can skip or add ...
                                                                                                                        //this may get painful with large lists ...
                                                                                                                        var matchingId = ThreadModelList.FirstOrDefault(_Item => (_Item.thread_id == thread_id));
                                                                                                                        if (matchingId != null)
                                                                                                                        {
                                                                                                                            //match, lets skip ...
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {

                                                                                                                            //nothing found, lets add new category...
                                                                                                                            ThreadModelList.Add(new EnjinExportTool.ExportModels.ThreadModel()
                                                                                                                            {
                                                                                                                                category_id = category_id,
                                                                                                                                forum_id = forum_id,
                                                                                                                                thread_id = thread_id,
                                                                                                                                thread_subject = thread_subject,
                                                                                                                                sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)

                                                                                                                            });

                                                                                                                        }

                                                                                                                    }

                                                                                                                    #endregion do ThreadModel checks

                                                                                                                    if (backgroundWorker.CancellationPending == true)
                                                                                                                    {
                                                                                                                        e.Cancel = true;
                                                                                                                        return;
                                                                                                                    }

                                                                                                                }
                                                                                                                else
                                                                                                                {

                                                                                                                    //report missed post item
                                                                                                                    //show UI update error message - NON FATAL

                                                                                                                    if (backgroundWorker.CancellationPending == true)
                                                                                                                    {
                                                                                                                        e.Cancel = true;
                                                                                                                        return;
                                                                                                                    }

                                                                                                                    ErrorEventModelList.Add(new EnjinExportTool.ExportModels.ErrorEventModel()
                                                                                                                    {

                                                                                                                        id = "",
                                                                                                                        type = "post_params",
                                                                                                                        message = "Post item was not exported due to missing required data fields",
                                                                                                                        sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)

                                                                                                                    });

                                                                                                                }

                                                                                                                #endregion validate params

                                                                                                            }
                                                                                                            else
                                                                                                            {

                                                                                                                //non site id match
                                                                                                                //do UI update error message - NON FATAL

                                                                                                                ErrorEventModelList.Add(new EnjinExportTool.ExportModels.ErrorEventModel()
                                                                                                                {

                                                                                                                    id = "",
                                                                                                                    type = "site_id_mismatch",
                                                                                                                    message = "The Configured Site ID does not matched with the post items site id " + _site_id + "",
                                                                                                                    sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)

                                                                                                                });

                                                                                                            }

                                                                                                            #endregion validate site_id

                                                                                                        }

                                                                                                        #endregion foreach

                                                                                                        #endregion do work on first request using userPostsJsonRequestJSONArray

                                                                                                    }


                                                                                                    Thread.Sleep(500);

                                                                                                    #endregion do userPostsJson work


                                                                                                }

                                                                                            }

                                                                                        }
                                                                                        else
                                                                                        {

                                                                                            #region do work on first request using userPostsJsonRequestJSONArray
                                                                                            //already got the data to inside userPostsJsonRequestJSONArray
                                                                                            
                                                                                            int posts = 0;

                                                                                            if (backgroundWorker.CancellationPending == true)
                                                                                            {
                                                                                                e.Cancel = true;
                                                                                                return;
                                                                                            }

                                                                                            dynamic dynObj = JsonConvert.DeserializeObject(userPostsJsonRequestJSON.ToString());
                                                                                            dynamic dynObj2 = JsonConvert.DeserializeObject(dynObj.result.posts.ToString());
                                                                                            JArray jarray = JArray.Parse(dynObj2.ToString());

                                                                                            #region foreach

                                                                                            foreach (var item in jarray)
                                                                                            {

                                                                                               
                                                                                                if (backgroundWorker.CancellationPending == true)
                                                                                                {
                                                                                                    e.Cancel = true;
                                                                                                    return;
                                                                                                }

                                                                                                #region result

                                                                                                /*
                                                                                            
                                                                                                    string preset_id = "";
                                                                                                    string is_thread = "";
                                                                                                    string post_content = "";
                                                                                                    string post_time = "";
                                                                                                    string post_votes = "";
                                                                                                    string post_id = "";
                                                                                                    string total_posts = "";
                                                                                                    string thread_id = "";
                                                                                                    string thread_subject = "";
                                                                                                    string forum_id = "";
                                                                                                    string thread_user_id = "";
                                                                                                    string enable_voting = "";
                                                                                                    string site_id = "";
                                                                                                    string name = "";
                                                                                                    string forum_name = "";
                                                                                                    string disable_voting = "";
                                                                                                    string users_see_own_threads = "";
                                                                                                    string forum_preset_id = "";
                                                                                                    string category_id = "";
                                                                                                    string category_name = "";
                                                                                                    string domain = "";
                                                                                                    string page = "";
                                                                                                    string url = "";
                                                                                            
                                                                                                    if(itemjObject.Key.ToString() == "preset_id"){
                                                                                                    }else if(itemjObject.Key.ToString() == "is_thread"){
                                                                                                    }else if(itemjObject.Key.ToString() == "post_content"){
                                                                                                    }else if(itemjObject.Key.ToString() == "post_time"){
                                                                                                    }else if(itemjObject.Key.ToString() == "post_votes"){
                                                                                                    }else if(itemjObject.Key.ToString() == "post_id"){
                                                                                                    }else if(itemjObject.Key.ToString() == "total_posts"){
                                                                                                    }else if(itemjObject.Key.ToString() == "thread_id"){
                                                                                                    }else if(itemjObject.Key.ToString() == "thread_subject"){
                                                                                                    }else if(itemjObject.Key.ToString() == "forum_id"){
                                                                                                    }else if(itemjObject.Key.ToString() == "thread_user_id"){
                                                                                                    }else if(itemjObject.Key.ToString() == "enable_voting"){
                                                                                                    }else if(itemjObject.Key.ToString() == "site_id"){
                                                                                                    }else if(itemjObject.Key.ToString() == "name"){
                                                                                                    }else if(itemjObject.Key.ToString() == "forum_name"){
                                                                                                    }else if(itemjObject.Key.ToString() == "disable_voting"){
                                                                                                    }else if(itemjObject.Key.ToString() == "users_see_own_threads"){
                                                                                                    }else if(itemjObject.Key.ToString() == "forum_preset_id"){
                                                                                                    }else if(itemjObject.Key.ToString() == "category_id"){
                                                                                                    }else if(itemjObject.Key.ToString() == "category_name"){
                                                                                                    }else if(itemjObject.Key.ToString() == "domain"){
                                                                                                    }else if(itemjObject.Key.ToString() == "page"){
                                                                                                    }else if(itemjObject.Key.ToString() == "url"){
                                                                                                    }
                                                                                            
                                                                                                */

                                                                                                /* 
                                                                                                        preset_id
                                                                                                        1148623
                                                                                                        is_thread
                                                                                                        0
                                                                                                        post_content
                                                                                                        post content will be rendered here this is just sample text
                                                                                                        post_time
                                                                                                        1298698176
                                                                                                        post_votes
                                                                                                        0
                                                                                                        post_id
                                                                                                        2735183
                                                                                                        total_posts
                                                                                                        28
                                                                                                        thread_id
                                                                                                        574659
                                                                                                        thread_subject
                                                                                                        Server Is Offline
                                                                                                        forum_id
                                                                                                        316857
                                                                                                        thread_user_id
                                                                                                        201737
                                                                                                        enable_voting
                                                                                                        1
                                                                                                        site_id
                                                                                                        55483
                                                                                                        name
                                                                                                        Minecraft Oblivion
                                                                                                        forum_name
                                                                                                        News & Announcements
                                                                                                        disable_voting
                                                                                                        0
                                                                                                        users_see_own_threads
                                                                                                        0
                                                                                                        forum_preset_id
                                                                                                        1148623
                                                                                                        category_id
                                                                                                        132280
                                                                                                        category_name
                                                                                                        Server
                                                                                                        domain
                                                                                                        http://www.theurl.com
                                                                                                        page
                                                                                                        forum
                                                                                                        url
                                                                                                        http://www.theurl.com/
                                                                                                */


                                                                                                #endregion  result

                                                                                                string preset_id = "";
                                                                                                string is_thread = "";
                                                                                                string post_content = "";
                                                                                                string post_time = "";
                                                                                                string post_votes = "";
                                                                                                string post_id = "";
                                                                                                string total_posts = "";
                                                                                                string thread_id = "";
                                                                                                string thread_subject = "";
                                                                                                string forum_id = "";
                                                                                                string thread_user_id = "";
                                                                                                string enable_voting = "";
                                                                                                string _site_id = "";
                                                                                                string name = "";
                                                                                                string forum_name = "";
                                                                                                string disable_voting = "";
                                                                                                string users_see_own_threads = "";
                                                                                                string forum_preset_id = "";
                                                                                                string category_id = "";
                                                                                                string category_name = "";
                                                                                                string domain = "";
                                                                                                string page = "";
                                                                                                string url = "";


                                                                                                #region do foreach

                                                                                                JObject jObject = JObject.Parse(item.ToString());
                                                                                                foreach (var itemjObject in jObject)
                                                                                                {

                                                                                                    if (backgroundWorker.CancellationPending == true)
                                                                                                    {
                                                                                                        e.Cancel = true;
                                                                                                        return;
                                                                                                    }

                                                                                                    #region do foreach
                                                                                                    if (itemjObject.Key.ToString() == "preset_id")
                                                                                                    {
                                                                                                        preset_id = itemjObject.Value.ToString();
                                                                                                    }
                                                                                                    else if (itemjObject.Key.ToString() == "is_thread")
                                                                                                    {
                                                                                                        is_thread = itemjObject.Value.ToString();
                                                                                                    }
                                                                                                    else if (itemjObject.Key.ToString() == "post_content")
                                                                                                    {
                                                                                                        post_content = itemjObject.Value.ToString();
                                                                                                    }
                                                                                                    else if (itemjObject.Key.ToString() == "post_time")
                                                                                                    {
                                                                                                        post_time = itemjObject.Value.ToString();
                                                                                                    }
                                                                                                    else if (itemjObject.Key.ToString() == "post_votes")
                                                                                                    {
                                                                                                        post_votes = itemjObject.Value.ToString();
                                                                                                    }
                                                                                                    else if (itemjObject.Key.ToString() == "post_id")
                                                                                                    {
                                                                                                        post_id = itemjObject.Value.ToString();
                                                                                                    }
                                                                                                    else if (itemjObject.Key.ToString() == "total_posts")
                                                                                                    {
                                                                                                        total_posts = itemjObject.Value.ToString();
                                                                                                    }
                                                                                                    else if (itemjObject.Key.ToString() == "thread_id")
                                                                                                    {
                                                                                                        thread_id = itemjObject.Value.ToString();
                                                                                                    }
                                                                                                    else if (itemjObject.Key.ToString() == "thread_subject")
                                                                                                    {
                                                                                                        thread_subject = itemjObject.Value.ToString();
                                                                                                    }
                                                                                                    else if (itemjObject.Key.ToString() == "forum_id")
                                                                                                    {
                                                                                                        forum_id = itemjObject.Value.ToString();
                                                                                                    }
                                                                                                    else if (itemjObject.Key.ToString() == "thread_user_id")
                                                                                                    {
                                                                                                        thread_user_id = itemjObject.Value.ToString();
                                                                                                    }
                                                                                                    else if (itemjObject.Key.ToString() == "enable_voting")
                                                                                                    {
                                                                                                        enable_voting = itemjObject.Value.ToString();
                                                                                                    }
                                                                                                    else if (itemjObject.Key.ToString() == "site_id")
                                                                                                    {
                                                                                                        _site_id = itemjObject.Value.ToString();
                                                                                                    }
                                                                                                    else if (itemjObject.Key.ToString() == "name")
                                                                                                    {
                                                                                                        name = itemjObject.Value.ToString();
                                                                                                    }
                                                                                                    else if (itemjObject.Key.ToString() == "forum_name")
                                                                                                    {
                                                                                                        forum_name = itemjObject.Value.ToString();
                                                                                                    }
                                                                                                    else if (itemjObject.Key.ToString() == "disable_voting")
                                                                                                    {
                                                                                                        disable_voting = itemjObject.Value.ToString();
                                                                                                    }
                                                                                                    else if (itemjObject.Key.ToString() == "users_see_own_threads")
                                                                                                    {
                                                                                                        users_see_own_threads = itemjObject.Value.ToString();
                                                                                                    }
                                                                                                    else if (itemjObject.Key.ToString() == "forum_preset_id")
                                                                                                    {
                                                                                                        forum_preset_id = itemjObject.Value.ToString();
                                                                                                    }
                                                                                                    else if (itemjObject.Key.ToString() == "category_id")
                                                                                                    {
                                                                                                        category_id = itemjObject.Value.ToString();
                                                                                                    }
                                                                                                    else if (itemjObject.Key.ToString() == "category_name")
                                                                                                    {
                                                                                                        category_name = itemjObject.Value.ToString();
                                                                                                    }
                                                                                                    else if (itemjObject.Key.ToString() == "domain")
                                                                                                    {
                                                                                                        domain = itemjObject.Value.ToString();
                                                                                                    }
                                                                                                    else if (itemjObject.Key.ToString() == "page")
                                                                                                    {
                                                                                                        page = itemjObject.Value.ToString();
                                                                                                    }
                                                                                                    else if (itemjObject.Key.ToString() == "url")
                                                                                                    {
                                                                                                        url = itemjObject.Value.ToString();
                                                                                                    }

                                                                                                    //Console.WriteLine(itemjObject.Key.ToString());
                                                                                                    //Console.WriteLine(itemjObject.Value.ToString());
                                                                                                    #endregion do foreach

                                                                                                }

                                                                                                #endregion foreach

                                                                                                #region validate site_id

                                                                                                //check if required values are set
                                                                                                if (site_id == _site_id)
                                                                                                {

                                                                                                    #region validate params

                                                                                                    if (post_content != null &&
                                                                                                        category_name != null &&
                                                                                                        forum_name != null &&
                                                                                                        name != null &&
                                                                                                        thread_subject != null &&
                                                                                                        post_time != null)
                                                                                                    {

                                                                                                        //add to model that we will do a foreach on later when writing to data file format ie: .csv or .json or even inside a php execute script
                                                                                                        if (backgroundWorker.CancellationPending == true)
                                                                                                        {
                                                                                                            e.Cancel = true;
                                                                                                            return;
                                                                                                        }

                                                                                                        PostModelList.Add(new EnjinExportTool.ExportModels.PostModel()
                                                                                                        {

                                                                                                            preset_id = preset_id,
                                                                                                            is_thread = is_thread,
                                                                                                            post_content = post_content,
                                                                                                            post_time = post_time,
                                                                                                            post_votes = post_votes,
                                                                                                            post_id = post_id,
                                                                                                            total_posts = total_posts,
                                                                                                            thread_id = thread_id,
                                                                                                            thread_subject = thread_subject,
                                                                                                            forum_id = forum_id,
                                                                                                            thread_user_id = thread_user_id,
                                                                                                            enable_voting = enable_voting,
                                                                                                            site_id = _site_id,
                                                                                                            name = name,
                                                                                                            forum_name = forum_name,
                                                                                                            disable_voting = disable_voting,
                                                                                                            users_see_own_threads = users_see_own_threads,
                                                                                                            forum_preset_id = forum_preset_id,
                                                                                                            category_id = category_id,
                                                                                                            category_name = category_name,
                                                                                                            domain = domain,
                                                                                                            page = page,
                                                                                                            url = url,
                                                                                                            sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)

                                                                                                        });

                                                                                                        if (backgroundWorker.CancellationPending == true)
                                                                                                        {
                                                                                                            e.Cancel = true;
                                                                                                            return;
                                                                                                        }

                                                                                                        #region do ForumModelList checks

                                                                                                        //add to ForumModelList after id checks
                                                                                                        //forum_id 
                                                                                                        //forum_preset_id 
                                                                                                        //forum_name

                                                                                                        if (ForumModelList.Count() == 0 ||
                                                                                                            ForumModelList.Count() == null)
                                                                                                        {
                                                                                                            //empty ForumModelList, add to Model
                                                                                                            ForumModelList.Add(new EnjinExportTool.ExportModels.ForumModel()
                                                                                                            {
                                                                                                                category_id = category_id,
                                                                                                                forum_id = forum_id,
                                                                                                                forum_preset_id = forum_preset_id,
                                                                                                                forum_name = forum_name,
                                                                                                                sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)
                                                                                                            });
                                                                                                        }
                                                                                                        else
                                                                                                        {

                                                                                                            //ForumModelList has data, check forum id match so we can skip or add ...
                                                                                                            //this may get painful with large lists ...
                                                                                                            var matchingId = ForumModelList.FirstOrDefault(_Item => (_Item.forum_id == forum_id));
                                                                                                            if (matchingId != null)
                                                                                                            {
                                                                                                                //match, lets skip ...
                                                                                                            }
                                                                                                            else
                                                                                                            {

                                                                                                                //nothing found, lets add new forum top level category...
                                                                                                                ForumModelList.Add(new EnjinExportTool.ExportModels.ForumModel()
                                                                                                                {
                                                                                                                    category_id = category_id,
                                                                                                                    forum_id = forum_id,
                                                                                                                    forum_preset_id = forum_preset_id,
                                                                                                                    forum_name = forum_name,
                                                                                                                    sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)
                                                                                                                });

                                                                                                            }

                                                                                                        }

                                                                                                        #endregion do ForumModelList checks

                                                                                                        if (backgroundWorker.CancellationPending == true)
                                                                                                        {
                                                                                                            e.Cancel = true;
                                                                                                            return;
                                                                                                        }

                                                                                                        #region do CategoryModel checks

                                                                                                        //add to CategoryModel after id checks
                                                                                                        //category_id
                                                                                                        //category_name

                                                                                                        if (CategoryModelList.Count() == 0 ||
                                                                                                            CategoryModelList.Count() == null)
                                                                                                        {
                                                                                                            //empty CategoryModelList, add to Model
                                                                                                            CategoryModelList.Add(new EnjinExportTool.ExportModels.CategoryModel()
                                                                                                            {
                                                                                                                category_id = category_id,
                                                                                                                category_name = category_name,
                                                                                                                sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)
                                                                                                            });
                                                                                                        }
                                                                                                        else
                                                                                                        {

                                                                                                            //CategoryModel has data, check forum id match so we can skip or add ...
                                                                                                            //this may get painful with large lists ...
                                                                                                            var matchingId = CategoryModelList.FirstOrDefault(_Item => (_Item.category_id == category_id));
                                                                                                            if (matchingId != null)
                                                                                                            {
                                                                                                                //match, lets skip ...
                                                                                                            }
                                                                                                            else
                                                                                                            {

                                                                                                                //nothing found, lets add new category...
                                                                                                                CategoryModelList.Add(new EnjinExportTool.ExportModels.CategoryModel()
                                                                                                                {
                                                                                                                    category_id = category_id,
                                                                                                                    category_name = category_name,
                                                                                                                    sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)

                                                                                                                });

                                                                                                            }

                                                                                                        }

                                                                                                        #endregion do CategoryModel checks

                                                                                                        if (backgroundWorker.CancellationPending == true)
                                                                                                        {
                                                                                                            e.Cancel = true;
                                                                                                            return;
                                                                                                        }

                                                                                                        #region do ThreadModel checks

                                                                                                        //add to ThreadModel after id checks
                                                                                                        //thread_id
                                                                                                        //thread_subject

                                                                                                        if (ThreadModelList.Count() == 0 ||
                                                                                                            ThreadModelList.Count() == null)
                                                                                                        {
                                                                                                            //empty ThreadModelList, add to Model
                                                                                                            ThreadModelList.Add(new EnjinExportTool.ExportModels.ThreadModel()
                                                                                                            {
                                                                                                                category_id = category_id,
                                                                                                                forum_id = forum_id,
                                                                                                                thread_id = category_id,
                                                                                                                thread_subject = thread_subject,
                                                                                                                sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)
                                                                                                            });
                                                                                                        }
                                                                                                        else
                                                                                                        {

                                                                                                            //ThreadModel has data, check forum id match so we can skip or add ...
                                                                                                            //this may get painful with large lists ...
                                                                                                            var matchingId = ThreadModelList.FirstOrDefault(_Item => (_Item.thread_id == thread_id));
                                                                                                            if (matchingId != null)
                                                                                                            {
                                                                                                                //match, lets skip ...
                                                                                                            }
                                                                                                            else
                                                                                                            {

                                                                                                                //nothing found, lets add new category...
                                                                                                                ThreadModelList.Add(new EnjinExportTool.ExportModels.ThreadModel()
                                                                                                                {
                                                                                                                    category_id = category_id,
                                                                                                                    forum_id = forum_id,
                                                                                                                    thread_id = thread_id,
                                                                                                                    thread_subject = thread_subject,
                                                                                                                    sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)

                                                                                                                });

                                                                                                            }

                                                                                                        }

                                                                                                        #endregion do ThreadModel checks

                                                                                                        if (backgroundWorker.CancellationPending == true)
                                                                                                        {
                                                                                                            e.Cancel = true;
                                                                                                            return;
                                                                                                        }

                                                                                                    }
                                                                                                    else
                                                                                                    {

                                                                                                        //report missed post item
                                                                                                        //show UI update error message - NON FATAL

                                                                                                        if (backgroundWorker.CancellationPending == true)
                                                                                                        {
                                                                                                            e.Cancel = true;
                                                                                                            return;
                                                                                                        }

                                                                                                        ErrorEventModelList.Add(new EnjinExportTool.ExportModels.ErrorEventModel()
                                                                                                        {

                                                                                                            id = "",
                                                                                                            type = "post_params",
                                                                                                            message = "Post item was not exported due to missing required data fields",
                                                                                                            sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)

                                                                                                        });

                                                                                                    }

                                                                                                    #endregion validate params

                                                                                                }
                                                                                                else
                                                                                                {

                                                                                                    //non site id match
                                                                                                    //do UI update error message - NON FATAL

                                                                                                    ErrorEventModelList.Add(new EnjinExportTool.ExportModels.ErrorEventModel()
                                                                                                    {

                                                                                                        id = "",
                                                                                                        type = "site_id_mismatch",
                                                                                                        message = "The Configured Site ID does not matched with the post items site id " + _site_id + "",
                                                                                                        sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)

                                                                                                    });

                                                                                                }

                                                                                                #endregion validate site_id

                                                                                            }

                                                                                            #endregion foreach

                                                                                            #endregion do work on first request using userPostsJsonRequestJSONArray

                                                                                        }


                                                                                        

                                                                                    }
                                                                                    catch (Exception error)
                                                                                    {

                                                                                        //error parse string to int ...
                                                                                        Console.WriteLine(error);
                                                                                        Console.WriteLine("End User Validated on Site " + communitiesNode.Name.ToString() + " but cant determine pages");
                                                                                        //do UI update error message - NON FATAL

                                                                                        ErrorEventModelList.Add(new EnjinExportTool.ExportModels.ErrorEventModel()
                                                                                        {

                                                                                            id = "",
                                                                                            type = "item_error",
                                                                                            message = "Processing Error:" + Environment.NewLine + error + "",
                                                                                            sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)

                                                                                        });


                                                                                        #region logging

                                                                                        try
                                                                                        {


                                                                                            //Logging                        

                                                                                            using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                                                                            {
                                                                                                string time = DateTime.Now.ToString();
                                                                                                sw.WriteLine("LogStart -------------------------------------------------------------------");
                                                                                                sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                                                                                sw.WriteLine("LogType Exception");
                                                                                                sw.WriteLine("LogMessage " + error);
                                                                                                sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                                                                            }
                                                                                        }
                                                                                        catch (Exception log_error)
                                                                                        {
                                                                                            //super fail

                                                                                        }

                                                                                        #endregion logging


                                                                                    }

                                                                                    



                                                                                }
                                                                                catch (Exception error)
                                                                                {

                                                                                    Console.WriteLine("End User Validated on Site " + communitiesNode.Name.ToString() + " but cant determine pages");
                                                                                    //do UI update error message - NON FATAL
                                                                                    ErrorEventModelList.Add(new EnjinExportTool.ExportModels.ErrorEventModel()
                                                                                    {

                                                                                        id = "",
                                                                                        type = "item_error",
                                                                                        message = "End User Validated on Site " + communitiesNode.Name.ToString() + " but cant determine pages",
                                                                                        sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)

                                                                                    });


                                                                                    #region logging

                                                                                    try
                                                                                    {


                                                                                        //Logging                        

                                                                                        using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                                                                        {
                                                                                            string time = DateTime.Now.ToString();
                                                                                            sw.WriteLine("LogStart -------------------------------------------------------------------");
                                                                                            sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                                                                            sw.WriteLine("LogType Exception");
                                                                                            sw.WriteLine("LogMessage " + error);
                                                                                            sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                                                                        }
                                                                                    }
                                                                                    catch (Exception log_error)
                                                                                    {
                                                                                        //super fail

                                                                                    }

                                                                                    #endregion logging


                                                                                }



                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                        }
                                                        catch (Exception error)
                                                        {


                                                            processState[0] = "event";
                                                            processState[1] = "Error processing ID " + prop.Name + " ... ";
                                                            processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                                                            processState[3] = "";
                                                            processState[4] = "";
                                                            processState[5] = "";
                                                            processState[6] = "";
                                                            processState[7] = "";
                                                            processState[8] = "";
                                                            processState[9] = "";
                                                            processState[10] = "";
                                                            backgroundWorker.ReportProgress(20, processState);

                                                            ErrorEventModelList.Add(new EnjinExportTool.ExportModels.ErrorEventModel()
                                                            {

                                                                id = "",
                                                                type = "item_error",
                                                                message = "Processing Error:" + Environment.NewLine + error + "",
                                                                sync_time = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now)

                                                            });



                                                            #region logging

                                                            try
                                                            {


                                                                //Logging                        

                                                                using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                                                {
                                                                    string time = DateTime.Now.ToString();
                                                                    sw.WriteLine("LogStart -------------------------------------------------------------------");
                                                                    sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                                                    sw.WriteLine("LogType Exception");
                                                                    sw.WriteLine("LogMessage " + error);
                                                                    sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                                                }
                                                            }
                                                            catch (Exception log_error)
                                                            {
                                                                //super fail

                                                            }

                                                            #endregion logging


                                                        }


                                                    }

                                                    #endregion do api work


                                                }
                                                else
                                                {

                                                    //missed user exporting .. not good really.
                                                    //do UI update error message - FATAL
                                                    #region logging

                                                    try
                                                    {


                                                        //Logging                        

                                                        using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                                        {
                                                            string time = DateTime.Now.ToString();
                                                            sw.WriteLine("LogStart -------------------------------------------------------------------");
                                                            sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                                            sw.WriteLine("LogType Exception");
                                                            sw.WriteLine("LogMessage missed user exporting .. not good really");
                                                            sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                                        }
                                                    }
                                                    catch (Exception log_error)
                                                    {
                                                        //super fail

                                                    }

                                                    #endregion logging

                                                }



                                            }
                                        }



                                    }
                                    #endregion foreach user

                                    #region do model processing & checks

                                    #region model valid items

                                    if (UserModelList.Count() == 0 &&
                                        PostModelList.Count() == 0 &&
                                        ForumModelList.Count() == 0 &&
                                        CategoryModelList.Count() == 0 &&
                                        ThreadModelList.Count() == 0)
                                    {

                                        //All list need at least 1 count on each ...
                                        //Update UI with error message state
                                        #region logging

                                        try
                                        {


                                            //Logging                        

                                            using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                            {
                                                string time = DateTime.Now.ToString();
                                                sw.WriteLine("LogStart -------------------------------------------------------------------");
                                                sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                                sw.WriteLine("LogType Exception");
                                                sw.WriteLine("LogMessage User API List were empty");
                                                sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                            }
                                        }
                                        catch (Exception log_error)
                                        {
                                            //super fail

                                        }

                                        #endregion logging


                                    }else
                                    {
                                        
                                        XmlWriter xmlWriter = XmlWriter.Create(migration_folder_xml_path + "export.xml");
                                        xmlWriter.WriteStartDocument();
                                        xmlWriter.WriteStartElement("export");

                                        #region do xml writes for CategoryModelList items

                                        xmlWriter.WriteStartElement("categories");

                                        foreach(var CategoryModelListItem in CategoryModelList) {

                                            xmlWriter.WriteStartElement("category");
                                            xmlWriter.WriteStartElement("category_id");
                                            xmlWriter.WriteString(CategoryModelListItem.category_id);
                                            xmlWriter.WriteEndElement();
                                            xmlWriter.WriteStartElement("category_name");
                                            xmlWriter.WriteString(CategoryModelListItem.category_name);
                                            xmlWriter.WriteEndElement();
                                            xmlWriter.WriteEndElement();
                                        }

                                        xmlWriter.WriteEndElement();

                                        #endregion do xml writes for CategoryModelList items

                                        #region do xml writes for ForumModelList items

                                        xmlWriter.WriteStartElement("forums");

                                        foreach (var ForumModelListItem in ForumModelList)
                                        {

                                            xmlWriter.WriteStartElement("forum");
                                            xmlWriter.WriteStartElement("category_id");
                                            xmlWriter.WriteString(ForumModelListItem.category_id);
                                            xmlWriter.WriteEndElement();
                                            xmlWriter.WriteStartElement("forum_id");
                                            xmlWriter.WriteString(ForumModelListItem.forum_id);
                                            xmlWriter.WriteEndElement();
                                            xmlWriter.WriteStartElement("forum_name");
                                            xmlWriter.WriteString(ForumModelListItem.forum_name);
                                            xmlWriter.WriteEndElement();
                                            xmlWriter.WriteEndElement();
                                        }

                                        xmlWriter.WriteEndElement();

                                        #endregion do xml writes for ForumModelList items

                                        #region do xml writes for ThreadModelList items

                                        xmlWriter.WriteStartElement("threads");

                                        foreach (var ThreadModelListItem in ThreadModelList)
                                        {

                                            xmlWriter.WriteStartElement("thread");
                                            xmlWriter.WriteStartElement("category_id");
                                            xmlWriter.WriteString(ThreadModelListItem.category_id);
                                            xmlWriter.WriteEndElement();
                                            xmlWriter.WriteStartElement("forum_id");
                                            xmlWriter.WriteString(ThreadModelListItem.forum_id);
                                            xmlWriter.WriteEndElement();
                                            xmlWriter.WriteStartElement("thread_id");
                                            xmlWriter.WriteString(ThreadModelListItem.thread_id);
                                            xmlWriter.WriteEndElement();
                                            xmlWriter.WriteStartElement("thread_name");
                                            xmlWriter.WriteString(ThreadModelListItem.thread_subject);
                                            xmlWriter.WriteEndElement();
                                            xmlWriter.WriteEndElement();
                                        }


                                        xmlWriter.WriteEndElement();

                                        #endregion do xml writes for ThreadModelList items

                                        #region do xml writes for UserModelList items

                                            xmlWriter.WriteStartElement("users");                                            

                                            #region do xml writes for UserModelList > PostModelList items

                                            foreach (var UserModelListItem in UserModelList)
                                            {

                                                xmlWriter.WriteStartElement("user");
                                                xmlWriter.WriteStartElement("user_id");
                                                xmlWriter.WriteString(UserModelListItem.user_id);
                                                xmlWriter.WriteEndElement();
                                                xmlWriter.WriteStartElement("user_name");
                                                xmlWriter.WriteString(UserModelListItem.user_name);
                                                xmlWriter.WriteEndElement();

                                                //foreach posts
                                                #region foreach
                                                xmlWriter.WriteStartElement("posts");

                                                foreach (var PostModelListItem in PostModelList)
                                                {

                                                    if (PostModelListItem.thread_user_id == UserModelListItem.user_id)
                                                    {

                                                        //users post
                                                        xmlWriter.WriteStartElement("post");

                                                        /*
                                                        preset_id = preset_id
                                                        is_thread = is_thread
                                                        post_content = post_content
                                                        post_time = post_time
                                                        post_votes = post_votes
                                                        post_id = post_id
                                                        total_posts = total_posts
                                                        thread_id = thread_id
                                                        thread_subject = thread_subject
                                                        forum_id = forum_id
                                                        thread_user_id = thread_user_id
                                                        enable_voting = enable_voting
                                                        site_id = _site_id
                                                        name = name
                                                        forum_name = forum_name
                                                        disable_voting = disable_voting
                                                        users_see_own_threads = users_see_own_threads
                                                        forum_preset_id = forum_preset_id
                                                        category_id = category_id
                                                        category_name = category_name
                                                        domain = domain
                                                        page = page
                                                        url = url
                                                        sync_time 
                                                        */

                                                        xmlWriter.WriteStartElement("preset_id");
                                                        xmlWriter.WriteString(PostModelListItem.preset_id);
                                                        xmlWriter.WriteEndElement();
                                                        xmlWriter.WriteStartElement("is_thread");
                                                        xmlWriter.WriteString(PostModelListItem.is_thread);
                                                        xmlWriter.WriteEndElement();
                                                        xmlWriter.WriteStartElement("post_content");
                                                        xmlWriter.WriteString(PostModelListItem.post_content);
                                                        xmlWriter.WriteEndElement();
                                                        xmlWriter.WriteStartElement("post_time");
                                                        xmlWriter.WriteString(PostModelListItem.post_time);
                                                        xmlWriter.WriteEndElement();
                                                        xmlWriter.WriteStartElement("post_votes");
                                                        xmlWriter.WriteString(PostModelListItem.post_votes);
                                                        xmlWriter.WriteEndElement();
                                                        xmlWriter.WriteStartElement("post_id");
                                                        xmlWriter.WriteString(PostModelListItem.post_id);
                                                        xmlWriter.WriteEndElement();
                                                        xmlWriter.WriteStartElement("total_posts");
                                                        xmlWriter.WriteString(PostModelListItem.total_posts);
                                                        xmlWriter.WriteEndElement();
                                                        xmlWriter.WriteStartElement("thread_id");
                                                        xmlWriter.WriteString(PostModelListItem.thread_id);
                                                        xmlWriter.WriteEndElement();
                                                        xmlWriter.WriteStartElement("thread_subject");
                                                        xmlWriter.WriteString(PostModelListItem.thread_subject);
                                                        xmlWriter.WriteEndElement();
                                                        xmlWriter.WriteStartElement("forum_id");
                                                        xmlWriter.WriteString(PostModelListItem.forum_id);
                                                        xmlWriter.WriteEndElement();
                                                        xmlWriter.WriteStartElement("thread_user_id");
                                                        xmlWriter.WriteString(PostModelListItem.thread_user_id);
                                                        xmlWriter.WriteEndElement();
                                                        xmlWriter.WriteStartElement("enable_voting");
                                                        xmlWriter.WriteString(PostModelListItem.enable_voting);
                                                        xmlWriter.WriteEndElement();
                                                        xmlWriter.WriteStartElement("site_id");
                                                        xmlWriter.WriteString(PostModelListItem.site_id);
                                                        xmlWriter.WriteEndElement();
                                                        xmlWriter.WriteStartElement("name");
                                                        xmlWriter.WriteString(PostModelListItem.name);
                                                        xmlWriter.WriteEndElement();
                                                        xmlWriter.WriteStartElement("forum_name");
                                                        xmlWriter.WriteString(PostModelListItem.forum_name);
                                                        xmlWriter.WriteEndElement();
                                                        xmlWriter.WriteStartElement("disable_voting");
                                                        xmlWriter.WriteString(PostModelListItem.disable_voting);
                                                        xmlWriter.WriteEndElement();
                                                        xmlWriter.WriteStartElement("users_see_own_threads");
                                                        xmlWriter.WriteString(PostModelListItem.users_see_own_threads);
                                                        xmlWriter.WriteEndElement();
                                                        xmlWriter.WriteStartElement("forum_preset_id");
                                                        xmlWriter.WriteString(PostModelListItem.forum_preset_id);
                                                        xmlWriter.WriteEndElement();
                                                        xmlWriter.WriteStartElement("category_id");
                                                        xmlWriter.WriteString(PostModelListItem.category_id);
                                                        xmlWriter.WriteEndElement();
                                                        xmlWriter.WriteStartElement("category_name");
                                                        xmlWriter.WriteString(PostModelListItem.category_name);
                                                        xmlWriter.WriteEndElement();
                                                        xmlWriter.WriteStartElement("domain");
                                                        xmlWriter.WriteString(PostModelListItem.domain);
                                                        xmlWriter.WriteEndElement();
                                                        xmlWriter.WriteStartElement("page");
                                                        xmlWriter.WriteString(PostModelListItem.page);
                                                        xmlWriter.WriteEndElement();
                                                        xmlWriter.WriteStartElement("url");
                                                        xmlWriter.WriteString(PostModelListItem.url);
                                                        xmlWriter.WriteEndElement();
                                                        xmlWriter.WriteStartElement("sync_time");
                                                        xmlWriter.WriteString(PostModelListItem.sync_time);
                                                        xmlWriter.WriteEndElement();


                                                        //end post
                                                        xmlWriter.WriteEndElement();
                                                        
                                                    }

                                                }

                                                xmlWriter.WriteEndElement();
                                                #endregion foreach


                                                xmlWriter.WriteEndElement();
                                            }

                                            xmlWriter.WriteEndElement();

                                            #endregion do xml writes for UserModelList > PostModelList items

                                            xmlWriter.WriteEndElement();

                                        #endregion do xml writes for UserModelList items

                                        xmlWriter.WriteEndDocument();
                                        xmlWriter.Close();

                                        

                                    }

                                    #endregion model valid items

                                    #region do error model processing & checks
                                    if (ErrorEventModelList.Count() == 0)
                                    {

                                       
                                        //no errors reported ... hmmm...

                                        #region logging

                                        try
                                        {


                                            //Logging                        

                                            using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                            {
                                                string time = DateTime.Now.ToString();
                                                sw.WriteLine("LogStart -------------------------------------------------------------------");
                                                sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                                sw.WriteLine("LogType Exception");
                                                sw.WriteLine("LogMessage no errors report during user api export");
                                                sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                            }
                                        }
                                        catch (Exception log_error)
                                        {
                                            //super fail

                                        }

                                        #endregion logging

                                    }
                                    else
                                    {


                                        XmlWriter xmlWriter = XmlWriter.Create(migration_folder_xml_path + "errors.xml");
                                        xmlWriter.WriteStartDocument();
                                        xmlWriter.WriteStartElement("errors");

                                        #region do xml writes for ErrorEventModelList items

                                        xmlWriter.WriteStartElement("error");

                                        foreach (var ErrorEventModelListItem in ErrorEventModelList)
                                        {

                                            xmlWriter.WriteStartElement("report");
                                            xmlWriter.WriteAttributeString("type", ErrorEventModelListItem.type);
                                            xmlWriter.WriteString(ErrorEventModelListItem.message);
                                            xmlWriter.WriteEndElement();

                                        }

                                        xmlWriter.WriteEndElement();
                                        

                                        xmlWriter.WriteEndDocument();
                                        xmlWriter.Close();



                                    }
                                    #endregion do error model processing & checks

                                    #endregion do model processing & checks

                                    #endregion do model processing & checks


                                }
                            }

                            #endregion do api user posts work

                            

                        }
                        else
                        {

                            #region error ui
                            processState[0] = "error";
                            processState[1] = "Session not validated";
                            processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                            backgroundWorker.ReportProgress(100, processState);

                            backgroundWorker.CancelAsync();
                            backgroundWorker.Dispose();

                            if (backgroundWorker.CancellationPending == true)
                            {
                                e.Cancel = true;
                                return;
                            }

                            #endregion error ui

                            #region logging

                            try
                            {


                                //Logging                        

                                using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                                {
                                    string time = DateTime.Now.ToString();
                                    sw.WriteLine("LogStart -------------------------------------------------------------------");
                                    sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                    sw.WriteLine("LogType Exception");
                                    sw.WriteLine("LogMessage Session ID was null ...");
                                    sw.WriteLine("LogEnd -------------------------------------------------------------------");
                                }
                            }
                            catch (Exception log_error)
                            {
                                //super fail

                            }

                            #endregion logging

                        }

                        #endregion do api session check



                        Thread.Sleep(1200);

                        processState[0] = "event";
                        processState[1] = "Validating processed data";
                        processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                        processState[3] = "";
                        processState[4] = "";
                        processState[5] = "";
                        processState[6] = "";
                        processState[7] = "";
                        processState[8] = "";
                        processState[9] = "";
                        processState[10] = "";
                        backgroundWorker.ReportProgress(25, processState);

                        if (backgroundWorker.CancellationPending == true)
                        {
                            e.Cancel = true;
                            return;
                        }



                        Thread.Sleep(1200);

                        processState[0] = "event";
                        processState[1] = "Validating processed data .";
                        processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                        processState[3] = "";
                        processState[4] = "";
                        processState[5] = "";
                        processState[6] = "";
                        processState[7] = "";
                        processState[8] = "";
                        processState[9] = "";
                        processState[10] = "";
                        backgroundWorker.ReportProgress(45, processState);

                        if (backgroundWorker.CancellationPending == true)
                        {
                            e.Cancel = true;
                            return;
                        }

                        Thread.Sleep(1200);

                        processState[0] = "event";
                        processState[1] = "Validating processed data ..";
                        processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                        processState[3] = "";
                        processState[4] = "";
                        processState[5] = "";
                        processState[6] = "";
                        processState[7] = "";
                        processState[8] = "";
                        processState[9] = "";
                        processState[10] = "";
                        backgroundWorker.ReportProgress(65, processState);

                        if (backgroundWorker.CancellationPending == true)
                        {
                            e.Cancel = true;
                            return;
                        }

                        Thread.Sleep(1200);

                        processState[0] = "event";
                        processState[1] = "Validated processed data ...";
                        processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                        processState[3] = "";
                        processState[4] = "";
                        processState[5] = "";
                        processState[6] = "";
                        processState[7] = "";
                        processState[8] = "";
                        processState[9] = "";
                        processState[10] = "";
                        backgroundWorker.ReportProgress(98, processState);

                        if (backgroundWorker.CancellationPending == true)
                        {
                            e.Cancel = true;
                            return;
                        }

                        Thread.Sleep(1200);

                        processState[0] = "event";
                        processState[1] = "Cleaning up ..";
                        processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                        processState[3] = "";
                        processState[4] = "";
                        processState[5] = "";
                        processState[6] = "";
                        processState[7] = "";
                        processState[8] = "";
                        processState[9] = "";
                        processState[10] = "";
                        backgroundWorker.ReportProgress(99, processState);

                        if (backgroundWorker.CancellationPending == true)
                        {
                            e.Cancel = true;
                            return;
                        }

                        Thread.Sleep(1200);

                        processState[0] = "event";
                        processState[1] = "Cleaning up ...";
                        processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                        processState[3] = "";
                        processState[4] = "";
                        processState[5] = "";
                        processState[6] = "";
                        processState[7] = "";
                        processState[8] = "";
                        processState[9] = "";
                        processState[10] = "";
                        backgroundWorker.ReportProgress(99, processState);

                        if (backgroundWorker.CancellationPending == true)
                        {
                            e.Cancel = true;
                            return;
                        }

                        Thread.Sleep(1200);

                        processState[0] = "event";
                        processState[1] = "Cleaning up ....";
                        processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                        processState[3] = "";
                        processState[4] = "";
                        processState[5] = "";
                        processState[6] = "";
                        processState[7] = "";
                        processState[8] = "";
                        processState[9] = "";
                        processState[10] = "";
                        backgroundWorker.ReportProgress(99, processState);

                        if (backgroundWorker.CancellationPending == true)
                        {
                            e.Cancel = true;
                            return;
                        }

                        Thread.Sleep(1200);

                        processState[0] = "ok";
                        processState[1] = "Enjin Export Completed ...";
                        processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);

                        Thread.Sleep(1200);
                        backgroundWorker.ReportProgress(100, processState);

                        if (backgroundWorker.CancellationPending == true)
                        {
                            e.Cancel = true;
                            return;
                        }


                        #region logging

                        try
                        {


                            //Logging                        

                            using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                            {
                                string time = DateTime.Now.ToString();
                                sw.WriteLine("LogStart -------------------------------------------------------------------");
                                sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                sw.WriteLine("LogType Success");
                                sw.WriteLine("LogMessage Export process completed successfully");
                                sw.WriteLine("LogEnd -------------------------------------------------------------------");
                            }
                        }
                        catch (Exception log_error)
                        {
                            //super fail

                        }

                        #endregion logging


                    }
                    catch (Exception error)
                    {



                        Console.WriteLine(error);

                        #region error ui
                        processState[0] = "error";
                        processState[1] = "processing failed";
                        processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                        backgroundWorker.ReportProgress(100, processState);

                        backgroundWorker.CancelAsync();
                        backgroundWorker.Dispose();

                        if (backgroundWorker.CancellationPending == true)
                        {
                            e.Cancel = true;
                            return;
                        }

                        #endregion error ui

                        #region logging

                        try
                        {


                            //Logging                        

                            using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                            {
                                string time = DateTime.Now.ToString();
                                sw.WriteLine("LogStart -------------------------------------------------------------------");
                                sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                sw.WriteLine("LogType Exception");
                                sw.WriteLine("LogMessage " + error);
                                sw.WriteLine("LogEnd -------------------------------------------------------------------");
                            }
                        }
                        catch (Exception log_error)
                        {
                            //super fail

                        }

                        #endregion logging

                    }
                    #endregion try login

                    }else{

                        #region error ui
                        processState[0] = "error";
                        processState[1] = "Error - Could not validate directories";
                        processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                        backgroundWorker.ReportProgress(100, processState);

                        backgroundWorker.CancelAsync();
                        backgroundWorker.Dispose();

                        if (backgroundWorker.CancellationPending == true)
                        {
                            e.Cancel = true;
                            return;
                        }

                        #endregion error ui

                        #region logging

                        try
                        {


                            //Logging                        

                            using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                            {
                                string time = DateTime.Now.ToString();
                                sw.WriteLine("LogStart -------------------------------------------------------------------");
                                sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                                sw.WriteLine("LogType Exception");
                                sw.WriteLine("LogMessage could not locate the path params passed. Check directories exist.");
                                sw.WriteLine("LogEnd -------------------------------------------------------------------");
                            }
                        }
                        catch (Exception log_error)
                        {
                            //super fail

                        }

                        #endregion logging


                    }

                }catch(Exception errorChecks) {

                    Console.WriteLine(errorChecks);

                    #region error ui
                    processState[0] = "error";
                    processState[1] = "processing failed";
                    processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                    backgroundWorker.ReportProgress(100, processState);

                    backgroundWorker.CancelAsync();
                    backgroundWorker.Dispose();

                    if (backgroundWorker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        return;
                    }

                    #endregion error ui

                    #region logging

                    try
                    {


                        //Logging                        

                        using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                        {
                            string time = DateTime.Now.ToString();
                            sw.WriteLine("LogStart -------------------------------------------------------------------");
                            sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                            sw.WriteLine("LogType Exception");
                            sw.WriteLine("LogMessage " + errorChecks);
                            sw.WriteLine("LogEnd -------------------------------------------------------------------");
                        }
                    }
                    catch (Exception log_error)
                    {
                        //super fail

                    }

                    #endregion logging

                }

                #endregion start checks



            }
            catch (Exception error)
            {

                Console.WriteLine(error);

                #region error ui
                processState[0] = "error";
                processState[1] = "processing failed";
                processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
                backgroundWorker.ReportProgress(100, processState);

                backgroundWorker.CancelAsync();
                backgroundWorker.Dispose();

                if (backgroundWorker.CancellationPending == true)
                {
                    e.Cancel = true;
                    return;
                }

                #endregion error ui


                #region logging

                try
                {


                    //Logging
                    string appfolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    string LogPath = System.IO.Path.Combine(appfolder, "EnjinExportTool/Logging");
                    if (!Directory.Exists(LogPath)) Directory.CreateDirectory(LogPath);

                    using (StreamWriter sw = File.AppendText(LogPath + "/log.txt"))
                    {
                        string time = DateTime.Now.ToString();
                        sw.WriteLine("LogStart -------------------------------------------------------------------");
                        sw.WriteLine("LogEventTimestamp " + string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now));
                        sw.WriteLine("LogType Exception");
                        sw.WriteLine("LogMessage " + error);
                        sw.WriteLine("LogEnd -------------------------------------------------------------------");
                    }
                }
                catch (Exception log_error)
                {
                    //super fail

                }

                #endregion logging


            }



        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs update)
        {

            string[] results = (string[])update.UserState;
            string value = update.ProgressPercentage.ToString();

            feedback.Text = results[1].ToString();
            feedback.ToolTip = results[1].ToString();
            feedback.Foreground = new SolidColorBrush(Colors.White);
            progress.Value = update.ProgressPercentage;
            progress.IsIndeterminate = false;

            
                if (results[0] == "ok")
                {

                    //feedback.Foreground = new SolidColorBrush(Colors.Green);
                    
                }
                else if(results[0] == "error")
                {

                    feedback.Foreground = new SolidColorBrush(Colors.Red);

                }
                else if (results[0] == "event")
                {

                    //feedback.Foreground = new SolidColorBrush(Colors.Orange);
                    if (results[10].ToString() != null)
                    {
                        feedbackMessage.Text = results[10].ToString();
                    }
                }




        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs update)
        {

            progressUI.Visibility = Visibility.Collapsed;
            loaderToolbar.Visibility = Visibility.Collapsed;
            beginProcess.IsEnabled = true;
            cancelProcess.IsEnabled = false;
            settingsProcess.IsEnabled = true;
            beginProcess.Visibility = Visibility.Visible;
            progress.Visibility = Visibility.Hidden;
            progressIntermediate.Visibility = Visibility.Hidden;

            try {

                string result = update.Cancelled.ToString();
                if (update.Cancelled)
                {
                    feedback.Text = "Process Cancelled";
                    ClearMessages();

                }
                else
                {


                }

                Console.WriteLine(result);

            }catch(Exception error) {



            }
            

        }

        private void beginProcess_Click(object sender, RoutedEventArgs e)
        {

            loaderToolbar.Visibility = Visibility.Visible;
            ImageSlider();
            
            progress.Value = 0;
            cancelProcess.IsEnabled = true;
            beginProcess.IsEnabled = false;
            settingsProcess.IsEnabled = false;
            progressUI.Visibility = Visibility.Visible;
            progress.Visibility = Visibility.Visible;
            progressIntermediate.Visibility = Visibility.Visible;
            feedback.Visibility = Visibility.Visible;
            feedback.Text = "";
            backgroundWorker.RunWorkerAsync();
        }

        private void cancelProcess_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {

                feedback.Text = "Cancelling ... Please wait ..";
                backgroundWorker.CancelAsync();
                backgroundWorker.Dispose();

                beginProcess.IsEnabled = false;
                cancelProcess.IsEnabled = true;
                progressUI.Visibility = Visibility.Visible;
                settingsProcess.IsEnabled = false;

            }catch(Exception errorCancelling) {

                //crash it!
                Application.Current.Shutdown();
            }
            
            
        }

        private void closeProcess_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                backgroundWorker.CancelAsync();
                backgroundWorker.Dispose();
                Application.Current.Shutdown();


            }
            catch (Exception errorCancelling)
            {

                //crash it!
                Application.Current.Shutdown();
            }
            
            
        }

        private void settingsProcess_MouseDown(object sender, MouseButtonEventArgs e)
        {

            SettingsWindow SettingsWindow = new SettingsWindow();
            SettingsWindow.Show();
            this.Close();


        }



    }


}
