import React, { useEffect } from 'react';
import { DragonWrapper } from './DragonWrapper';
import LocalizedStrings from 'localized-strings'
import { useParams } from "react-router-dom"
 
export function FamilyTree(props) {

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
                id && props.loginInfo.loggedIn && <DragonWrapper loginInfo={props.loginInfo} familyId={id} />
            }
        </div>
    );
}