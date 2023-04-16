import React, { useEffect, useState } from 'react';
import LocalizedStrings from 'localized-strings'
import { HistoryPostPage } from './HistoryPostPage';
import GoogleAuth from './GoogleAuth';

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

    useEffect(() => {
        if (props.logout) {
            props.onLogOut();
        }
    }, [])

    return (
        <div onClick={(e) => console.log(e)} style={{ marginLeft: 'auto', marginRight: 'auto', height: '90vh', width: '100%' }} className="middle-east">
            <h1 style={{ textAlign: 'center' }}><b>{getString(props.language, 'title')}</b></h1>
            {!props.loginInfo.loggedIn && <GoogleAuth onLogin={(loginJwt) => props.onLogin(loginJwt)} />}
            {props.loginInfo.loggedIn && < HistoryPostPage loginInfo={props.loginInfo} />}
        </div>
    );
}