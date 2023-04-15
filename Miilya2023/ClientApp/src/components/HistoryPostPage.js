import React, { useEffect, useState } from 'react';
import LocalizedStrings from 'localized-strings';

export function HistoryPostPage(props) {

    const [historyPosts, setHistoryPosts] = useState([]);

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
        fetch('PrivateHistory/HistoryPost', {
            headers: {
                'Authorization': props.loginInfo.jwt
                }
            })
            .then(response => response.json())
            .then(data => {
                setHistoryPosts(data);
            })
            .catch(error => { });
    }, []);

    return (
        <div style={{
            alignContent: 'center',
            display: 'block'
        }}>
            {
                historyPosts.map((historyPost) => (
                    <div key={historyPost.Index} style={{
                        marginLeft: 'auto',
                        marginRight: 'auto',
                        marginTop: '20px',
                        marginBottom: '20px',
                        width: 'fit-content',
                        padding: '20px', borderRadius: '10px',
                        boxShadow: '5px 5px 10px rgba(0, 0, 0, 0.5)' } }>
                        <h3 style={{ padding: '10px' }}>{historyPost.Title}</h3>

                        <img
                            style={{
                                width: '100%',
                                maxHeight: '50vh',
                                boxShadow: '2px 2px 4px rgba(0, 0, 0, 0.5)'
                            }}
                            alt={"asd"} src={`./PrivateHistory/Media/Images/${historyPost.ImageName}`} />
                        <h5 style={{ paddingTop: '20px' }}>{historyPost.Description}</h5>
                    </div>
                ))
            }
        </div>
    );
}