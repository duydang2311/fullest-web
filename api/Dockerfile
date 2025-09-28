FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build

WORKDIR /app

COPY . .

RUN dotnet publish -c Release WebApp.Api -o published

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime

WORKDIR /app

COPY --from=build /app/published .

EXPOSE 8080 443

ENTRYPOINT ["dotnet", "WebApp.Api.dll"]
