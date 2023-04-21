import React from "react";
import MyAPI from "../MyAPI";

function MicrosoftAuth(props) {
    const handleLoginClick = async () => {
            props.msalInstance.loginPopup({
                scopes: ['user.read']
            }).then(response => {
                console.log(response);
            }).catch(error => {
                console.error(error);
            });
    };

    return (
        <div>
            <button onClick={handleLoginClick}>Login with Microsoft</button>
        </div>
    );
}

export default MicrosoftAuth;
