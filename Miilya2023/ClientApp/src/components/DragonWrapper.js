import React, { useEffect, Component } from 'react';
import OpenSeadragon from 'openseadragon';

export function DragonWrapper(familyId, loginInfo) {

    useEffect(() => {
        OpenSeadragon({
            id: "dragon1",
            prefixUrl: "./openseadragon/images/",
            tileSources: `PrivateHistory/Media/Families/${familyId}/${familyId}.dzi`,
            ajaxWithCredentials: true,
            loadTilesWithAjax: true,
            ajaxHeaders: {
                Authorization: loginInfo.jwt
            }
        });
    }, [familyId]);

    return (
        <div id="dragon1" style={{ width: '100%', height: '100%', backgroundColor: 'black' }}>
        </div>
    );
}