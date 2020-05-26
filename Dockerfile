FROM node:10.13.0-alpine as node
WORKDIR /app
COPY MedicamentsAggregator.Service/ClientApp/public ./MedicamentsAggregator.Service/ClientApp/public
COPY MedicamentsAggregator.Service/ClientApp/src ./MedicamentsAggregator.Service/ClientApp/src
COPY MedicamentsAggregator.Service/ClientApp/package*.json ./MedicamentsAggregator.Service/ClientApp/
WORKDIR /app/MedicamentsAggregator.Service/ClientApp
RUN npm install --progress=true --loglevel=silent
RUN npm run build

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY MedicamentsAggregator.Service/*.csproj ./MedicamentsAggregator.Service/
RUN dotnet restore

# copy everything else and build app
COPY MedicamentsAggregator.Service/. ./MedicamentsAggregator.Service/
WORKDIR /app/MedicamentsAggregator.Service
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /app/MedicamentsAggregator.Service/out ./
COPY --from=node /app/MedicamentsAggregator.Service/ClientApp/dist ./ClientApp/dist
ENTRYPOINT ["dotnet", "MedicamentsAggregator.Service.dll"]