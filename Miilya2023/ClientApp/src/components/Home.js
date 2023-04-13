import React, { useEffect, useState } from 'react';
import { DragonWrapper } from './DragonWrapper';
import LocalizedStrings from 'localized-strings'
import { Families } from './Families';

export function Home(props) {

    const strings = new LocalizedStrings({
        ar: {
            title: 'موقع تراث معليا'
        },
        he: {
            title: 'אתר מורשת מעיליא'
        },
    });

    useEffect(() => {
        console.log('testt')
        fetch('PrivateHistory/Image')
            .then(response => response.json())
            .then(data => {
                //console.log(data);
                // callback function to handle the data
                //handleData(data);
            });
    }, []);

    function getString(language, str) {
        strings.setLanguage(language);
        return strings[str];
    }

    return (
        <div style={{ height: '90vh', width: '100%' }} className="middle-east">
            <h1>{getString(props.language, 'title')}</h1>
            <Families language={props.language} />
        </div>
    );
}