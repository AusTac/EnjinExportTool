using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnjinExportTool
{
    class ExportGalleryModels
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
        //category_id
        //category_name

        public class GalleryCategoryModel
        {

            public string id { get; set; }
            public string name { get; set; }
            public string sync_time { get; set; }

        }




        //Json Response
        //everything

        public class GalleryItemModel
        {

            public string image_id { get; set; }
            public string preset_id { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string created { get; set; }
            public string user_id { get; set; }
            public string views { get; set; }
            public string album_id { get; set; }
            public string have_original { get; set; }
            public string ordering { get; set; }
            public string number_comments { get; set; }
            public string comment_cid { get; set; }
            public string url { get; set; }
            public string url_full { get; set; }
            public string url_original { get; set; }
            public bool can_modify { get; set; }           
            public string category_id { get; set; }
            public string category_name { get; set; }
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
