using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewSyncVideo.Website.Api
{
    public interface IClientPlayerHub
    {
        Task OnSetUsername(Guid id, string name);
        Task OnPause();
        Task OnPlay(double time);
        Task OnQueueVideo(string url);

        Task OnUserConnected(PlayerRoomParticipant participant);
        Task OnUserDisconnected(Guid id);
    }
}
