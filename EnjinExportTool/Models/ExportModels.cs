using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnjinExportTool
{
    class ExportModels
    {

        //Json Response
        //user_id 
        //user_name


        public class UserModel
        {

            public string user_id { get; set; }
            public string user_name { get; set; }
            public string sync_time { get; set; }

        }

        //Json Response
        //forum_id 
        //forum_preset_id 
        //forum_name


        public class ForumModel
        {

            public string forum_id { get; set; }
            public string forum_preset_id { get; set; }
            public string forum_name { get; set; }
            public string sync_time { get; set; }

        }


        //Json Response
        //category_id
        //category_name

        public class CategoryModel
        {

            public string category_id { get; set; }
            public string category_name { get; set; }
            public string sync_time { get; set; }

        }


        //Json Response
        //thread_id
        //thread_subject

        public class ThreadModel
        {

            public string thread_id { get; set; }
            public string thread_subject { get; set; }
            public string sync_time { get; set; }

        }


        //Json Response
        //everything

        public class PostModel
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
            public string sync_time { get; set; }

        }


        public class ErrorEventModel
        {

            public string id { get; set; }
            public string type { get; set; }
            public string message { get; set; }
            public string sync_time { get; set; }

        }

    }


}
