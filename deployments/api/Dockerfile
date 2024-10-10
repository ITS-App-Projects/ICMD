# Build runtime image and run .net core
# https://learn.microsoft.com/en-us/dotnet/core/compatibility/containers/8.0/aspnet-port
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY $PWD/out/electrical-api ./

EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
ENTRYPOINT ["dotnet", "ICMD.API.dll"]
