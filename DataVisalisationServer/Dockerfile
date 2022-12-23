FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DataVisalisationServer/DataVisalisationServer.csproj", "DataVisalisationServer/"]
RUN dotnet restore "DataVisalisationServer/DataVisalisationServer.csproj"
COPY . .
WORKDIR "/src/DataVisalisationServer"
RUN dotnet build "DataVisalisationServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DataVisalisationServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataVisalisationServer.dll"]
