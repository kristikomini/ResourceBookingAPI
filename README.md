#ResourceBookingAPI
Overview
ResourceBookingAPI is a web application designed to manage bookings for various resources. It includes features such as user authentication, resource management, booking management, and more.

Project Structure
Controllers: Contains all the API controllers for managing users, resources, bookings, and authentication.
AuthController.cs
BookingController.cs
ResourceController.cs
ResourceTypeController.cs
UserController.cs
Helpers: Contains helper classes and utilities.
MappingProfiles.cs

Getting Started
Prerequisites
.NET SDK 8.0
SQL Server

Installation
Clone the repository:

git clone https://github.com/kristi2002/ResourceBookingAPI.git
cd ResourceBookingAPI
Restore dependencies:

dotnet restore
Build the project:

dotnet build
Update the connection string in appsettings.json to point to your SQL Server instance.

Updating the Connection String
To update the connection string, follow these steps:

Open appsettings.json located in the ResourceBooking project directory.

Find the ConnectionStrings section and update the DefaultConnection value with your SQL Server details. It should look something like this:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your_server_name;Database=your_database_name;User Id=your_username;Password=your_password;"
  }
}

Running the Application
To run the application locally, use the following command:

dotnet run --project ResourceBooking/ResourceBooking.csproj
This will start the application, and you can access it via http://localhost:5000.

API Documentation
The API documentation is available via Swagger. Once the application is running, navigate to http://localhost:5000/swagger to view and test the API endpoints.

Contributing
Fork the repository.
Create a feature branch (git checkout -b feature-branch).
Commit your changes (git commit -am 'Add new feature').
Push to the branch (git push origin feature-branch).
Create a new Pull Request.

Team & Contact Info
If you have any questions or feedback, feel free to reach out to any of the team members:

Kristi Komini: kristi.komini@studenti.unicam.it

