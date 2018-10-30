# .Net Core version of the "Node JS: Advanced Concepts" course

> source code for the .Net Core version of the "Node JS: Advanced Concepts" Udemy course

## Build Setup

## install server dependencies

$ dotnet restore (They will be installed automatically when running anyway)

## install clients dependencies

$ cd NetCorereactReduxAdvanced\wwwroot

$ npm install # Or yarn install

## execute it

$ dotnet run (or F5 from Visual Studio Code)

*   it is loaded at both http://localhost:5000 and https://localhost:50001
*   it executes both server and client at the same time 

## Google Client ID configuration

* It is possible that you need to add https://localhost:5001 into "Authorised JavaScript origins".
* It is possible that you need to add http://localhost:5000/Logins/ExternalLoginCallBack or https://localhost:5001/Logins/ExternalLoginCallBack into "Authorised redirect URIs".

## Before executing it, setting credentials and variables must be updated

1st) Rename appsettings.example.json to appsettings.json

2nd) Update your own values

## In order to use Redis it must be installed and started

- Install from https://github.com/MicrosoftArchive/redis/releases (Windows Version)
- Execute redis-server to start it

## In order to run the automated testing

1st) Publish the solution with
- dotnet run

2st) Execute the tests with
- dotnet test NetCoreReactReduxAdvanced.IntegrationTest

## In order to setup Travis CI and Appveyor
1- Put the Environment Variables from the appsettings.json on the Settings on the Web Site for the project.

2- The Environment Variables that must be included are:

* AllowedHosts = *
* ASPNETCORE_ENVIRONMENT = Testing
* Google__ClientId = "Your Google Client Id"
* Google__ClientSecret = = "Your Google Client Secret"
* MongoDbSettings__ConnectionString = mongodb://127.0.0.1:27017/blog_ci
* MongoDbSettings__DatabaseName = blog_ci
* RedisSettings__Expiration = 60
* RedisSettings__Host = redis://127.0.0.1:6379


## Within the code you can see how to
- Use MongoDb with .Net Core
- Use Identity with MongoDb 
- Use Redis to cache Api responses
- Return unauthorized code with specific response instead of redirect 302 for .NET Core Api
- Use TestServer to run Integration tests
- Use Pupperteer Sharp to run UI tests
- Autorun Tests with Pupperteer using WebHostBuilder
- Create a fake protected .Net Core cookie to test authenticated routes
- Test .NET Core Api requests using Pupperteer Sharp
- Use C# Anonymous Methods to simplify the execution of API request tests
- Use Travis CI for Continuous Integration
- Use Appveyor for Continuous Integration
- Use Amazon S3 to store images with Presigned URL

## Travis CI issue
- Please notice that currently tests are not executed on Travis CI Linux properly although they do on Travis CI OSX. 
They keep on running for 10 minutes with the following meessage:

Test run for 

/home/travis/build/peelmicro/dot-net-core-react-redux-advanced/NetCoreReactReduxAdvanced.IntegrationTest/bin/Debug/netcoreapp2.1/NetCoreReactReduxAdvanced.IntegrationTest.dll(.NETCoreApp,Version=v2.1)
Microsoft (R) Test Execution Command Line Tool Version 15.8.0
Copyright (c) Microsoft Corporation.  All rights reserved.
Starting test execution, please wait...

After 10 minutes they fail with the following message:

The command "travis_wait dotnet test NetCoreReactReduxAdvanced.IntegrationTest" exited with 137.
Done. Your build exited with 1.

- It has been detected that it hungs on when trying to open the browser with:

```
Browser = await Puppeteer.LaunchAsync(new LaunchOptions{Headless = true});
```

It seems as if the ```WebHostBuilder``` method is not working properly with Linux.

## In order to get to know what has been developed follow the course on

https://www.udemy.com/advanced-node-for-developers
