import React, { useState } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import LocalizedStrings from 'localized-strings'

export default function NavMenu(props) {

    const localizedMiilya = {
        ar: "معليا",
        he: "מעיליא"
    };

    const strings = new LocalizedStrings({
        ar: {
            home: 'المنزل',
            families: 'عائلات',
        },
        he: {
            home: 'בית',
            families: 'משפחות'
        },
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
                    <NavbarBrand tag={Link} to="/">{localizedMiilya[props.language]}</NavbarBrand>
                    <NavLink href="#" className="text-dark" onClick={props.onClickLanguage}>{props.language == "ar" ? 'עברית' : 'عربية'}</NavLink>
                    <NavbarToggler onClick={toggleNavbar} className="mr-2" />
                    <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={state.collapsed} navbar>
                        <ul className="navbar-nav flex-grow">
                            <NavItem>
                                <NavLink tag={Link} className="text-dark" to="/">{getString(props.language, 'home')}</NavLink>
                            </NavItem>
                            <NavItem>
                                <NavLink tag={Link} className="text-dark" to="/PrivateHistory/Families">{getString(props.language, 'families')}</NavLink>
                            </NavItem>
                        </ul>
                    </Collapse>
                </Container>
            </Navbar>
        </header>
    );
}
