import React, { useEffect, useState } from 'react';

export function HistoryPostForVertical({ title, description, imageUrl, imageName, imageDate, bookmarked, control, onClickDelete, onClickBookmark, onClickHistoryImage }) {

    useEffect(() => {
        console.log(imageDate);
    });

    return (
        <div className={'card main-column'} style={{ height: '100%', width: '100%' }} >
            <div className={'padded'}
                style={{
                width: 'fit-content',
                position: 'relative',
                marginRight: 'auto',
                height: 'fit-content'
            }}>
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
            {title && <h6 className={'padded'} style={{ overflowWrap: 'break-word' }}>{title}</h6>}
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
                        {
                            width: '100%',
                            height: 'auto',
                            marginTop: 'auto',
                            marginBottom: 'auto'
                        }
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
            <input type="date" value={imageDate ? imageDate : "1700-01-01"} unselectable="on" style={{ fontWeight: 'bold', color: 'black', backgroundColor: 'white', fontSize: '18px', marginLeft: '6px', marginTop: '6px', userSelect: 'none', border: 'none' }} readOnly disabled></input>
            {description && <text className={'padded language-direction'} style={{ paddingTop: '10px' }}><pre style={{ fontSize: '16px' }} >{description}</pre></text>}
        </div>
    );
}