using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnjinExportTool
{
    class UserModel
    {

        public class JsonModel
        {


            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("method")]
            public string Method { get; set; }

            [JsonProperty("params")]
            public Params Params { get; set; }
        }

        public class Params
        {
            [JsonProperty("email")]
            public string Email { get; set; }

            [JsonProperty("password")]
            public string Password { get; set; }
        }


    }

    class EnjinUserModel
    {
             
        public class _EnjinUserModel
        {

            [JsonProperty("result")]
            public Result Result { get; set; }
        }

        public class Result
        {
            [JsonProperty("hasIdentity")]
            public string hasIdentity { get; set; }
            
            [JsonProperty("session_id")]
            public string session_id { get; set; }

            [JsonProperty("user_id")]
            public string user_id { get; set; }

            [JsonProperty("id")]
            public string id { get; set; }

            [JsonProperty("site_id")]
            public string site_id { get; set; }
            
        }       

    }

    class EnjinUserPostsModel
    {


        public class _EnjinUserPostsModel
        {

            [JsonProperty("result")]
            public Result Result { get; set; }
        }


        public class Result
        {
            [JsonProperty("pages")]
            public string pages { get; set; }

            

        }



    }

    class EnjinUserTagsModel
    {

       

        public class _EnjinUserTagsModel
        {

            [JsonProperty("user")]
            public User user { get; set; }
        }

        public class User
        {
            [JsonProperty("user")]
            public string user { get; set; }


        }


    }

    class EnjinErrorModel
    {

        /*
        {"error":{"code":-32000,
         * "message":"Too many login attempts, you may try again in 30 minutes.",
         * "data":{}},
         * "id":"123456789",
         * "jsonrpc":"2.0"}
        */


        public class _EnjinErrorModel
        {

            [JsonProperty("error")]
            public Error Error { get; set; }
        }

        public class Error
        {
            [JsonProperty("message")]
            public string message { get; set; }

            
        }

        

    }
}
