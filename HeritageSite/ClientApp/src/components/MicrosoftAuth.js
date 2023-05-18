import React from "react";
import MyAPI from "../MyAPI";

function MicrosoftAuth({ msalInstance, onLogin }) {
    const handleLoginClick = async () => {
            msalInstance.loginPopup({
                scopes: ['user.read']
            }).then(response => {
                return MyAPI.getLoginJwtFromMicrosoftJwt(response.idToken.rawIdToken);
            }).then(loginJwt => {
                onLogin(loginJwt);
            }).catch(error => {
                console.error(error);
            });
    };

    return (
        <div className={"login-button-container"}>
            <div onClick={handleLoginClick}>
                <img src={"/microsoft-login.svg"} alt={"Microsoft Login"} />
            </div>
        </div>
    );
}

export default MicrosoftAuth;
