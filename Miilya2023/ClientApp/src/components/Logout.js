import React, { useEffect } from 'react';
import { Redirect } from 'react-router';

export function Logout({ onLogOut }) {

    useEffect(() => {
        onLogOut();
    }, [])

    return (
        <Redirect to='/' />
    );
}