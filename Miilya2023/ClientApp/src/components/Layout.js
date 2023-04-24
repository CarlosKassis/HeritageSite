import React, { useEffect } from 'react';
import { Container } from 'reactstrap';
import NavMenu from './NavMenu';

export function Layout(loginInfo, onClickLanguage, language, children)  {
    return (
        <div>
            <NavMenu loginInfo={loginInfo} onClickLanguage={onClickLanguage} language={language} />
            <Container>
                {children}
            </Container>
        </div>
    );
}
