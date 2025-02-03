# Use official .NET 8 runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use .NET 8 SDK to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CommunityKnowledgeSharingPlatform.csproj", "./"]
RUN dotnet restore "CommunityKnowledgeSharingPlatform.csproj"
COPY . .
RUN dotnet publish "CommunityKnowledgeSharingPlatform.csproj" -c Release -o /app/publish

# Final stage to run the app
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CommunityKnowledgeSharingPlatform.dll"]

