import React, { useEffect } from 'react';
import { Redirect } from 'react-router';

export function MicrosoftLogin(props) {

    useEffect(() => {
        console.log('aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa')
        const getToken = async () => {
            try {
                const tokenResponse = await msalInstance.acquireTokenPopup({
                    scopes: ['user.read'],
                });
                // Use the token to call an API or perform other authorized actions
                console.log(tokenResponse.accessToken);
            } catch (error) {
                console.log(error);
            }
        };

        getToken();
    }, []);


    return (
        <div></div>
    );
}