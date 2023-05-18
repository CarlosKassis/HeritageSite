import React, { useEffect, useRef } from 'react';
import LocalizedStrings from 'localized-strings'
import { InfinitePage } from './InfinitePage';
import { HistoryPostsContainer } from './HistoryPostsContainer'
import GoogleAuth from './GoogleAuth';
import MicrosoftAuth from './MicrosoftAuth';

export function Home({ loginInfo, logout, onLogOut, onLogin, language, msalInstance, getImageUrl }) {

    // Handle logout URL
    useEffect(() => {
        if (logout) {
            onLogOut();
        }
    }, [])

    return (
        <div style={{ marginLeft: 'auto', marginRight: 'auto', height: '90vh', width: '100%' }} className="middle-east">
            {
                !loginInfo.initialVariableState && !loginInfo.loggedIn &&
                <div>
                    <GoogleAuth onLogin={(loginJwt) => onLogin(loginJwt)} />
                    {msalInstance && < MicrosoftAuth onLogin={(loginJwt) => onLogin(loginJwt)} msalInstance={msalInstance} />}
                </div>
            }
            {
                loginInfo.loggedIn &&
                <InfinitePage loginInfo={loginInfo}>
                    <HistoryPostsContainer getImageUrl={getImageUrl} />
                </InfinitePage>
            }

        </div>
    );
}
