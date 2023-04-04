import React, { useEffect, useState } from 'react';
import { DragonWrapper } from './DragonWrapper';
import LocalizedStrings from 'react-localization';
import data from './Persistance'

export function Home() {

    const [strings, setStrings] = useState({
        aaa: new LocalizedStrings({
            en: {
                greeting: 'Hello',
                message: 'Welcome to my website!',
                buttonLabel: 'Click me!',
            },
            es: {
                greeting: 'Hola',
                message: '¡Bienvenido a mi sitio web!',
                buttonLabel: 'Haz clic aquí',
            },
        })
    });

    useEffect(() => {
        data.loggedIn = true;
    }, []);

    return (
        <div style={{ height: '90vh', width: '100%' }}>
            <h1>{strings.aaa.message}</h1>
            <DragonWrapper />
        </div>
    );
}