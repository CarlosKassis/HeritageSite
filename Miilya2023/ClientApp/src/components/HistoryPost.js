import React, { useEffect, useState } from 'react';
import LocalizedStrings from 'localized-strings';
import MyAPI from '../MyAPI';

export function HistoryPost(props) {
    const [imageUrl, setImageUrl] = useState(null);

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
        if (!props.loginInfo.loggedIn) {
            return;
        }

        if (!props.imageName) {
            return;
        }

        MyAPI.getHistoryImage(props.loginInfo.jwt, props.imageName).then(historyImageResponse => {
            if (historyImageResponse) {
                setImageUrl(URL.createObjectURL(historyImageResponse));
            }
        });
    }, [props.imageName, props.loginInfo])

    return (
        <div key={props.index} className={"history-post"}>
            <h3 style={{ padding: '10px' }}>{props.title}</h3>
            {
                imageUrl &&
                <img className={"history-post-image"} alt={props.imageName} src={imageUrl} />
            }
            <h5 style={{ paddingTop: '20px' }}>{props.description}</h5>
        </div>
    );
}