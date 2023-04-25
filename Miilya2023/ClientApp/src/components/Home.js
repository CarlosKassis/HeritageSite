import React, { useEffect } from 'react';
import LocalizedStrings from 'localized-strings'
import { InfinitePage } from './InfinitePage';
import { HistoryPostsContainer } from './HistoryPostsContainer'
import GoogleAuth from './GoogleAuth';
import MicrosoftAuth from './MicrosoftAuth';

export function Home({ loginInfo, logout, onLogOut, onLogin, language, msalInstance }) {

    const strings = new LocalizedStrings({
        ar: {
            title: 'موقع تراث معليا'
        },
        he: {
            title: 'אתר מורשת מעיליא'
        },
    });

    function getString(language, str) {
        strings.setLanguage(language);
        return strings[str];
    }


    // Handle logout URL
    useEffect(() => {
        if (logout) {
            onLogOut();
        }
    }, [])

    return (
        <div style={{ marginLeft: 'auto', marginRight: 'auto', height: '90vh', width: '100%' }} className="middle-east">
            {
                !loginInfo.loggedIn &&
                <div>
                    <GoogleAuth onLogin={(loginJwt) => onLogin(loginJwt)} />
                    {msalInstance && < MicrosoftAuth onLogin={(loginJwt) => onLogin(loginJwt)} msalInstance={msalInstance} />}
                </div>
            }
            {
                loginInfo.loggedIn &&
                <InfinitePage loginInfo={loginInfo}>
                        <HistoryPostsContainer />
                    </InfinitePage>
            }

        </div>
    );
}
