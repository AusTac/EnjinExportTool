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

namespace EnjinExportTool
{


    public class PostItemModel
    {
        public string preset_id { get; set; }
        public string is_thread { get; set; }
        public string post_content { get; set; }
        public string post_time { get; set; }
        public string post_votes { get; set; }
        public string post_id { get; set; }
        public string total_posts { get; set; }
        public string thread_id { get; set; }
        public string thread_subject { get; set; }
        public string forum_id { get; set; }
        public string thread_user_id { get; set; }
        public string enable_voting { get; set; }
        public string site_id { get; set; }
        public string name { get; set; }
        public string forum_name { get; set; }
        public string disable_voting { get; set; }
        public string users_see_own_threads { get; set; }
        public string forum_preset_id { get; set; }
        public string category_id { get; set; }
        public string category_name { get; set; }
        public string domain { get; set; }
        public string page { get; set; }
        public string url { get; set; }
    }
    
    
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

            
            Thread.Sleep(1200);
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

                INIFile inif = new INIFile(ApplicationPath + @"\Settings.ini");

                string base_enjin_url = "";
                string base_api_slug = "";
                string tags_api_slug = "api/get-tags";
                string site_id = "";
                string email = "";
                string password = "";
                string session_id = "";

                string migration_folder_path = "";
                string migration_folder_path_root = "";
                string exportType = "";

                if (System.IO.File.Exists(ApplicationPath + @"\Settings.ini"))
                {
                    if (!string.IsNullOrEmpty(inif.Read("Application", "base_url")) &&
                        !string.IsNullOrEmpty(inif.Read("Application", "api_slug")) &&
                        !string.IsNullOrEmpty(inif.Read("Application", "tags_api_slug")) &&
                        !string.IsNullOrEmpty(inif.Read("Application", "site_id")) &&
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
                        email = inif.Read("Application", "email");
                        password = inif.Read("Application", "password");
                        session_id = inif.Read("Application", "session_id");
                        migration_folder_path = inif.Read("Application", "migration_folder_path");
                        exportType = inif.Read("Application", "exportType");

                        //internal migration folder

                        migration_folder_path_root = migration_folder_path + @"\EnjinExportTool\migration\" + string.Format("{0:yyyy-MM-dd}", DateTime.Now) + @"\";
                        if (!Directory.Exists(migration_folder_path_root)) Directory.CreateDirectory(migration_folder_path_root);





                    }


                }


                #region start checks
                //do some folder creations & checks before moving on ...

                try
                {

                    if(Directory.Exists(migration_folder_path_root)) {


                        #region try login
                    try
                    {

                        if (backgroundWorker.CancellationPending == true)
                        {
                            e.Cancel = true;
                            return;
                        }

                        processState[0] = "event";
                        processState[1] = "Connecting to Enjin ...";
                        processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
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


                                }

                            }

                        }

                        #endregion session auth

                        #region do api work

