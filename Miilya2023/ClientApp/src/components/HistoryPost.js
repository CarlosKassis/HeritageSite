import React, { useEffect, useState } from 'react';
import LocalizedStrings from 'localized-strings'

export function HistoryPost(props) {

    const strings = new LocalizedStrings({
        ar: {
            title: 'موقع تراث معليا'
        },
        he: {
            title: 'אתר מורשת מעיליא'
        },
    });

    const imageRef = useRef(null);

    function getString(language, str) {
        strings.setLanguage(language);
        return strings[str];
    }

    useEffect(() => {
        if (props.imageName === null) {
            return;
        }

        console.log(props.imageName);
        imageRef.current = document.createElement("img");

        imageRef.current.src = "https://www.google.com/images/branding/googlelogo/2x/googlelogo_light_color_272x92dp.png";
        image.onload = () => {
            setShow(true);
        };

    }, [props.imageName])

    return (
        <div style={{ height: '90vh', width: '100%' }}>
            <h1>MyTitle:</h1>
            <Image source={{
                uri: `https://www.google.com/images/branding/googlelogo/2x/googlelogo_light_color_272x92dp.png`,
            }}/>
            <h3>MyImageDate</h3>
        </div>
    );
}