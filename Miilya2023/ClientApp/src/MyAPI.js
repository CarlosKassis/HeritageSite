
export default class MyAPI {
    static async validateLoginJwt(jwt) {
        const response = await fetch('/UserAuthentication/Validate', {
             headers: {
                 'Authorization': jwt
             }
         });

         return response.ok;
    }

    static async getLoginJwtFromGoogleJwt(jwt) {
        const response = await fetch('/UserAuthentication/Google/Login', {
            method: "POST",
            headers: {
                'google-credentials': jwt
            }
        });

        if (response.ok) {
            return await response.text();
        }

        return null;
    }

    static async getLoginJwtFromMicrosoftJwt(jwt) {
        const response = await fetch('/UserAuthentication/Microsoft/Login', {
            method: "POST",
            headers: {
                'microsoft-credentials': jwt
            }
        });

        if (response.ok) {
            return await response.text();
        }

        return null;
    }

    static async getFamilies(jwt) {
        const response = await fetch('api/PrivateHistory/Family/All', {
            headers: {
                'Authorization': jwt
            }
        });

        if (response.ok) {
            return await response.json();
        }

        return null;
    }

    static async getHistoryImageLowRes(jwt, name) {
        const response = await fetch(`api/PrivateHistory/Image/LowRes/${name}`, {
            headers: {
                'Authorization': jwt
            }
        });

        if (response.ok) {
            return await response.blob();
        }

        return null;
    }

    static async getHistoryImage(jwt, name) {
        const response = await fetch(`api/PrivateHistory/Image/${name}`, {
            headers: {
                'Authorization': jwt
            }
        });

        if (response.ok) {
            return await response.blob();
        }

        return null;
    }

    static async getHistoryPosts(jwt, startingFromIndex) {

        // If start index is empty then don't pass it
        var url = `api/PrivateHistory/HistoryPost${(startingFromIndex !== null ? ('/' + startingFromIndex) : '')}`;

        const response = await fetch(url, {
            headers: {
                'Authorization': jwt
            }
        });

        if (response.ok) {
            return await response.json();
        }

        return null;
    }

    static async submitHistoryPost(title, description, image, jwt) {

        const formData = new FormData();

        if (title !== null && title !== '') {
            formData.append('title', title);
        }

        if (description !== null && description !== '') {
            formData.append('description', description);
        }

        if (image !== null) {
            formData.append('image', image);
        }

        const response = await fetch('api/PrivateHistory/HistoryPost/Submit', {
            method: 'POST',
            body: formData,
            headers: {
                'Authorization': jwt
            }
        });

        if (response.ok) {
            return "ok";
        }

        return null;
    }
}
