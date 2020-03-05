# WorkPointSamples

Daemon application to WorkPoint 365 WebAPI

The WorkPoint 365 WebAPI supports the OAuth 2.0 client credentials grant type. This allows you to call the WebAPI using the identity of an application instead of an user identity. This solution is suitable for batch jobs, long running processes and integration scenarios where you don’t want any users involved.
You start by creating an application that can authenticate with Azure AD using the application identity instead of the user identity and then call the WorkPoint 365 WebAPI with the indentity of the application. User interaction is not possible with this grant type. The application must have its own identity and use this identity when it authenticates with Azure AD and calls the WorkPoint 365 WebAPI.
Daemon application requests an access token by using its application identity and presenting its Application ID, credential (password or certificate), and application ID URI to Azure AD. After successful authentication, the daemon application receives an access token from Azure AD, which is then used to call the web API.
A Daemon application must require application permission to the WorkPoint 365 WebAPI before the WebAPI will accept requests from the deamon application. An application permission is granted to the daemon application by your organization's administrator
https://login.microsoftonline.com/{tenant}/adminconsent?client_id={clientId}&redirect_uri={redirectUri}
More information on the OAuth 2.0 client credentials flow in Azure AD:	 
https://docs.microsoft.com/da-dk/azure/active-directory/develop/active-directory-v2-protocols-oauth-client-creds
Before you get started, a tenant administrator have to sign up to the WorkPoint 365 WebAPI:
https://wp365webapi.azurewebsites.net
When you have signed up to the WebAPI you can go on and create a new daemon app in your Azure AD.
1.	Give your daemon application a name, select type Web app / API and enter a sign-on URL (this could be anything i.e. https://WorkPoint365WebAPIDaemon)
2.	When the application is created, Click Keys and create your application secret. This secret is used together with your application ID as the identity of your application. Copy the key and save it for later use.
3.	Go to required permissions and select the Application Permission “Access WorkPoint365 WebAPI”.
4.	Get a tenant administrator to consent your daemon application https://login.microsoftonline.com/{tenant}/adminconsent?client_id={clientId}&redirect_uri={redirectUri}
5.	Go on an build your daemon application using the sample code provided. Replace the following values in the app.config:
o	{Tenant}: The name of your tenant
o	{ClientId}: The client id (application id) of your daemon application.
o	{AppKey}: The application secret created in step 2.
o	{SitecollectionUrl}: The url of your WorkPoint 365 solution.
You are now ready to start calling the WorkPoint 365 WebAPI using an application identity.
