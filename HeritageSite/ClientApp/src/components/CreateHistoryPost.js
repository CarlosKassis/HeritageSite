import React, { useState, useRef } from "react";
import MyAPI from "../MyAPI";
import UploadImage from "./UploadImage";

export function CreateHistoryPost({ loginInfo }) {
    const [title, setTitle] = useState(null);
    const [image, setImage] = useState(null);
    const [description, setDescription] = useState(null);
    const [imageUploadError, setImageUploadError] = useState(null);
    const clickedDescriptionOnce = useRef(false);

    const handleTitleChange = (e) => {
        setTitle(e.target.value);
    };

    const handleDescriptionChange = (e) => {
        setDescription(e.target.value);
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

    function onClickDescription() {
        if (clickedDescriptionOnce.current) {
            return;
        }

        clickedDescriptionOnce.current = true;
        document.getElementById('post-description').style.height = '150px';
    }

    function onUploadImage(image) {
        setImage(image)
    }

    return (
        <div className={"create-history-post padded main-column"} >
            <form onSubmit={onSubmit}>

                <h5 htmlFor="title">عنوان:</h5>
                <input className={"create-history-post-input"} autoComplete="off" style={{ width: '100%' }} type="text" id="post-title" onChange={handleTitleChange} />

                <UploadImage onUploadImage={onUploadImage}/>
                <h5 htmlFor="description" style={{ marginTop: '10px' }} >وصف:</h5>
                <textarea id="post-description" className={"create-history-post-input"} onClick={onClickDescription} style={{ width: '100%', height: '40px' }} onChange={handleDescriptionChange} />
                { imageUploadError && <p style={{ color: '#f55', fontSize: '14px', marginTop: '5px' }}>{imageUploadError}</p> }
                <button type="submit"
                    style={{ marginTop: '10px', marginRight: 'auto', border: 'none', borderRadius: '2px', boxShadow: '1px 1px 4px rgba(0, 0, 0, 0.8)' }}>Submit</button>
            </form>
        </div>
    );
}