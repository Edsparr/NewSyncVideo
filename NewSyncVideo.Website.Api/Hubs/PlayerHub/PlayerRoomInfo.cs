using NewSyncVideo.Website.Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewSyncVideo.Website.Api
{
    public class PlayerRoomInfo
    {
        [JsonProperty("roomId")]
        public string RoomId { get; set; }

        [JsonProperty("participants")]
        public HashSet<PlayerRoomParticipant> Participants { get; set; } = new HashSet<PlayerRoomParticipant>();

    }
}
