# Build context: backend/Ibnelve.Api
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Ibnelve.Api.csproj .
RUN dotnet restore Ibnelve.Api.csproj

COPY . .
RUN dotnet publish Ibnelve.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

# Render (e hosts Docker parecidos) injeta a porta a escutar via $PORT; usa 8080 se nao vier.
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet Ibnelve.Api.dll"]
