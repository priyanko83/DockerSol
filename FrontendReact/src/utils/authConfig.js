import { UserAgentApplication } from "msal";

// For a full list of msal.js configuration parameters, 
// visit https://azuread.github.io/microsoft-authentication-library-for-js/docs/msal/modules/_authenticationparameters_.html
export const msalApp = new UserAgentApplication({
    auth: {
        clientId: "5e20e54b-7c50-49ed-992b-d6151e3104ab",
        authority: "https://login.microsoftonline.com/38857842-8570-4fcf-870b-b7aa1fcddf06",
        validateAuthority: true,
        redirectUri: "https://pm-aks-microservices.centralindia.cloudapp.azure.com",
        //redirectUri: "http://localhost:4200",
        navigateToLoginRequestUrl: false
    },
    cache: {
        cacheLocation: "localStorage", // This configures where your cache will be stored
        storeAuthStateInCookie: false // Set this to "true" if you are having issues on IE11 or Edge
    },
});

// Coordinates and required scopes for your web api
export const apiConfig = {
    baseUri: "https://pm-aks-microservices.centralindia.cloudapp.azure.com/gatewayapi",
    //baseUri:  "http://localhost:50918",
    resourceUri: "https://pm-aks-microservices.centralindia.cloudapp.azure.com/gatewayapi/api/values",
    resourceScope: "api://50e6b9f0-b53e-42c7-89ab-bcbf35c47147/consent"
}

/** 
 * Scopes you enter here will be consented once you authenticate. For a full list of available authentication parameters, 
 * visit https://azuread.github.io/microsoft-authentication-library-for-js/docs/msal/modules/_authenticationparameters_.html
 */
export const loginRequest = {
    scopes: ["openid", "profile"]
}

// Add here scopes for access token to be used at the API endpoints.
export const tokenRequest = {
    scopes: [apiConfig.resourceScope, "offline_access"]
}
