import React, { useEffect, useState, useRef } from 'react';
import LocalizedStrings from 'localized-strings';
import MyAPI from '../MyAPI';

export function HistoryPostPage(props) {

    const [historyPosts, setHistoryPosts] = useState([]);
    const [historyImages, setHistoryImages] = useState({}); 

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
        if (props.loginInfo.loggedIn) {
            MyAPI.getHistoryPosts(props.loginInfo.jwt).then(historyPosts => {
                if (historyPosts) {
                    setHistoryPosts(historyPosts);
                }
            });
        }
    }, [props.loginInfo]);

    useEffect(() => {
        for (const historyPost of historyPosts) {
            // Check if image was already fetched
            if (!historyImages[historyPost.ImageName]) {
                MyAPI.getHistoryImage(props.loginInfo.jwt, historyPost.ImageName).then(historyImage => {
                    // Failed fetch
                    if (historyImage) {
                        // Replace image dict state with new dict with additional image
                        const newHistoryImages = { ...historyImages };
                        newHistoryImages[historyPost.ImageName] = URL.createObjectURL(historyImage);
                        setHistoryImages(newHistoryImages);
                    }
                });
            }
        }
    }, [historyPosts]);

    return (
        <div style={{
            alignContent: 'center',
            display: 'block'
        }}>
            {
                historyPosts.map((historyPost) => (
                    <div key={historyPost.Index} className={"history-post"}>
                        <h3 style={{ padding: '10px' }}>{historyPost.Title}</h3>
                        {
                            historyImages[historyPost.ImageName] &&
                            <img className={"history-post-image"} alt={historyPost.ImageName} src={historyImages[historyPost.ImageName]} />
                        }
                        <h5 style={{ paddingTop: '20px' }}>{historyPost.Description}</h5>
                    </div>
                ))
            }
        </div>
    );
}