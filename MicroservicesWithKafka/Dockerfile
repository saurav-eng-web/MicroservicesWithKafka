# Stage 1: Build and publish the .NET project

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy only the main .csproj and restore dependencies

COPY MicroservicesWithKafka.csproj ./

RUN dotnet restore

# Copy all files

COPY . ./

# Publish only the project (not solution)

RUN dotnet publish MicroservicesWithKafka.csproj -c Release -o /app/out

# Stage 2: Runtime image

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "MicroservicesWithKafka.dll"]
 