import React, { useEffect, useState, useRef } from 'react';
import LocalizedStrings from 'localized-strings';
import MiilyaApi from '../MiilyaApi';

export function HistoryPostPage(props) {

    const [historyPosts, setHistoryPosts] = useState([]);
    const [images, setImages] = useState(null); 

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
                console.log(data)
                setHistoryPosts(data);
            })
            .catch(error => { });

        fetch(`PrivateHistory/Media/Images/Village.jpg`, {
            headers: {
                'Authorization': props.loginInfo.jwt
            }
        })
            .then(response => response.blob())
            .then(blob => {
                console.log(blob);
                setImages(URL.createObjectURL(blob));
            });
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

                        { images && <img
                            style={{
                                width: '100%',
                                maxHeight: '50vh',
                                boxShadow: '2px 2px 4px rgba(0, 0, 0, 0.5)'
                            }}
                            alt={"asd"} src={images} />}
                        <h5 style={{ paddingTop: '20px' }}>{historyPost.Description}</h5>
                    </div>
                ))
            }
        </div>
    );
}