# .Net Core version of the Node With React Fullstack Web Development Udemy course

> source code for the .Net Core version of the "Node JS: Advanced Concepts" Udemy course

## Build Setup

# install server dependencies

$ dotnet restore (They will be installed automatically when running anyway)

# install clients dependencies

$ cd client

$ npm install # Or yarn install

# execute it

$ dotnet run (or F5 from Visual Studio Code)

*   server is loaded at both http://localhost:5000 and https://localhost:50001
*   client is loaded at http://localhost:3000 (configured in appsettings.Development.json, it's totally optional. It will used http://localhost:53480/ it is not defined)
*   it executes both server and client at the same time

# Google Client ID configuration

* It is possible that you need to add https://localhost:5001 into "Authorised JavaScript origins".
* It is possible that you need to add http://localhost:5000/Logins/ExternalLoginCallBack or https://localhost:5001/Logins/ExternalLoginCallBack into "Authorised redirect URIs".

# Before executing it, setting credentials and variables must be updated

1st) Rename appsettings.example.json to appsettings.json

2nd) Update your own values

# It it possible 

# Follow the course

Follow the course on https://www.udemy.com/advanced-node-for-developers
