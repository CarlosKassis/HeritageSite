import React from "react";
import MyAPI from "../MyAPI";

function MicrosoftAuth(props) {
    const handleLoginClick = async () => {
            props.msalInstance.loginPopup({
                scopes: ['user.read']
            }).then(response => {
                return MyAPI.getLoginJwtFromMicrosoftJwt(response.idToken.rawIdToken);
            }).then(loginJwt => {
                props.onLogin(loginJwt);
            }).catch(error => {
                console.error(error);
            });
    };

    return (
        <div className={"login-button-container"}>
            {
            <button onClick={handleLoginClick}>Login with Microsoft</button>
            }
        </div>
    );
}

export default MicrosoftAuth;
