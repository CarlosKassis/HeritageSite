import React, { useEffect } from 'react';
import LocalizedStrings from 'localized-strings'
import { InfinitePage } from './InfinitePage';
import { HistoryPostsContainer } from './HistoryPostsContainer'
import GoogleAuth from './GoogleAuth';
import MicrosoftAuth from './MicrosoftAuth';

export function Home(props) {

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
        if (props.logout) {
            props.onLogOut();
        }
    }, [])

    return (
        <div style={{ marginLeft: 'auto', marginRight: 'auto', height: '90vh', width: '100%' }} className="middle-east">
            <h1 style={{ textAlign: 'center' }}><b>{getString(props.language, 'title')}</b></h1>
            {
                !props.loginInfo.loggedIn &&
                <div>
                    <GoogleAuth onLogin={(loginJwt) => props.onLogin(loginJwt)} />
                    { props.msalInstance && < MicrosoftAuth msalInstance={props.msalInstance} />}
                </div>
            }
            {
                props.loginInfo.loggedIn &&
                <InfinitePage loginInfo={props.loginInfo}>
                        <HistoryPostsContainer />
                    </InfinitePage>
            }
        </div>
    );
}
