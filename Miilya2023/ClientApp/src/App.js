import React, { Component, useEffect, useState } from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Counter } from './components/Counter';

import './custom.css'
import { FamilyPage } from './components/FamilyPage';

export default function App() {

    const [language, setLanguage] = useState('ar');

    useEffect(() => {
        setLanguage('ar');
    }, []);

    function onClickLanguage() {
        setLanguage(language == "ar" ? "he" : "ar");
    }

    return (
        <Layout onClickLanguage={onClickLanguage} language={language}>
            <Route exact path='/' render={() => <Home language={language} />} />
            <Route exact path='/Family' render={() => <FamilyPage language={language} />} />
            <Route exact path='/counter' component={Counter} />
        </Layout>
    );
}
