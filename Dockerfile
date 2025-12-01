# build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# kopiujemy plik solution i csproj
COPY *.sln ./
COPY Backend_AIHost/*.csproj Backend_AIHost/

# przywracamy paczki
RUN dotnet restore

# kopiujemy resztę kodu
COPY . .

# budujemy aplikację
WORKDIR /src/Backend_AIHost
RUN dotnet build -c Release -o /app/build

# publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "Backend_AIHost.dll"]
