# What it is?

This is the Evaucation API, I created this project as a template that use clean architecture, Redis and postgres.
This project also demonstrate how to use semaphore or lock as race condition prevention wityhin the same instance.

# Set up

- Please copy content from appsettings.json ConnectionStrings and environmentVariables to your `manage user secret`
- Set the db password id docker-compose.yml first if you are testing on your local machine.
- manage user secret PostgresConnection db password should be the same as in docker-compose

# How to run

After you have set up all docker container with docker compose, then run

```
//Enable Postgres and redis
docker compose up -d

// Restore nuget package
dotnet restore

// Build the app
dotnet build -c Release

// Run
dotnet run
```

If you are using Visual Studio code like i do

Then you need `C# Dev Kit` extention installed. and F5 on the project it should prompt you where to run, which you should select `C#` then `evacPlanMoni.csproj`

# DB Migration command

Update database with migration files

```
dotnet ef database update
```

Create new migration

```
dotnet ef migrations add {name}
```

# Dotnet Build & publish

```
// Restore nuget package
dotnet restore

// Build the app
dotnet build -c Release

// publish the app
dotnet publish -c Release -o ./publish
```

# API

Please use http file to test.

### Add an Evacuation Zone

```
POST {{HostAddress}}/api/evacuation-zones
Content-Type: application/json

{
  "zoneId": "Z1",
  "locationCoordinates": {
    "latitude": 13.7563,
    "longitude": 100.5018
  },
  "numberOfPeople": 100,
  "urgencyLevel": 5
}
```

### Add a bus

```

POST {{HostAddress}}/api/vehicles
Content-Type: application/json

{
  "vehicleId": "V1",
  "capacity": 40,
  "type": "Bus",
  "LocationCoordinates": {
    "latitude": 13.7650,
    "longitude": 100.5381
  },
  "speed": 60
}

```

### Generate Evacuation Plan

```
POST {{HostAddress}}/api/Evacuations/plan
Accept: application/json
```

### Get Evacuation Status

```
GET {{HostAddress}}/api/Evacuations/status
Accept: application/json
```

### Update Status

```
PUT {{HostAddress}}/api/Evacuations/update
Content-Type: application/json

{
  "ZoneId": "Z1",
  "EvacuatedCount": 20,
  "VehicleId": "V2"
}
```

### Clear Data

```
DELETE {{HostAddress}}/api/Evacuations/clear
Accept: application/json
```

---
