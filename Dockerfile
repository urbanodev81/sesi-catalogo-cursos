# Imagem única: o build do front vai para o wwwroot da API, que serve os dois.
# Usada no compose local e na publicação (Render, deploy por Dockerfile).

FROM node:24-alpine AS front
WORKDIR /app/frontend
COPY frontend/package.json frontend/package-lock.json ./
RUN npm ci --no-audit --no-fund
COPY frontend/ ./
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS api
WORKDIR /app
COPY CatalogoCursos.slnx ./
COPY src/CatalogoCursos.Api/CatalogoCursos.Api.csproj src/CatalogoCursos.Api/
RUN dotnet restore src/CatalogoCursos.Api/CatalogoCursos.Api.csproj
COPY src/ src/
COPY --from=front /app/src/CatalogoCursos.Api/wwwroot src/CatalogoCursos.Api/wwwroot
RUN dotnet publish src/CatalogoCursos.Api -c Release -o /publicacao --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=api /publicacao ./
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "CatalogoCursos.Api.dll"]
