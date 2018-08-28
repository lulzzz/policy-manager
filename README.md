# Policy Manager

A policy manager using Azure Functions and Cosmos DB to manage contextual based resources using policies.

## Using the service without a client

Use something like [Insomnia](https://insomnia.rest/download/)

Create a new application under the [dev center](https://apps.dev.microsoft.com/)

Create a new Folder

![New Folder](docs/folder.png)

For Get Code, configure it like the screens below:

![Address](docs/get-code-address.png)

> Set the address bar to an address that has the tenant id like https://login.microsoftonline.com/[tenant-id]/oauth2/v2.0/authorize

Then under the Query tab

![Query](docs/get-code-query.png)

> In this example, we have the api://{guid}/access_as_user setup as the scope. This is a custom scope built for my app id.

Then we take the address under "URL PREVIEW" and copy and paste it into a browser, then login and consent to app consent.

For the Get Token, configure it like the screens below:

![Address](docs/get-token-address.png)

Then under the "Body" tab select "Form URL Encoded" and configure it like the below screenshot:

![Form](docs/get-token-form.png)

> The code is from the url redirect from the get code screens in the browser.

> The id, secret and redirect url are from the application registration.

Once you click "send" it should give you json output that has a bearer token in it to use to call the api.