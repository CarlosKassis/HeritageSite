import React, { useEffect, useState } from 'react';
import LocalizedStrings from 'localized-strings'
import { Link } from 'react-router-dom'
import MyAPI from '../MyAPI';

export function Families({ loginInfo }) {

    const [families, setFamilies] = useState([]);

    useEffect(() => {
        if (!loginInfo.loggedIn) {
            return;
        }
        MyAPI.getFamilies(loginInfo.jwt).then(families => {
            if (families) {
                setFamilies(families);
            }
        });
    }, [loginInfo]);

    return (
        <div style={{ alignContent: 'center', display: 'block' }}>
            {
                families.map((family) => (
                    <Link key={family.Identifier} to={`./FamilyTree/${family.Identifier}`} onClick={(event) => !family.Available && event.preventDefault()}>
                        <div
                            className={'family-hoverable'}
                            style={{
                                backgroundColor: family.Available ? '#2196f3' : '#444',
                                margin: '20px auto 20px auto',
                                padding: '20px',
                                borderRadius: '5px',
                                width: 'fit-content',
                                boxShadow: '5px 5px 10px rgba(0, 0, 0, 0.5)'
                            }}>
                            <h3 style={{ textAlign: 'center' }}>{family.Name}</h3>
                        </div>
                    </Link>
                ))
            }
        </div>
    );
}