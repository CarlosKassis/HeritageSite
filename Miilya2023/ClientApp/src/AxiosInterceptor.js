import React, { useEffect } from 'react';
import OpenSeadragon from 'openseadragon';
import axios from 'axios';

export function AxiosInterceptor() {

    const axiosRef = useRef(null);

    useEffect(() => {
        axiosRef.current = axios.create();
        axiosRef.current.interceptors.request.use((config) => {
            config.headers['Authorization'] = 'Bearer AAAAAAAAAAAAAAAAAAAAAAAAAA';
            return config;
        });
    }, []);

    return (
        <div id="dragon1" style={{ width: '100%', height: '100%', backgroundColor: 'black' }}>
        </div>
    );
}