
export default class MyAPI {
    static async validateLoginJwt(jwt) {
        const response = await fetch('/UserAuthentication/Validate', {
             headers: {
                 'Authorization': jwt
             }
         });

         return response.ok;
    }

    static async getFamilies(jwt) {
        const response = await fetch('PrivateHistory/Family/All', {
            headers: {
                'Authorization': jwt
            }
        });

        if (response.ok) {
            return await response.json();
        }

        return null;
    }

    static async getHistoryImage(jwt, name) {
        const response = await fetch(`PrivateHistory/Media/Images/${name}`, {
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
        var url = startingFromIndex !== null ? `PrivateHistory/HistoryPost/${startingFromIndex}` : 'PrivateHistory/HistoryPost';

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
}
