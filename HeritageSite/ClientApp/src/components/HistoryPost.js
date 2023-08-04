import React, { useEffect, useState } from 'react';
import LocalizedStrings from 'localized-strings';
import MyAPI from '../MyAPI';
import { HistoryPostForGrid } from './HistoryPostForGrid';
import { HistoryPostForVertical } from './HistoryPostForVertical';

export function HistoryPost({ loginInfo, imageName, imageDate, title, index, description, control, onClickBookmarkPost, bookmarked, onDeletePost, getImageUrl, containerViewMode }) {
    const [imageUrl, setImageUrl] = useState(null);

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

        const result = confirm("Do you want to remove this post?");
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
            {containerViewMode == 'grid' && <HistoryPostForGrid imageName={imageName} imageUrl={imageUrl} onClickHistoryImage={onClickHistoryImage} />}
            {containerViewMode != 'grid' && <HistoryPostForVertical
                imageUrl={imageUrl}
                imageDate={imageDate}
                control={control}
                description={description}
                title={title}
                bookmarked={bookmarked}
                onClickDelete={onClickDelete}
                onClickBookmark={onClickBookmark}
                imageName={imageName}
                onClickHistoryImage={onClickHistoryImage}
            />}
        </div>
    );
}