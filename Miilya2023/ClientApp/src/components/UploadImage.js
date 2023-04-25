import React, { useState } from 'react';

export function UploadImage({ onUploadImage }) {
    const [image, setImage] = useState(null);

    const handleImageUpload = (e) => {
        e.preventDefault()
        console.log(e)
        var file = e.dataTransfer.files[0];
        const fileReader = new FileReader();
        fileReader.readAsDataURL(file);
        fileReader.onload = () => {
            setImage(fileReader.result);
            onUploadImage(file);
        };
        //setImage(URL.createObjectURL(e.dataTransfer.files[0]));
    };

    return (
        <div
            htmlFor="imageUpload"
            style={{
                border: '2px dashed grey',
                marginTop: '10px',
                padding: '20px',
                boxShadow: '2px 2px 5px rgba(0, 0, 0, 0.2)',
                width: '100%',
                minHeight: '100px',
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center'
            }}
            onDragOver={(event) => event.preventDefault()}
            onDrop={handleImageUpload}
        >
            {!image && (
                <>
                    <input
                        type="file"
                        id="imageUpload"
                        style={{ display: 'none' }}
                        onChange={handleImageUpload}
                        style={{ width: '100%', height: ' 100%', display: 'none' }}
                    />
                    <label >Upload Image</label>
                </>
            )}
            {image && (
                <>
                    <img
                        src={image}
                        alt="Uploaded file"
                        style={{ maxWidth: '100%', maxHeight: '100%' }}
                    />

                </>
            )}
        </div>
    );
};

export default UploadImage;
