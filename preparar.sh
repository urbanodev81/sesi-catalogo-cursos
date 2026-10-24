#!/usr/bin/env bash
# Prepara a máquina para trabalhar no teste: instala .NET 10 e Node 24 (só no
# usuário, sem sudo) se faltarem, baixa as dependências, sobe o banco e roda os
# testes. Pode rodar de novo quantas vezes quiser.
#
# Uso, numa máquina sem nada:
#   mkdir -p ~/dev/UrbanoDev/testes && cd ~/dev/UrbanoDev/testes
#   gh repo clone urbanodev81/sesi-catalogo-cursos && cd sesi-catalogo-cursos
#   ./preparar.sh
set -euo pipefail
cd "$(dirname "$0")"

NODE_VERSAO=24.18.0

if [ ! -x ~/.dotnet/dotnet ] || ! ~/.dotnet/dotnet --list-sdks | grep -q '^10\.'; then
  echo "==> Instalando .NET SDK 10 em ~/.dotnet"
  curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
  bash /tmp/dotnet-install.sh --channel 10.0 --install-dir ~/.dotnet
fi

if [ ! -x ~/.local/node/bin/node ] || ! ~/.local/node/bin/node -v | grep -q '^v24\.'; then
  echo "==> Instalando Node $NODE_VERSAO em ~/.local/node"
  mkdir -p ~/.local/node
  curl -fsSL "https://nodejs.org/dist/v$NODE_VERSAO/node-v$NODE_VERSAO-linux-x64.tar.xz" \
    | tar -xJ --strip-components=1 -C ~/.local/node
fi

if ! docker info >/dev/null 2>&1; then
  echo "!! Docker não responde. Abra o Docker Desktop (integração WSL ligada) e rode de novo."
  exit 1
fi

export DOTNET_ROOT=~/.dotnet PATH=~/.dotnet:~/.local/node/bin:$PATH

echo "==> Dependências"
dotnet tool restore
dotnet restore
(cd frontend && npm ci)

echo "==> Banco local (127.0.0.1:5470)"
docker compose up -d postgres

echo "==> Testes"
dotnet test
(cd frontend && npm test)

echo
echo "Pronto. Para trabalhar:"
echo "  export DOTNET_ROOT=~/.dotnet PATH=~/.dotnet:~/.local/node/bin:\$PATH"
echo "  dotnet run --project src/CatalogoCursos.Api   # API :5070"
echo "  (cd frontend && npm run dev)                  # front :5173"
echo "  ou tudo junto: docker compose up -d --build    # :8080"
