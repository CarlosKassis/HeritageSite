import React, { useEffect } from 'react';
import { Container } from 'reactstrap';
import NavMenu from './NavMenu';

export function Layout(props)  {
    return (
        <div>
            <NavMenu loginInfo={props.loginInfo} onClickLanguage={props.onClickLanguage} language={props.language} />
            <Container>
                {props.children}
            </Container>
        </div>
    );
}
