import React, { PureComponent } from 'react';

import {
    Button,
} from 'react-bootstrap';

import './ProfileRegister.css'


class ProfileRegister extends PureComponent {

    handleRegister = (methodSuffix) => {
        this.props.initialize(methodSuffix);

        
    }

    render() {
        if(this.props.profile && this.props.profile.preferredLanguage){
            this.items = this.props.profile.preferredLanguage.map((item) =>
                <li>{item}</li>
            );
            }
            else{
                this.items = [];
            };
        return (
        <div className="p-register">
            <h3>Welcome Onboard!</h3>
            <p>
            <Button variant="primary" onClick={() => this.handleRegister('OnBehalfOfUser')}>On Behalf Of Flow</Button><span>&nbsp;&nbsp;&nbsp;</span>
            <Button variant="primary" onClick={() => this.handleRegister('ClientCredentialsFlow')}>Client Credentials Flow</Button>
            </p>
            <p>
            <Button variant="primary" onClick={() => this.handleRegister('ReadUsingDelegatedPermission')}>Read Using Delegated Permission</Button><span>&nbsp;&nbsp;&nbsp;</span>
            <Button variant="primary" onClick={() => this.handleRegister('WriteUsingDelegatedPermission')}>Write Using Delegated Permission</Button>
            </p>
            <p>
            <Button variant="primary" onClick={() => this.handleRegister('ReadUsingApplicationPermission')}>Read Using Application Permission</Button><span>&nbsp;&nbsp;&nbsp;</span>
            <Button variant="primary" onClick={() => this.handleRegister('WriteUsingApplicationPermission')}>Write Using Application Permission</Button>
            </p>
            <p>
            <Button variant="primary" onClick={() => this.handleRegister('ReadUsingViewerSecurityGroup')}>Read Using Viewer Security Group</Button><span>&nbsp;&nbsp;&nbsp;</span>
            <Button variant="primary" onClick={() => this.handleRegister('WriteUsingContributorSecurityGroup')}>Write Using Contributor SecurityGroup</Button>
            </p>
            <p></p>
            <p>
            <ul>{this.items}</ul>
            </p>
        </div>
        );
    }
}

export default ProfileRegister;