                        if (session_id != null)
                        {



                            #region do api work

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

                                System.IO.StreamWriter file = new System.IO.StreamWriter(ApplicationPath + "/users.json");
                                file.WriteLine(_usersJson);
                                file.Close();


                                JObject results = JObject.Parse(_usersJson);

                                string usersCount = results["users"].Count().ToString();
                                int userCount = 0;

                                processState[0] = "event";
                                processState[1] = usersCount + " Users to process";
                                processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
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

                                }
                                else
                                {

                                    //int PostItemModel List
                                    List<PostItemModel> PostItemModelList = new List<PostItemModel>();

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
                                                        backgroundWorker.ReportProgress(20, processState);

                                                        userPostsJsonRequestJSON = userPostsJsonRequestReader.ReadToEnd();
                                                        userPostsJsonRequestReader.Close();

                                                        // Write the string to a file.
                                                        System.IO.StreamWriter fileUserJsonPosts = new System.IO.StreamWriter(JsonFolderUsersPath + prop.Name + ".json");
                                                        fileUserJsonPosts.WriteLine(userPostsJsonRequestJSON);
                                                        fileUserJsonPosts.Close();

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


                                                                                                    Console.WriteLine("Processing first page (" + i.ToString() + " of " + pagesCount.ToString() + ")");

                                                                                                }
                                                                                                else
                                                                                                {

                                                                                                    //do new request,processState json repsonse

                                                                                                    if (backgroundWorker.CancellationPending == true)
                                                                                                    {
                                                                                                        e.Cancel = true;
                                                                                                        return;
                                                                                                    }

                                                                                                    Console.WriteLine("Processing other page (" + i.ToString() + " of " + pagesCount.ToString() + ")");

                                                                                                    #region do userPostsPageJson work

                                                                                                    /*
                                                                                            {
                                                                                                     "id":"123456789",
                                                                                                     "jsonrpc":"2.0",
                                                                                                     "method":"Profile.getPosts",
                                                                                                     "params" : {
                                                                                                         "user_id": "6468909",
                                                                                                         "session_id" : "ii4fgcemf2ikkdemv2f8v5tuh4",
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

                                                                                                    }

                                                                                                    if (userPostsPageJsonRequestJSONString == null || userPostsPageJsonRequestJSONString == "")
                                                                                                    {


                                                                                                    }
                                                                                                    else
                                                                                                    {



                                                                                                    }


                                                                                                    Thread.Sleep(500);

                                                                                                    #endregion do userPostsJson work


                                                                                                }

                                                                                            }

                                                                                        }
                                                                                        else
                                                                                        {


                                                                                            //already got the data to inside userPostsJsonRequestJSONArray
                                                                                            Console.WriteLine("doing post items from request ...");
                                                                                            int posts = 0;

                                                                                            if (backgroundWorker.CancellationPending == true)
                                                                                            {
                                                                                                e.Cancel = true;
                                                                                                return;
                                                                                            }

                                                                                            dynamic dynObj = JsonConvert.DeserializeObject(userPostsJsonRequestJSON.ToString());
                                                                                            //Console.WriteLine("{0} {1}", dynObj.results.posts, dynObj.results.age);

                                                                                            //JArray jsonObj = JArray.Parse(dynObj.results.posts.ToString());

                                                                                            dynamic dynObj2 = JsonConvert.DeserializeObject(dynObj.result.posts.ToString());
                                                                                            //Console.WriteLine(dynObj2);
                                                                                            JArray jarray = JArray.Parse(dynObj2.ToString());

                                                                                            foreach (var item in jarray)
                                                                                            {

                                                                                                Console.WriteLine("------------- new post item --------------");
                                                                                                if (backgroundWorker.CancellationPending == true)
                                                                                                {
                                                                                                    e.Cancel = true;
                                                                                                    return;
                                                                                                }

                                                                                                #region sample result

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


                                                                                                #endregion sample result

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

                                                                                                //check if required values are set
                                                                                                if (site_id == _site_id)
                                                                                                {
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


                                                                                                    }
                                                                                                    else
                                                                                                    {

                                                                                                        //report missed post item
                                                                                                        //show UI update error message - NON FATAL
                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                {

                                                                                                    //non site id match
                                                                                                    //do UI update error message - NON FATAL
                                                                                                }

                                                                                                Console.WriteLine("------------- end post item --------------");

                                                                                            }



                                                                                        }


                                                                                    }
                                                                                    catch (Exception error)
                                                                                    {

                                                                                        //error parse string to int ...
                                                                                        Console.WriteLine(error);
                                                                                        Console.WriteLine("End User Validated on Site " + communitiesNode.Name.ToString() + " but cant determine pages");
                                                                                        //do UI update error message - NON FATAL
                                                                                    }





                                                                                }
                                                                                catch (Exception error)
                                                                                {

                                                                                    Console.WriteLine("End User Validated on Site " + communitiesNode.Name.ToString() + " but cant determine pages");
                                                                                    //do UI update error message - NON FATAL

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
                                                            backgroundWorker.ReportProgress(20, processState);

                                                            Console.WriteLine(error);
                                                        }


                                                    }

                                                    #endregion do api work


                                                }
                                                else
                                                {

                                                    //missed user exporting .. not good really.
                                                    //do UI update error message - FATAL
                                                }



                                            }
                                        }



                                    }
                                    #endregion foreach user

                                }
                            }

                            #endregion do api work

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

                        }

                        #endregion do api work



                        Thread.Sleep(1200);

                        processState[0] = "event";
                        processState[1] = "Validating processed data";
                        processState[2] = string.Format("{0:yyyy-MM-dd H:m:s}", DateTime.Now);
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

                    }
                    #endregion try login

                    }else{

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

                }




        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs update)
        {


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
