#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NanoFinanceTracker.WebApi/NanoFinanceTracker.WebApi.csproj", "NanoFinanceTracker.WebApi/"]
COPY ["NanoFinanceTracker.Core/NanoFinanceTracker.Core.csproj", "NanoFinanceTracker.Core/"]
RUN dotnet restore "./NanoFinanceTracker.WebApi/NanoFinanceTracker.WebApi.csproj"
COPY . .
WORKDIR "/src/NanoFinanceTracker.WebApi"
RUN dotnet build "./NanoFinanceTracker.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NanoFinanceTracker.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
ENV ASPNETCORE_URLS http://+:8080
ENV ASPNETCORE_ENVIRONMENT Production
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NanoFinanceTracker.WebApi.dll"]