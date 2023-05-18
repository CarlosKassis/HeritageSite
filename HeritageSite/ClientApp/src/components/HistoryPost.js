import React, { useEffect, useState } from 'react';
import LocalizedStrings from 'localized-strings';
import MyAPI from '../MyAPI';

export function HistoryPost({ loginInfo, imageName, title, index, description, control, onClickBookmarkPost, bookmarked, onDeletePost, getImageUrl, containerViewMode }) {
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
        if (!loginInfo.loggedIn) {
            return;
        }

        if (!imageName) {
            return;
        }

        getImageUrl(imageName, onAquireImageUrl);
    }, [imageName, loginInfo])

    function onAquireImageUrl(imageUrl) {
        if (imageUrl) {
            setImageUrl(imageUrl);
        }
    }

    function onClickHistoryImage(imageName) {
        MyAPI.getHistoryImage(loginInfo.jwt, imageName)
            .then(historyImage => {
                const url = URL.createObjectURL(historyImage);
                window.open(url);
            });
    }

    function onClickBookmark() {
        onClickBookmarkPost(index);
    }

    function onClickDelete() {
        if (!control) {
            return;
        }

        const result = confirm("هل تريد ان تحذف المنشور?");
        if (!result) {
            return;
        }

        MyAPI.deleteHistoryPost(loginInfo.jwt, index).then(() => {
            onDeletePost(index);
        }).catch(ex => {
            console.log(ex);
        })
    }

    return (
        <div key={index}>
            <div

                className={`card ${containerViewMode == "grid" ? 'history-post-gridcell' : ''}`}
                style=
                {{
                    backgroundColor: (!control || control == 0) ? 'white' : (control == 1 ? '#dfd' : '#fdd')
                }}>
                {
                    containerViewMode != "grid" &&
                    <div style={{
                        width: 'fit-content',
                        position: 'relative',
                        marginRight: 'auto',
                        height: '36px'
                    }}>
                        {
                            // Edit post
                            //control !== null && control > 0 &&
                            //<img
                            //    className={"history-button"}
                            //    src={'./edit.png'} />
                        }
                        {
                            // Delete post
                            control !== null && control > 0 &&
                            <img
                                onClick={onClickDelete}
                                className={"history-button"}
                                src={'./delete.png'} />
                        }
                        {
                            // Bookmark post
                            <img
                                className={"history-button"}
                                onClick={onClickBookmark}
                                src={bookmarked ? './bookmarked.png' : './bookmark.png'} />
                        }
                    </div>
                }

                {
                    containerViewMode != "grid" &&
                    // Title
                    title && <h5 style={{ overflowWrap: 'break-word' }}>{title}</h5> ||
                    !title && <br />
                }
                {
                    // Image
                    imageUrl &&
                    <img
                        onClick={() => onClickHistoryImage(imageName)}
                        className={"history-post-image"}
                        src={imageUrl}
                        alt={imageName}
                        style=
                        {
                            containerViewMode == "grid" ? {
                                maxHeight: '200px',
                                marginTop: 'auto',
                                marginBottom: 'auto'
                            } : {}
                        }
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
                    containerViewMode != "grid" &&
                    // Description
                    <h6 style={{ paddingTop: '10px', overflowWrap: 'break-word' }}>{description}</h6>
                }
            </div >
        </div>
    );
}