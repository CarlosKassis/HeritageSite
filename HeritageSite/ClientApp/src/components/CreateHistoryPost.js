﻿import React, { useState, useRef } from "react";
import MyAPI from "../MyAPI";
import UploadImage from "./UploadImage";

export function CreateHistoryPost({ loginInfo }) {
    const [title, setTitle] = useState(null);
    const [image, setImage] = useState(null);
    const [description, setDescription] = useState(null);
    const [imageUploadError, setImageUploadError] = useState(null);

    const handleTitleChange = (e) => {
        setTitle(e.target.value);
    };

    const handleDescriptionChange = (e) => {
        setDescription(e.target.value);
        e.target.style.height = '40px'
        e.target.style.height = `${e.target.scrollHeight}px`;
    };

    function onSubmit(e) {
        e.preventDefault();

        if (image == null) {
            setImageUploadError("Please submit an image")
            return;
        }

        MyAPI.submitHistoryPost(title, description, image, loginInfo.jwt).then(response => {
            location.reload();
        }).catch(error => {
            setImageUploadError(error);
        })
    };

    function onUploadImage(image) {
        setImage(image)
    }

    return (
        <div className={"create-history-post padded main-column"} >
            <form onSubmit={onSubmit}>

                <h5 htmlFor="title">عنوان:</h5>
                <input className={"create-history-post-input"} autoComplete="off" style={{ width: '100%', padding: '6px' }} type="text" id="post-title" onChange={handleTitleChange} />

                <UploadImage onUploadImage={onUploadImage} />

                <h5 htmlFor="description" style={{ marginTop: '10px' }} >وصف:</h5>
                <textarea id="post-description" className={"create-history-post-input language-direction"} style={{ width: '100%', height: '40px', padding: '10px', overflow: 'hidden' }} onChange={handleDescriptionChange} />

                {imageUploadError && <p style={{ color: '#f55', fontSize: '14px', marginTop: '5px' }}>{imageUploadError}</p>}
                <button type="submit" style={{ marginTop: '10px', marginRight: 'auto'}}>أُنشر</button>
            </form>
        </div>
    );
}