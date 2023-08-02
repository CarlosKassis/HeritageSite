import React, { useEffect, useState } from 'react';

export function HistoryPostForGrid({ imageUrl, imageName, onClickHistoryImage }) {

    return (
        <div className={'floating'} style={{ maxWidth: '200px' }} >
            {
                // Image
                imageUrl &&
                <img
                    onClick={() => onClickHistoryImage(imageName)}
                    className={"history-post-image"}
                    src={imageUrl}
                    style=
                    {
                       {
                            height: '200px',
                            width: '200px',
                            objectFit: 'cover',
                            margin: '0px'
                       }
                    }
                />
            }
            {
                // Image loading
                !imageUrl &&
                <div style={{ backgroundColor: '#ccc', height: '200px', paddingTop: '100px' }}>
                    <h3 style={{ direction: 'ltr', textAlign: 'center', verticalAlign: 'middle' }}>Loading...</h3>
                </div>
            }
        </div>
    );
}