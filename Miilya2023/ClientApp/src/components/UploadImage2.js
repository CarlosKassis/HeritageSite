import React, { useState, useRef } from 'react';

const UploadImage2 = () => {
    const fileInputRef = useRef(null);

    function handleFileUpload(file) {
        console.log(file);
    }

    function onDropFile(e) {
        e.preventDefault();
        handleFileUpload(e.dataTransfer.files[0]);
    };

    function onClickEnterFile(e) {
        handleFileUpload(e.target.files[0]);
    };

    function handleDragOver(e) {
        e.preventDefault();
    };

    return (
        <div
            id={"upload-image-container" }
            style={{
                marginTop: '20px',
                boxShadow: 'inset 1px 1px 6px rgba(0, 0, 0, 0.5)',
                width: '100%',
                height: '200px',
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center'
                } }
            onDragOver={handleDragOver}
            onDrop={onDropFile}
            onClick={() => fileInputRef.current.click()}
        >
            <input
                type="file"
                multiple={false}
                style={{ display: 'none' }}
                ref={fileInputRef}
                onChange={onClickEnterFile}
            />
            <img src={"/file-upload-outline.svg"} alt={"Upload Image"}
                style={{ width: '80px', height: '80px', margin: 'auto' }} />
        </div>
        )
};

export default UploadImage2;
