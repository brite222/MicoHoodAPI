FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MicoHoodAPI/MicoHood.API.csproj", "MicoHoodAPI/"]
RUN dotnet restore "MicoHoodAPI/MicoHood.API.csproj"
COPY . .
WORKDIR /src/MicoHoodAPI
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "MicoHood.API.dll"]