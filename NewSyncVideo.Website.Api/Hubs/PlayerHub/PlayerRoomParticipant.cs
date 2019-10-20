using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewSyncVideo.Website.Api
{
    public class PlayerRoomParticipant
    {
        [JsonIgnore] //This is secret for the client
        public string ConnectionId { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("rank")]
        public PlayerRoomParticipantRank Rank { get; set; }

        public override int GetHashCode()
        {
            return ConnectionId.GetHashCode();
        }
    }

    public enum PlayerRoomParticipantRank
    {
        Admin = 1,
        Member = 0
    }
}
