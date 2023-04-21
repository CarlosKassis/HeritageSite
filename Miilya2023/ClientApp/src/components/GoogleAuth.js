﻿import { useEffect, useRef } from 'react'
import MyAPI from '../MyAPI'

const loadScript = (src) =>
    new Promise((resolve, reject) => {
        if (document.querySelector(`script[src="${src}"]`)) return resolve()
        const script = document.createElement('script')
        script.src = src
        script.onload = () => resolve()
        script.onerror = (err) => reject(err)
        document.body.appendChild(script)
    })

const GoogleAuth = (props) => {

    const googleButton = useRef(null);

    useEffect(() => {
        const src = 'https://accounts.google.com/gsi/client'
        const id = "774040641386-74otf6r69gv7nd92efvbh5kf5l6j8jf8.apps.googleusercontent.com"

        loadScript(src)
            .then(() => {
                google.accounts.id.initialize({
                    client_id: id,
                    callback: handleCredentialResponse,
                })
                google.accounts.id.renderButton(
                    googleButton.current,
                    { theme: 'outline', size: 'large' }
                )
            })
            .catch(console.error)

        return () => {
            const scriptTag = document.querySelector(`script[src="${src}"]`)
            if (scriptTag) document.body.removeChild(scriptTag)
        }
    }, [])

    function handleCredentialResponse(response) {
        MyAPI.getLoginJwtFromGoogleJwt(response.credential).then(loginJwt => {
            props.onLogin(loginJwt);
        })
    }

    return (
        <div ref={googleButton}></div>
    )
}

export default GoogleAuth