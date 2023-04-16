import { parseJSON } from "openseadragon";

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

    static async getHistoryPosts(jwt) {
        const response = await fetch('PrivateHistory/HistoryPost', {
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
