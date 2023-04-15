import React, { Component, useEffect, useState } from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import './custom.css'
import { Families } from './components/Families';
import { FamilyTree } from './components/FamilyTree';

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
            <Route exact path='/PrivateHistory/Families' render={() => <Families language={language} />} />
            <Route path='/PrivateHistory/FamilyTree:id' render={() => <FamilyTree language={language} />} />
        </Layout>
    );
}
