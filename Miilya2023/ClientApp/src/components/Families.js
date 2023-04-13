import React, { useEffect, useState } from 'react';
import { DragonWrapper } from './DragonWrapper';
import LocalizedStrings from 'localized-strings'

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
        fetch('PrivateHistory/Family/All')
            .then(response => response.json())
            .then(data => {
                setFamilies(data);
            })
            .catch(error => {});
    }, []);

    return (
        <div style={{ alignContent: 'center', display: 'block' }}>
            {
                families.map((family) => (
                    <div key={family.identifier}
                        onClick={() => console.log(family.name)}
                        className={ 'hoverable' }
                        style={{
                            margin: '20px auto 20px auto',
                            padding: '20px',
                            borderRadius: '10px',
                            maxWidth: '300px',
                            boxShadow: '5px 5px 10px rgba(0, 0, 0, 0.5)'
                        }}>
                        <h3 style={{ textAlign: 'center' }}>{family.name}</h3>
                    </div>
                ))
            }
        </div>
    );
}