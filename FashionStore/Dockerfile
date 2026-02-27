# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY FashionStore/FashionStore.csproj FashionStore/
RUN dotnet restore FashionStore/FashionStore.csproj

COPY . .
WORKDIR /src/FashionStore
RUN dotnet publish -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 10000
ENTRYPOINT ["dotnet", "FashionStore.dll"]