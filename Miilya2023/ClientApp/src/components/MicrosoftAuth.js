import React from "react";

function MicrosoftAuth(props) {
    const handleLoginClick = async () => {
        try {
            const request = {
                scopes: ["user.read"],
            };
            const authResult = await props.msalInstance.loginPopup(request);
            console.log(authResult);

            // Use the access token to call an API or perform other authorized actions
        } catch (error) {
            console.log(error);
        }
    };

    return (
        <div>
            <button onClick={handleLoginClick}>Login with Microsoft</button>
        </div>
    );
}

export default MicrosoftAuth;
