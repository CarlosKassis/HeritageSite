import React, { useEffect } from 'react';
import OpenSeadragon from 'openseadragon';

export function DragonWrapper() {

    useEffect(() => {
        OpenSeadragon({
            id: "dragon1",
            prefixUrl: "./openseadragon/images/",
            tileSources: "PrivateHistory/Media/Families/Layous/Layous.dzi",
            ajaxWithCredentials: true,
            loadTilesWithAjax: true,
            ajaxHeaders: {
                Authorization: `Bearer ASD`
            }
        });
    }, []);

    return (
        <div id="dragon1" style={{ width: '100%', height: '100%', backgroundColor: 'black' }}>
        </div>
    );
}