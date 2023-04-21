import React, { Component, useEffect, useState } from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import './custom.css'
import { Families } from './components/Families';
import { FamilyTree } from './components/FamilyTree';
import Cookies from 'universal-cookie';
import MyAPI from './MyAPI';
import { Logout } from './components/Logout';
import * as msal from "msal";

export default function App() {

    const cookies = new Cookies();
    const [language, setLanguage] = useState('ar');
    const [loginInfo, setLoginInfo] = useState({ loggedIn: false })

    useEffect(() => {
        setLanguage('ar');

        // Validate login token and try set state to logged in
        const loginJwt = cookies.get(`login-jwt`);
        if (loginJwt) {
            MyAPI.validateLoginJwt(loginJwt).then(validJwt => {
                if (validJwt) {
                    onValidLoginJwt(loginJwt);
                } else {
                    cookies.remove('login-jwt', { path: '/' });
                }
            }).catch(ex => {
                cookies.remove('login-jwt', { path: '/' });
            })
        }

    }, []);

    function onValidLoginJwt(loginJwt, setCookie) {
        setLoginInfo({ loggedIn: true, jwt: loginJwt });
        if (setCookie) {
            cookies.set(`login-jwt`, loginJwt, { path: '/' });
        }
    }

    function onClickLanguage() {
        setLanguage(language == "ar" ? "he" : "ar");
    }

    function onLogOut() {
        cookies.remove('login-jwt', { path: '/' });
        setLoginInfo({ loggedIn: false });
    }

    const [msalInstance, setMsalInstance] = useState(null);

    useEffect(() => {

        const msalConfig = {
            auth: {
                clientId: "3ee7d2ed-3ea7-4790-b63c-e06ccd058189",
                redirectUri: 'https://10.0.0.14:5001/',
            }
        };

        setMsalInstance(new msal.UserAgentApplication(msalConfig))
    }, [])

    return (
        <Layout loginInfo={loginInfo} onClickLanguage={onClickLanguage} language={language}>
            <Route exact path='/' render={() => <Home msalInstance={msalInstance} onLogOut={onLogOut} loginInfo={loginInfo} language={language} onLogin={(loginJwt) => onValidLoginJwt(loginJwt, true)} />} />
            <Route exact path='/PrivateHistory/Families' render={() => <Families loginInfo={loginInfo} language={language} />} />
            <Route path='/PrivateHistory/FamilyTree/:id' render={() => <FamilyTree loginInfo={loginInfo} language={language} />} />
            <Route exact path='/Logout' render={() => <Logout onLogOut={onLogOut} />} />
        </Layout>
    );
}
