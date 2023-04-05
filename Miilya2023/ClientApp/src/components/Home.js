import React, { useEffect, useState } from 'react';
import { DragonWrapper } from './DragonWrapper';
import LocalizedStrings from 'localized-strings'

export function Home(props) {

    const strings = new LocalizedStrings({
        en: {
            title: 'Welcome to my website!'
        },
        es: {
            title: '¡Bienvenido a mi sitio web!'
        },
    });

    function getString(language, str) {
        strings.setLanguage(language);
        return strings[str];
    }


    return (
        <div style={{ height: '90vh', width: '100%' }}>
            <h1>{getString(props.language, 'title')}</h1>
            <DragonWrapper />
        </div>
    );
}