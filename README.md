# TwitterCloneApiTwitter Clone API
This is a C# .NET Web API project for a Twitter clone application. It utilizes Neon DB for data persistence and Entity Framework for database management. This README file provides an overview of the project and instructions for setting it up and using it.

## Features
- User registration and authentication
- Create, read, update, and delete tweets
- Follow/unfollow other users
- Like and retweet tweets
- User profile management
- Search for tweets and users

## Prerequisites
Make sure you have the following prerequisites installed on your system:

- .NET Core SDK
- Neon DB
- Entity Framework
- Installation
- Clone this repository to your local machine or download the ZIP file and extract it.
- Open the project in your preferred Integrated Development Environment (IDE).

## Configuration
- Open the appsettings.json file.
- Modify the ConnectionStrings section to configure the database connection details.
- Make sure the Neon DB and Entity Framework are properly configured based on your database provider and connection string.

## Usage
- Build the project to ensure that all required dependencies are resolved.
- Run the application. This will start the Web API server on a specified port (default: 5000).
- Use your preferred API testing tool (such as Postman) to interact with the endpoints.
