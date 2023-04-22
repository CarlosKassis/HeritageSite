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

        MyAPI.getHistoryImageLowRes(props.loginInfo.jwt, props.imageName).then(historyImageResponse => {
            if (historyImageResponse) {
                setImageUrl(URL.createObjectURL(historyImageResponse));
            }
        });
    }, [props.imageName, props.loginInfo])

    function onClickHistoryImage(imageName) {
        MyAPI.getHistoryImage(props.loginInfo.jwt, imageName)
            .then(historyImage => {
                const url = URL.createObjectURL(historyImage);
                window.open(url);
            })
    }

    return (
        <div key={props.index} className={"history-post"}>
            <h3 style={{ padding: '10px' }}>{props.title}</h3>
            {
                imageUrl &&
                <img
                    onClick={() => onClickHistoryImage(props.imageName)}
                    className={"history-post-image"}
                    src={imageUrl}
                    alt={props.imageName}
                />
            }
            {
                !imageUrl &&
                <div style={{ backgroundColor: '#ccc', height: '400px', paddingTop: '200px' }}>
                    <h3 style={{ direction: 'ltr', textAlign: 'center', verticalAlign: 'middle' }}>Loading...</h3>
                </div>
            }
            <h6 style={{ paddingTop: '20px', overflowWrap: 'break-word' }}>{props.description}</h6>
        </div>
    );
}