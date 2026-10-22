#!/usr/bin/env bash
# Gera publicacao/sesi-catalogo-cursos-publicacao.zip (entregável 8 do edital):
#   app/        o que roda no servidor — `dotnet publish` com o build do front em wwwroot
#   database/   script SQL idempotente (schema + catálogo fictício)
#   Dockerfile, .dockerignore, render.yaml   como a publicação é montada
# Pode rodar de novo; a pasta publicacao/ é ignorada pelo git.
set -euo pipefail
cd "$(dirname "$0")"
export DOTNET_ROOT=~/.dotnet PATH=~/.dotnet:~/.local/node/bin:$PATH

destino=publicacao/pacote
rm -rf publicacao
mkdir -p "$destino/database"

echo "==> Build do front"
(cd frontend && npm ci --no-audit --no-fund && npm run build)

echo "==> Publicação da API (Release)"
dotnet publish src/CatalogoCursos.Api -c Release -o "$destino/app"
# Configuração de desenvolvimento (banco local) não vai para o servidor.
rm "$destino/app/appsettings.Development.json"

cp Dockerfile .dockerignore render.yaml "$destino/"
cp database/catalogo.sql "$destino/database/"

zip=publicacao/sesi-catalogo-cursos-publicacao.zip
(cd "$destino" && python3 -m zipfile -c ../sesi-catalogo-cursos-publicacao.zip .)
echo "==> $zip ($(du -h "$zip" | cut -f1))"
