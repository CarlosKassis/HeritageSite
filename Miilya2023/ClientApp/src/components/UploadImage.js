import React, { useState, useRef } from 'react';

export function UploadImage({ onUploadImage }) {

    const [uploadedImage, setUploadedImage] = useState(null);
    const fileInputRef = useRef(null);

    function handleFileUpload(file) {
        const fileReader = new FileReader();
        fileReader.readAsDataURL(file);
        fileReader.onload = () => {
            setUploadedImage(fileReader.result);
            onUploadImage(file);
        };
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
                padding: '20px',
                marginTop: '10px',
                boxShadow: 'inset 1px 1px 6px rgba(0, 0, 0, 0.5)',
                width: '100%',
                minHeight: '80px',
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
            {
                !uploadedImage &&
                <img src={"/file-upload-outline.svg"} alt={"Upload Image"}
                    style={{ width: '60px', height: '60px', margin: 'auto' }} />
            }
            {
                uploadedImage &&
                <>
                    <img
                        src={uploadedImage}
                        alt="Uploaded Image"
                        style={{
                            maxWidth: '100%',
                            maxHeight: '100%',
                            boxShadow: '1px 1px 6px rgba(0, 0, 0, 0.5)',
                        }}
                    />
                </>
            }
        </div>
        )
};

export default UploadImage;