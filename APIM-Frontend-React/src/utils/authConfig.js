// For a full list of msal.js configuration parameters, 
// visit https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-browser/docs/configuration.md
export const msalConfig = {
    auth: {
        clientId: "44fd3681-bdba-4543-9a9d-c292babbddd3",
        authority: "https://login.microsoftonline.com/38857842-8570-4fcf-870b-b7aa1fcddf06",
        //redirectUri: "http://localhost:3000"
        redirectUri: "https://apimfrontendwebsite.z13.web.core.windows.net"
    },
    cache: {
        cacheLocation: "localStorage", // This configures where your cache will be stored
        storeAuthStateInCookie: false // Set this to "true" if you are having issues on IE11 or Edge
    },
}

// Coordinates and required scopes for your web api
export const apiConfig = {
    resourceUri: "https://pm-apim-cp.azure-api.net/p1/api/Product01",
    //resourceUri: "https://localhost:44351/api/profile",
    resourceScope: "api://b953bd49-7f46-4272-af93-68ef1ea619aa/.default"
}

/** 
 * Scopes you enter here will be consented once you authenticate. For a full list of available authentication parameters, 
 * visit https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-browser/docs/configuration.md
 */
export const loginRequest = {
    scopes: ["openid", "profile", "offline_access"]
}

// Add here scopes for access token to be used at the API endpoints.
export const tokenRequest = {
    scopes: [apiConfig.resourceScope]
}

// Add here scopes for silent token request
export const silentRequest = {
    scopes: ["openid", "profile", apiConfig.resourceScope]
}
