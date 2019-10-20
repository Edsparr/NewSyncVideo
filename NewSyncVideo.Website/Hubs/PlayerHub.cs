using Microsoft.AspNetCore.SignalR;
using NewSyncVideo.Website.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewSyncVideo.Website
{
    public class PlayerHub : Hub<IClientPlayerHub>
    {
        private readonly PlayerService playerService;
        public PlayerHub(PlayerService playerService)
        {
            this.playerService = playerService;
        }

        public async Task<UserRoomInfo> GetUserRoomInfo()
        {
            var room = await GetRoomInfo();
            var userInfo = await GetUserInfo();
            return new UserRoomInfo()
            {
                Room = room,
                Id = userInfo.Id
            };
        }

        private async Task<PlayerRoomInfo> GetRoomInfo()
        {
            var room = playerService.Rooms.FirstOrDefault(c => c.Participants.Any(k => k.ConnectionId == Context.ConnectionId));
            if (room == null)
                return null;
            return room;
        }

        public async Task<PlayerRoomParticipant> GetUserInfo()
        {
            var room = await GetRoomInfo();
            return room?.Participants?.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
        }


        public async Task Pause()
        {
            var room = await GetRoomInfo();

            if (room == null)
                return;

            await this.Clients.GroupExcept(room.RoomId, Context.ConnectionId).OnPause();

        }

        public async Task Play(double time)
        {
            var room = await GetRoomInfo();
            if (room == null)
                return;

            await this.Clients.GroupExcept(room.RoomId, this.Context.ConnectionId).OnPlay(time);
        }

        public async Task QueueVideo(string videoUrl)
        {
            var room = await GetRoomInfo();

            await Clients.GroupExcept(room.RoomId, Context.ConnectionId).OnQueueVideo(videoUrl);
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();

            if (!httpContext.Request.Query.TryGetValue("name", out var name) ||
                !httpContext.Request.Query.TryGetValue("roomId", out var roomId))
                throw new Exception("Invalid params");

            var room = playerService.Rooms.SingleOrDefault(c => c.RoomId == roomId);
            if (room == null ||
                room.Participants.Any(c => c.ConnectionId == Context.ConnectionId))
                throw new Exception("Room not found");

            if (room.Participants.Any(c => c.ConnectionId == Context.ConnectionId))
                throw new Exception("Already connected");

            var rank = PlayerRoomParticipantRank.Member;
            if (room.Participants.Count == 0)
                rank = PlayerRoomParticipantRank.Admin; //temporary

            var participant = new PlayerRoomParticipant()
            {
                ConnectionId = Context.ConnectionId,
                Name = name,
                Rank = rank,
                Id = Guid.NewGuid()
            };

            room.Participants.Add(participant);

            await this.Clients.Group(room.RoomId).OnUserConnected(participant);

            await this.Groups.AddToGroupAsync(Context.ConnectionId, room.RoomId);
        }

        public async Task SetUsername(string username)
        {
            var room = await GetRoomInfo();
            var particpiant = await GetUserInfo();

            particpiant.Name = username;
            await this.Clients.Group(room.RoomId).OnSetUsername(particpiant.Id, username);

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);


            var room = await GetRoomInfo();

            var participant = room.Participants.Single(c => c.ConnectionId == Context.ConnectionId);

            room.Participants.Remove(participant);

            await this.Groups.RemoveFromGroupAsync(Context.ConnectionId, room.RoomId);

            await this.Clients.Group(room.RoomId).OnUserDisconnected(participant.Id);

        }


    }
}
