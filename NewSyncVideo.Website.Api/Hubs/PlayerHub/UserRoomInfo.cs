using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewSyncVideo.Website.Api
{
    public class UserRoomInfo
    {
        [JsonProperty("room")]
        public PlayerRoomInfo Room { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }
    }
}
