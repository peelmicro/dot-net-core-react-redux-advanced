# .Net Core version of the "Node JS: Advanced Concepts" course

> source code for the .Net Core version of the "Node JS: Advanced Concepts" Udemy course

## Build Setup

# install server dependencies

$ dotnet restore (They will be installed automatically when running anyway)

# install clients dependencies

$ cd NetCorereactReduxAdvanced\wwwroot

$ npm install # Or yarn install

# execute it

$ dotnet run (or F5 from Visual Studio Code)

*   it is loaded at both http://localhost:5000 and https://localhost:50001
*   it executes both server and client at the same time

# Google Client ID configuration

* It is possible that you need to add https://localhost:5001 into "Authorised JavaScript origins".
* It is possible that you need to add http://localhost:5000/Logins/ExternalLoginCallBack or https://localhost:5001/Logins/ExternalLoginCallBack into "Authorised redirect URIs".

# Before executing it, setting credentials and variables must be updated

1st) Rename appsettings.example.json to appsettings.json

2nd) Update your own values

# In order to use Redis it must be installed and started

- Install from https://github.com/MicrosoftArchive/redis/releases (Windows Version)
- Execute redis-server to start it

# In order to run the automated testing

1st) Run the main program in one terminal
- Open a new Terminal
- Change to NetCorereactReduxAdvanced folder
- Execute: dotnet run

2st) Execute the tests in another terminal
- Open a new Terminal
- Change to NetCorereactReduxAdvanced.IntegrationTest folder
- Execute: dotnet test

# In order to setup Travis CI
- Put the settings from the appsettings.json in the Repository Settings on the Web Site

# Within the code you can see how to
- Use MongoDb with .Net Core
- Use Identity with MongoDb 
- Use Redis to cache Api responses
- Return unauthorized code with specific response instead of redirect 302 for .NET Core Api
- Use TestServer to run Integration tests
- Use Pupperteer Sharp to run UI tests
- Create a fake protected .Net Core cookie to test authenticated routes
- Test .NET Core Api requests using Pupperteer Sharp
- Use of C# Anonymous Methods to simplify the execution of API request tests
- Use of Travis CI for Continuous Integration

# Follow the course

Follow the course on https://www.udemy.com/advanced-node-for-developers
