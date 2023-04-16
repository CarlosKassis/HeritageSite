import { parseJSON } from "openseadragon";

export default class MiilyaApi {
     static async validateLoginJwt(jwt) {
         const response = await fetch('/UserAuthentication/Validate', {
             headers: {
                 'Authorization': jwt
             }
         });

         return response.ok;
    }

    static async getFamilies(jwt) {
        let response = await fetch('PrivateHistory/Family/All', {
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
}
