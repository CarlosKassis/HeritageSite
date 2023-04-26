import React, { useEffect, useState } from 'react';
import LocalizedStrings from 'localized-strings';
import MyAPI from '../MyAPI';

export function HistoryPost({ loginInfo, imageName, title, index, description, myPost }) {
    const [imageUrl, setImageUrl] = useState(null);
    const [bookmarked, setBookmarked] = useState(false);

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
        if (!loginInfo.loggedIn) {
            return;
        }

        if (!imageName) {
            return;
        }

        MyAPI.getHistoryImageLowRes(loginInfo.jwt, imageName).then(historyImageResponse => {
            if (historyImageResponse) {
                setImageUrl(URL.createObjectURL(historyImageResponse));
            }
        });
    }, [imageName, loginInfo])

    function onClickHistoryImage(imageName) {
        MyAPI.getHistoryImage(loginInfo.jwt, imageName)
            .then(historyImage => {
                const url = URL.createObjectURL(historyImage);
                window.open(url);
            })
    }

    return (
        <div
            key={index}
            className={"history-post"}
            style=
            {{
                backgroundColor: myPost == true ? '#dfd' : 'white'
            }}>

            {
                <div style={{
                    width: 'fit-content',
                    position: 'relative',
                    marginRight: 'auto',
                    marginBlock: '5px',
                    height: '36px'
                }}>
                    {
                        // Delete post
                        myPost &&
                        <img
                            className={"history-post-button"}
                            src={'./delete.png'}/>
                    }
                    {
                        // Bookmark post
                        <img
                            className={"history-post-button"}
                            onClick={() => { setBookmarked(!bookmarked); }}
                            src={bookmarked ? './bookmarked.png' : './bookmark.png'}/>
                    }
                </div>
            }

            {
                // Title
                title && <h5 style={{ overflowWrap: 'break-word' }}>{title}</h5> ||
                !title && <br/>
            }
            {
                // Image
                imageUrl &&
                <img
                    onClick={() => onClickHistoryImage(imageName)}
                    className={"history-post-image"}
                    src={imageUrl}
                    alt={imageName}
                />
            }
            {
                // Image loading
                imageName && !imageUrl &&
                <div style={{ backgroundColor: '#ccc', height: '400px', paddingTop: '200px' }}>
                    <h3 style={{ direction: 'ltr', textAlign: 'center', verticalAlign: 'middle' }}>Loading...</h3>
                </div>
            }

            {
                // Description
                <h6 style={{ paddingTop: '10px', overflowWrap: 'break-word' }}>{description}</h6>
            }
        </div>
    );
}