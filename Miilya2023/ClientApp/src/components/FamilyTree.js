import React, { useEffect } from 'react';
import LocalizedStrings from 'localized-strings'
import { useParams } from "react-router-dom"
import { BalkanFamilyTreeWrapper } from './BalkanFamilyTreeWrapper';

export function FamilyTree({ loginInfo }) {

    const { id } = useParams();
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
            {
                id && loginInfo.loggedIn && <BalkanFamilyTreeWrapper loginInfo={loginInfo} familyId={ id } />
            }
        </div>
    );
}