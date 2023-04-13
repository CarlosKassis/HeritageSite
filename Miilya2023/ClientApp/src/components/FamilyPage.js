import React, { useEffect, useState } from 'react';
import { DragonWrapper } from './DragonWrapper';
import LocalizedStrings from 'localized-strings'

export function FamilyPage(props) {

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
        <div style={{ height: '90vh', width: '100%' }}>
            <DragonWrapper />
        </div>
    );
}