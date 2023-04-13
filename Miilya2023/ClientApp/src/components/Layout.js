import React, { useEffect } from 'react';
import { Container } from 'reactstrap';
import NavMenu from './NavMenu';

export function Layout(props)  {

    useEffect(() => {
        var counter = sessionStorage.getItem('counter');

        if (!counter) {
            counter = 0;
        }
        counter++;
        var counter = sessionStorage.setItem('counter', counter);

    }, [props.children]);

    return (
        <div>
            <NavMenu onClickLanguage={props.onClickLanguage} language={props.language} />
            <Container>
                {props.children}
            </Container>
        </div>
    );
}
