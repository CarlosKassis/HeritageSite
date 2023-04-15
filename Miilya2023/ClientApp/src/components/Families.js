﻿import React, { useEffect, useState } from 'react';
import LocalizedStrings from 'localized-strings'
import { Link } from 'react-router-dom'
import MiilyaApi from '../MiilyaApi';

export function Families(props) {

    const [families, setFamilies] = useState([]);

    const strings = new LocalizedStrings({
        ar: {
            families: 'عائلات'
        },
        he: {
            families: 'משפחות'
        },
    });

    function getString(language, str) {
        strings.setLanguage(language);
        return strings[str];
    }

    useEffect(() => {
        const families = MiilyaApi.getFamilies(props.loginInfo.jwt);
        if (families) {
            setFamilies(families);
        }
    }, []);

    return (
        <div style={{ alignContent: 'center', display: 'block' }}>
            {
                families.map((family) => (
                    <Link key={family.identifier} to={`./FamilyTree/${family.identifier}`}>
                        <div
                            className={'hoverable'}
                            style={{
                                margin: '20px auto 20px auto',
                                padding: '20px',
                                borderRadius: '10px',
                                maxWidth: '300px',
                                boxShadow: '5px 5px 10px rgba(0, 0, 0, 0.5)'
                            }}>
                            <h3 style={{ textAlign: 'center' }}>{family.name}</h3>
                        </div>
                    </Link>
                ))
            }
        </div>
    );
}