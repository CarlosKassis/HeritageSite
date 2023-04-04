import React, { useEffect } from 'react';
import OpenSeadragon from 'openseadragon';

export function DragonWrapper() {

    useEffect(() => {
        console.log('aaaa');
        OpenSeadragon({
            id: "a1",
            prefixUrl: "./openseadragon/images/",
            tileSources: "~/PrivateHistory/Families/Layous/Layous.dzi"
        });
    }, []);

    return (

        <div id="a1" style={{ width: '800px', height: '600px', backgroundColor: 'black' }}>
        </div>
    );
}