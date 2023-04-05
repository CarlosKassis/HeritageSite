import React, { Component, useEffect, useState } from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Counter } from './components/Counter';

import './custom.css'

export default function App() {

    const [language, setLanguage] = useState('en');

    useEffect(() => {
        setLanguage('es');
    }, []);

    function onClickLanguage() {
        setLanguage(language == "en" ? "es" : "en");
    }

    return (
        <Layout onClickLanguage={onClickLanguage} language={language}>
            <Route exact path='/' render={() => <Home language={language} />} />
            <Route exact path='/counter' component={Counter} />
        </Layout>
    );
}
