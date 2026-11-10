FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
WORKDIR /app
COPY global.json ./
COPY HosteraBackend.API/*.csproj HosteraBackend.API/
RUN dotnet restore ./HosteraBackend.API
COPY . .
RUN dotnet publish ./HosteraBackend.API -c Release -o out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=builder /app/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "HosteraBackend.API.dll"]
