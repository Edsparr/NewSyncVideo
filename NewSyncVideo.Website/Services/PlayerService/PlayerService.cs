using NewSyncVideo.Website.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewSyncVideo.Website
{
    public class PlayerService
    {
        public ICollection<PlayerRoomInfo> Rooms { get; } = new List<PlayerRoomInfo>()
        {
            new PlayerRoomInfo()
            {
                RoomId = "XXX",
            }
        };
    }
}
