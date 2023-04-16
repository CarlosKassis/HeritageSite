import React, { Component, useEffect, useState } from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import './custom.css'
import { Families } from './components/Families';
import { FamilyTree } from './components/FamilyTree';
import Cookies from 'universal-cookie';
import MiilyaApi from './MiilyaApi';

export default function App() {

    const cookies = new Cookies();
    const [language, setLanguage] = useState('ar');
    const [loginInfo, setLoginInfo] = useState({ loggedIn: false })

    useEffect(() => {
        setLanguage('ar');

        // Validate login token and try set state to logged in
        const loginJwt = cookies.get(`login-jwt`);
        if (loginJwt) {
            if (MiilyaApi.validateLoginJwt(loginJwt)) {
                console.log('Successful login JWT')
                onValidLoginJwt(loginJwt);
            } else {
                console.log('Invalid login JWT')
                cookies.remove('login-jwt', { path: '/' });
            }
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
        console.log('qqqqqqqqqq');
        cookies.remove('login-jwt', { path: '/' });
        setLoginInfo({ loggedIn: false });
    }

    return (
        <Layout loginInfo={loginInfo} onClickLanguage={onClickLanguage} language={language}>
            <Route exact path='/' render={() => <Home onLogOut={onLogOut} loginInfo={loginInfo} language={language} onLogin={(loginJwt) => onValidLoginJwt(loginJwt, true)} />} />
            <Route exact path='/PrivateHistory/Families' render={() => <Families loginInfo={loginInfo} language={language} />} />
            <Route path='/PrivateHistory/FamilyTree/:id' render={() => <FamilyTree loginInfo={loginInfo} language={language} />} />
            <Route exact path='/Logout' render={() => <Home logout={true} onLogOut={onLogOut} loginInfo={loginInfo} language={language} onLogin={(loginJwt) => onValidLoginJwt(loginJwt, true)} />} />
        </Layout>
    );
}
