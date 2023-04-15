import React, { useEffect, useState } from 'react';
import LocalizedStrings from 'localized-strings'
import { HistoryPostPage } from './HistoryPostPage';

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

    return (
        <div style={{ height: '90vh', width: '100%' }} className="middle-east">
            <h1 style={{ textAlign: 'center' }}><b>{getString(props.language, 'title')}</b></h1>
            <HistoryPostPage />
        </div>
    );
}