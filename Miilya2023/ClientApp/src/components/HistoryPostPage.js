import React, { useEffect, useState, useRef } from 'react';
import LocalizedStrings from 'localized-strings';
import MiilyaApi from '../MiilyaApi';

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
        MiilyaApi.getHistoryPosts(props.loginInfo.jwt).then(historyPosts => {

            if (historyPosts) {
                setHistoryPosts(historyPosts);
            }
        });
    }, []);

    useEffect(() => {
        for (const historyPost of historyPosts) {
            MiilyaApi.getHistoryImage(props.loginInfo.jwt, historyPost.ImageName).then(historyImage => {
                if (historyImage) {
                    const newHistoryImages = { ...historyImages };
                    newHistoryImages[historyPost.ImageName] = URL.createObjectURL(historyImage);
                    setHistoryImages(newHistoryImages);
                }
            });
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
                        {historyImages[historyPost.ImageName] && <img className={"history-post-image"} alt={historyPost.ImageName} src={historyImages[historyPost.ImageName]} />}
                        <h5 style={{ paddingTop: '20px' }}>{historyPost.Description}</h5>
                    </div>
                ))
            }
        </div>
    );
}