import React, { useState, useRef } from "react";
import MyAPI from "../MyAPI";

export function CreateHistoryPost({ loginInfo }) {
    const [title, setTitle] = useState(null);
    const [image, setImage] = useState(null);
    const [description, setDescription] = useState(null);
    const clickedDescriptionOnce = useRef(false);

    const handleTitleChange = (e) => {
        setTitle(e.target.value);
    };

    const handleDescriptionChange = (e) => {
        setDescription(e.target.value);
    };

    const onDropImage = (e) => {
        e.preventDefault();
        setImage(e.dataTransfer.files[0]);
    };

    function onSubmit(e) {
        e.preventDefault();

        MyAPI.submitHistoryPost(title, description, image, loginInfo.jwt).then(response => {
            console.log('wow')
            setTitle(null);
            setDescription(null);
            setImage(null);
        }).catch(error => {
            console.log(error);
        })
    };

    function onClickDescription() {
        if (clickedDescriptionOnce.current) {
            return;
        }

        clickedDescriptionOnce.current = true;
        document.getElementById('post-description').style.height = '200px';
    }

    return (
        <div className={"create-history-post"} >
            <form onSubmit={onSubmit}>

                <h4 htmlFor="title">عنوان:</h4>
                <input className={"create-history-post-input"} autoComplete="off" style={{ width: '100%' }} type="text" id="post-title" onChange={handleTitleChange} />

                <div
                    id="post-image"
                    onDrop={onDropImage}
                    onDragOver={(e) => e.preventDefault()}
                    style={{ border: "1px dashed black", height: "100px", marginTop: '20px' }} />

                <h4 htmlFor="description" style={{ marginTop: '20px' }} >وصف:</h4>
                <textarea id="post-description" className={"create-history-post-input"} onClick={onClickDescription} style={{ width: '100%' }} onChange={handleDescriptionChange} />

                <button type="submit"
                    style={{ marginTop: '20px', marginRight: 'auto', border: 'none', borderRadius: '2px', boxShadow: '1px 1px 4px rgba(0, 0, 0, 0.8)' }}>Submit</button>
            </form>
        </div>
    );
}