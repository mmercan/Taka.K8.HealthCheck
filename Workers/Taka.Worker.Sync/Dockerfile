FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
ENV ASPNETCORE_URLS=http://+:80
WORKDIR /app
EXPOSE 80
EXPOSE 81

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS develop
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_USE_POLLING_FILE_WATCHER=1
ENV ASPNETCORE_ENVIRONMENT=Development
WORKDIR /src/Workers/Taka.Worker.Sync
EXPOSE 80
EXPOSE 81

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src/
COPY ["Workers/Taka.Worker.Sync/Taka.Worker.Sync.csproj", "Workers/Taka.Worker.Sync/"]
RUN dotnet restore "Workers/Taka.Worker.Sync/Taka.Worker.Sync.csproj"
COPY . .
WORKDIR "/src/Workers/Taka.Worker.Sync"
RUN dotnet build "Taka.Worker.Sync.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Taka.Worker.Sync.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Taka.Worker.Sync.dll"]
