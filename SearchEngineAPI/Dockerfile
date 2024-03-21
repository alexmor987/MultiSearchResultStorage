#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

RUN apt-get update && apt-get install -y iputils-ping && rm -rf /var/lib/apt/lists/*


RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SearchEngineAPI/SearchEngineAPI.csproj", "SearchEngineAPI/"]
RUN dotnet restore "./SearchEngineAPI/SearchEngineAPI.csproj"
COPY . .
WORKDIR "/src/SearchEngineAPI"
RUN dotnet build "./SearchEngineAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SearchEngineAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SearchEngineAPI.dll"]