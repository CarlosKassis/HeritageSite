
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

    static async getHistoryPosts(jwt, startingFromIndex, searchText) {

        // If start index is empty then don't pass it
        var url = `api/PrivateHistory/HistoryPost${(startingFromIndex !== null ? ('/' + startingFromIndex) : '')}`;

        const formData = new FormData();

        if (searchText !== null) {
            formData.append('searchText', searchText);
        }

        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Authorization': jwt
            },
            body: formData
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

    static async bookmarkHistoryPost(jwt, historyPostIndex) {
        var url = `api/PrivateHistory/Bookmark/Add/${historyPostIndex}`;

        const response = await fetch(url, {
            headers: {
                'Authorization': jwt
            }
        });

        if (response.ok) {
            return "ok";
        }

        return null;
    }

    static async unbookmarkHistoryPost(jwt, historyPostIndex) {
        var url = `api/PrivateHistory/Bookmark/Remove/${historyPostIndex}`;

        const response = await fetch(url, {
            headers: {
                'Authorization': jwt
            }
        });

        if (response.ok) {
            return "ok";
        }

        return null;
    }

    static async deleteHistoryPost(jwt, historyPostIndex) {
        var url = `api/PrivateHistory/HistoryPost/Delete/${historyPostIndex}`;

        const response = await fetch(url, {
            method: "POST",
            headers: {
                'Authorization': jwt
            }
        });

        if (response.ok) {
            return "ok";
        }

        return null;
    }

    static async getFamilyTree(jwt, familyId) {
        var url = `api/PrivateHistory/Family/Tree/${familyId}`;

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
}
