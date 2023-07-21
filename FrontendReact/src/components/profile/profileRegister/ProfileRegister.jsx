import React, { Component } from 'react';

import {
    Button,
} from 'react-bootstrap';

import './ProfileRegister.css';
import { apiConfig } from "../../../utils/authConfig";
import * as signalR from '@aspnet/signalr';

class ProfileRegister extends Component {
    signalRInitiated = false;
    constructor(props){
        super(props);
        this.state = { signalRMessages: [] };
    }
    
    handleRegister = () => {
        this.response = this.props.testProfile();
    }

    render() {
        const { signalRMessages } = this.state;
        if(this.props.auth.accessToken && !this.signalRInitiated){
            this.initSignalR(this.props.auth.accessToken);
            this.signalRInitiated = true;
        };
        

        return (
            <div className="p-register">
                <h3>Welcome Onboard!</h3>
                <p>You will now be asked to update your profile information.</p>
                <Button variant="primary" onClick={this.handleRegister}>Accept</Button>
                <p>{this.response}</p>

                <div>
               
                {this.state.signalRMessages.map(function(d, idx){
                    return (<p key={idx}>{d}</p>)
                })}
                </div>
                
            </div>

            
        );
    }

    initSignalR(accessToken) {

        console.log('entered init ' + accessToken);

        var options = { accessTokenFactory: () => accessToken };
        var hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(apiConfig.baseUri + '/notify', options)
            .configureLogging(signalR.LogLevel.Information)
            //.withAutomaticReconnect()
            .build();

        hubConnection.serverTimeoutInMilliseconds = 300000;


        hubConnection.start().then(function () {
            console.log("Successfully started signalrhub connection");
        }).catch(function (err) {
            console.error("ERROR: Starting SignalRHub Connection....." + err.toString());
        });
        var that = this;
        hubConnection.on('MessageReceived', (message) => {
            var msgs = this.state.signalRMessages;
            console.log('msgs: ' + msgs);
            msgs.push(message.message);
            that.setState({ signalRMessages: [] });
            that.setState({ signalRMessages: msgs });
            console.log('state: ' + this.state.signalRMessages);
        });

        hubConnection.onclose((error) => {
            if (this.hubConnection) {
                this.hubConnection.start();
            }
            console.error(`hubConnection.onclose - Something went wrong: ${error}`);
        });
    };
}

export default ProfileRegister;