# Build docker locally to check if its working during debug

## If image has not been build to ACR
- Execute below command on terminal
```typescript
// Go to Dockerfile Path
cd itsquarehub.services/ITSquarehub.Auth.Api

// Update the environment variables in the dockerfile using the following
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_ENVIRONMENT='development' \
    DB_CONNECTION_STRING='<redacted>'

// Build the solution
dotnet publish . -c Release -o ./out

// Go to docker path and run
docker build -t auth-api .
docker run -it --rm -p 8080:8080 auth-api
```

# Install Database
- Run the following command under /icmd-main/Backend to point both project and startup project
```c#
  // Run under /Backend only
  dotnet ef database update --verbose --project ICMD.EntityFrameworkCore --startup-project ICMD.API

  // If you encounter issue on
  CREATE ROLE postgres
  GRANT ALL ON SCHEMA public TO postgres
  GRANT USAGE, CREATE ON SCHEMA public TO postgres

  // Change needed is to remove the assignment of OWNER to alter or create in the code
  // Find them and remove them to recreate the migration.
  // Remove the whole ALTER with owner to.

  // Change the appsettings.Development.json and the orig 1 with this
  Server=<host>;Port=5432;Database=electrical-icmd;User Id=<name>;Password=<password>;Pooling=true;Timeout=300;CommandTimeout=300;
```
