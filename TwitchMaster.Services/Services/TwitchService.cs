using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TwitchMaster.Core.Enums;
using TwitchMaster.Core.Models;
using TwitchMaster.Services.Interfaces;

namespace TwitchMaster.Services.Services
{
    public class TwitchService : ITwitchService
    {
        #region Constants
        private const string SERVER_URL = "http://54.180.66.24";
        private const string Get_APP_ACCESS_URL = "https://id.twitch.tv/oauth2/authorize?client_id={0}&redirect_uri={1}&response_type=token&scope={2}";

        private const string Get_Clips_URL = "https://api.twitch.tv/helix/clips?broadcaster_id={0}&after={1}&before={2}&ended_at={3}&first={4}&started_at={5}";
        private const string Get_Clips_URL_T = "https://api.twitch.tv/helix/clips?broadcaster_id={0}";
      
        private const string Get_USERS_URL = "https://api.twitch.tv/helix/users?login={0}";
        private const int TWITCH_MAX_LOAD_LIMIT = 100;
       
        private const string TWITCH_CLIENT_ID = "eon17uhlko9oymtjuyrrh6tq0jnbh7";
        private const string TWITCH_SECRET_ID = "3ifaxkh1upsn9dggy90gqw57cxyubn";

        private const string TWITCH_CLIENT_ID_HEADER = "Client-ID";
        private const string TWITCH_AUTHORIZATION_HEADER = "Authorization";

        #endregion Constants

        #region Constructors

        public TwitchService() {

        }

        #endregion Constructors

        #region Methods

        private string AccessToken { get; set; }

        private bool IsAuthorized
        {
            get
            {
                return AccessToken != null;
            }
        }

        //gui 환경 필요
        public string GetAccessToken(string scope)
        {
            string url = String.Format(Get_APP_ACCESS_URL, TWITCH_CLIENT_ID, SERVER_URL, scope);
            Console.WriteLine(url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.AllowAutoRedirect = false;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string redirUrl = response.Headers["Location"];
            response.Close();

            //Show the redirected url
            return redirUrl;
        }

        private WebClient CreateTwitchWebClient()
        {
            WebClient wc = new WebClient();
            wc.Headers.Add(TWITCH_CLIENT_ID_HEADER, TWITCH_CLIENT_ID);
            wc.Encoding = Encoding.UTF8;
            return wc;
        }

        private WebClient CreateAuthorizedTwitchWebClient(string scope)
        {
            WebClient wc = CreateTwitchWebClient();

            if (IsAuthorized)
            {
                wc.Headers.Add(TWITCH_AUTHORIZATION_HEADER, "OAuth " + AccessToken);
            }

            return wc;
        }

        public string GetUsers(string userLoginName)
        {
            string userId = "";

            using (WebClient wc = CreateTwitchWebClient())
            { 
                string resultStr = wc.DownloadString(string.Format(Get_USERS_URL, userLoginName));
                JObject resultJson = JObject.Parse(resultStr);
                foreach (JObject user in resultJson.Value<JArray>("data"))
                {
                    userId = user["id"].ToString();
                }
            }
            return userId;
        }

        public TwitchClip[] GetClips(string broadcasterId)
        {
            TwitchClip[] twitchClips = new TwitchClip[100];
            int index = 0;

            using (WebClient wc = CreateTwitchWebClient())
            {
                string getUserUrl = string.Format(Get_Clips_URL_T, broadcasterId) + "&first=100";
                //string getUserUrl = string.Format(Get_Clips_URL_T, broadcasterId) + "&first=100" + "&started_at=2019-01-01T22:34:18Z" + "&ended_at=2019-03-01T22:34:18Z";
                //string getUserUrl = string.Format(Get_Clips_URL, broadcasterId, "", "", "", 100, "");
                string twitchClipsStr = wc.DownloadString(getUserUrl);
                JObject twitchClipsJson = JObject.Parse(twitchClipsStr);

                foreach (JObject twitchClipJson in twitchClipsJson.Value<JArray>("data"))
                {
                    twitchClips[index] = new TwitchClip();
                    twitchClips[index].Title = twitchClipJson["title"].ToString();
                    twitchClips[index].CreatedAt = twitchClipJson["created_at"].ToString();
                    index++;
                }
            }
            return twitchClips;
        }
    #endregion Methods
    }
}
