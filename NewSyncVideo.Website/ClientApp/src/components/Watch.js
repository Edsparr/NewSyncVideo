import React, { Component } from 'react';
import ReactPlayer from 'react-player'
import NetworkedVideoPlayer from './NetworkedVideoPlayer';

export class FetchData extends Component {
  displayName = FetchData.name

  constructor(props) {
    super(props);
    this.handlePlayVideoClick = this.handlePlayVideoClick.bind(this);
    this.handleVideoUrlChanged = this.handleVideoUrlChanged.bind(this);

    this.state = {
      url: null,
      currentInputUrl: null,
    }
  }

  componentDidMount() {

  }

  handlePlayVideoClick() {
    this.player.queueVideo(this.state.currentInputUrl);

    this.setState(state => {
      return {
        url: state.currentInputUrl,
        currentInputUrl: null
      }
    });
  }


  handleVideoUrlChanged(e) {
    this.setState({
      currentInputUrl: e.target.value
    });
    console.log(e.target.value);
  }

  render() {
    let participants = null;

    if (this.player != null && this.player.state.userRoomInfo !== null) {
      participants = this.player.state.userRoomInfo.room.participants.map(c => {
        <p>{c.name} ({c.id})</p>
      });
    }

    return (
      <div>
        <input type="text" onChange={this.handleVideoUrlChanged} value={this.currentInputUrl} placeholder="Enter video url" />
        <button onClick={this.handlePlayVideoClick}>Add Video</button>
        <button onClick={this.pause}>Pause</button>
        {participants}

        <NetworkedVideoPlayer
          roomId="XXX"
          username={`Name: ${Math.ceil(Math.random() * 10)} `}
          ref={value => { this.player = value; } } />

      </div>
    );
  }
}
