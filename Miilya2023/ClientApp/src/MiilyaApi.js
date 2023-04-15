
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
        const response = fetch('PrivateHistory/Family/All', {
            headers: {
                'Authorization': jwt
            }
        });

        if (response.ok) {
            return response.json();
        }

        return null;
    }
}
