import React, { useEffect } from 'react';
import LocalizedStrings from 'localized-strings'
import { useParams } from "react-router-dom"
import { BalkanFamilyTreeWrapper } from './BalkanFamilyTreeWrapper';

export function FamilyTree({ loginInfo }) {

    const { id } = useParams();

    return (
        <div style={{ height: '90vh', width: '100%' }}>
            {
                id && loginInfo.loggedIn && <BalkanFamilyTreeWrapper loginInfo={loginInfo} familyId={ id } />
            }
        </div>
    );
}