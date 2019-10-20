import React, { Component } from 'react';
import HubConnection, { HubConnectionBuilder  } from '@microsoft/signalr';
import PropTypes from 'prop-types';
import ReactPlayer from 'react-player'

export default class NetworkedVideoPlayer extends Component {
  displayName = NetworkedVideoPlayer.name
  wasOrderedByApp = false;
  reactPlayer = null;
  wasTriggeredByThis = false;

  constructor(props) {
    super(props);

    this.state = {
      hubConnection: null,
      userRoomInfo: null,
      url: null,
      isPlaying: false
    }
  }

  componentDidMount() {
    console.log("NetworkedVideoPlayer did mount...");
    this.connectToRoom(this.props.roomId, this.props.username);
  }

  componentWillUnmount() {
    console.log("NetworkedVideoPlayer unmounting...");
    this.state.hubConnection.stop();
  }

  componentWillUpdate(nextProps, nextState) {
    if (this.props.roomId !== nextProps.roomId)
      this.connectToRoom(nextProps.roomId, nextProps.username);

    if (this.props.username !== nextProps.username &&
      this.state.hubConnection !== null && this.state.hubConnection.connectionStarted) {

      this.state.hubConnection.invoke("SetUsername", nextProps.username);
      this.setUsername(this.state.userRoomInfo.id, nextProps.username);
    }

  }



  connectToRoom(roomId, name) {
    if (this.state.hubConnection !== null) {
      this.state.hubConnection.stop();
    }

    var hubConnection = new HubConnectionBuilder()
      .withUrl(`/Hubs/PlayerHub?roomId=${roomId}&name=${name}`)
      .build();

    this.subscribeToHubEvents(hubConnection);

    hubConnection.start()
      .then(() => {

        this.setState({ hubConnection });

        hubConnection.invoke("GetUserRoomInfo")
          .then(userRoomInfo => {
            this.setState({ userRoomInfo });
          })

      });
  }

  setUsername(userId, username) {
    this.setState(state => {
      var userRoomInfo = this.state.userRoomInfo;
      userRoomInfo.room.participants.find(c => c.id === userId).name = username;

      return {
        userRoomInfo
      };
    });
  }

  subscribeToHubEvents(connection) {
    connection.on("OnSetUsername", (id, name) => {
      this.setUsername(id, name);
    });

    connection.on("OnPause", () => {
      this.wasTriggeredByThis = true;
      this.setState({
        isPlaying: false
      });
      console.log("Received OnPause RPC");

    });

    connection.on("OnPlay", (time) => {
      this.wasTriggeredByThis = true;
      this.reactPlayer.seekTo(time);

      this.wasTriggeredByThis = true;
      this.setState({
        isPlaying: true
      });
      console.log("Received OnPlay RPC");
    });

    connection.on("OnQueueVideo", (url) => {
      this.setState({ url });
      console.log("Received OnQueueVideo RPC");

    });

    connection.on("OnUserConnected", (participant) => {
      this.setState(state => {
        var userRoomInfo = state.userRoomInfo;

        userRoomInfo.room.participants.push(participant);

        return {
          userRoomInfo
        };
      });
    });

    connection.on("OnUserDisconnected", (id) => {
      this.setState(state => {
        var userRoomInfo = state.userRoomInfo;

        userRoomInfo.room.participants = userRoomInfo.room.participants.filter(c => c.id === id);
        return {
          userRoomInfo
        };
      });
    })
  }

  queueVideo(url) {
    this.state.hubConnection.invoke("QueueVideo", url);
    this.setState({
      url
    });
  }

  onPlayerPlay(e) {
    if (this.wasTriggeredByThis) {
      this.wasTriggeredByThis = false;
      return;
    }
    console.log("Sending Play RPC");
    this.state.hubConnection.invoke("Play", this.reactPlayer.getCurrentTime());
  }

  onPlayerPause(e) {
    if (this.wasTriggeredByThis) {
      this.wasTriggeredByThis = false;
      return;
    }
    console.log("Sending Pause RPC");
    this.state.hubConnection.invoke("Pause");
  }

  render() {
    console.log(`IsPlaying: ${this.state.isPlaying}`)
    return (
      <section>
        <ReactPlayer ref={(value) => { this.reactPlayer = value }}
          url={this.state.url}
          onPause={this.onPlayerPause.bind(this)}
          onPlay={this.onPlayerPlay.bind(this)}
          playing={this.state.isPlaying}
          controls={true} />
      </section>)
  }

}

NetworkedVideoPlayer.propTypes = {
  roomId: PropTypes.string,
  username: PropTypes.string
}

const ranks = {
  member: 0,
  admin: 1
}

