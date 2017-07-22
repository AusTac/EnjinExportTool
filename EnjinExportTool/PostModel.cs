﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnjinExportTool
{
    class PostModel
    {


        public class Post
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

        public class Result
        {
            
            public int page { get; set; }
            public List<Post> posts { get; set; }
            public int totalPosts { get; set; }
            public int pages { get; set; }
        }

        public class RootObject
        {
            public Result result { get; set; }
            public string id { get; set; }
            public string jsonrpc { get; set; }
        }



    }




}
