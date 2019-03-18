using System;

namespace TwitchMaster.Core.Models
{
    public class TwitchClip
    {
        #region Properties
      
        public string BroadcasterName { get; set; }
        public string CreatorName { get; set; }
        public string CreatedAt { get; set; }
        public string EmbedUrl { get; set; }
        public int GameId { get; set; }
        public int Id { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Title { get; set; }
        public int VideoId { get; set; }
        public int ViewCount { get; set; }

        #endregion Properties

    }
}
