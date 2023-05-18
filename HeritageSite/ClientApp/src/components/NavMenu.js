import React, { useState } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import LocalizedStrings from 'localized-strings'

export default function NavMenu({ loginInfo, onClickLanguage, language }) {

    const localizedSiteName = {
        he: "אתר מורשת",
        ar: "موقع تراث"
    };

    const strings = new LocalizedStrings({
        he: {
            home: 'בית',
            families: 'משפחות',
            login: 'כניסה',
            logout: 'יציאה'
        },
        ar: {
            home: 'المنزل',
            families: 'عائلات',
            login: 'الدخول',
            logout: 'الخروج',
        }
    });

    function getString(language, str) {
        strings.setLanguage(language);
        return strings[str];
    }

    const [state, setState] = useState({ collaped: true });

    function toggleNavbar() {
        setState({ collapsed: !state.collapsed });
    }

    return (
       <header>
            <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3 middle-east" light>
                <Container>
                    <NavbarBrand tag={Link} to="/">{localizedSiteName[language]}</NavbarBrand>
                    <NavLink href="#" className="text-dark" onClick={onClickLanguage}>{language == "ar" ? 'עברית' : 'عربية'}</NavLink>
                    <NavbarToggler onClick={toggleNavbar} className="mr-2" />
                    <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={state.collapsed} navbar>
                        {loginInfo.loggedIn &&
                            <ul className="navbar-nav flex-grow">
                                <NavItem>
                                    <NavLink tag={Link} className="text-dark" to="/">{getString(language, 'home')}</NavLink>
                                </NavItem>
                                <NavItem>
                                    <NavLink tag={Link} className="text-dark" to="/PrivateHistory/Families">{getString(language, 'families')}</NavLink>
                                </NavItem>
                                <NavItem>
                                    <NavLink tag={Link} className="text-dark" to="/Logout">{getString(language, 'logout')}</NavLink>
                                </NavItem>
                            </ul>}
                        {!loginInfo.loggedIn &&
                            <ul className="navbar-nav flex-grow">
                                <NavItem>
                                    <NavLink tag={Link} className="text-dark" to="/">{getString(language, 'login')}</NavLink>
                                </NavItem>
                            </ul>}
                    </Collapse>
                </Container>
            </Navbar>
        </header>
    );
}
