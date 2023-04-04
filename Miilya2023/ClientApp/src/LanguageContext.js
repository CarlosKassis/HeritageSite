import React, { createContext, useState } from 'react';

export const LanguageContext = createContext();

export const LanguageProvider = ({ children }) => {
    const [language, setLanguage] = useState('es');

    const switchLanguage = (language) => {
        setLanguage(language);
    };

    return (
        <LanguageContext.Provider value={{ language, switchLanguage }}>
            {children}
        </LanguageContext.Provider>
    );
};