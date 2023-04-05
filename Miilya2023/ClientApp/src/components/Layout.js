import React, { Component, useEffect } from 'react';
import { Container } from 'reactstrap';
import NavMenu from './NavMenu';

export function Layout(props)  {

    useEffect(() => {
        console.log(props.children);

        var counter = sessionStorage.getItem('counter');
        console.log('zzz: ' + counter);
        if (!counter) {
            counter = 0;
        }
        counter++;
        var counter = sessionStorage.setItem('counter', counter);

    }, [props.children]);

    return (
        <div>
            <NavMenu onClickLanguage={() => props.onClickLanguage()} language={props.language} />
            <Container>
                {props.children}
            </Container>
        </div>
    );
}
