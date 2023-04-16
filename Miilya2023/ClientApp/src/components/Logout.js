import React, { useEffect } from 'react';
import { Redirect } from 'react-router';

export function Logout(props) {

    useEffect(() => {
        props.onLogOut();
    }, [])

    return (
        <Redirect to='/' />
    );
}