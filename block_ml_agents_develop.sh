#!/bin/bash
# Script para bloquear a criação da pasta ml-agents-develop

PASTA_PROBLEMA="Assets/ml-agents-develop"

if [ -d "$PASTA_PROBLEMA" ]; then
    echo "⚠️  Removendo pasta problemática: $PASTA_PROBLEMA"
    rm -rf "$PASTA_PROBLEMA"
    rm -rf "${PASTA_PROBLEMA}.meta"
fi

# Criar um arquivo dummy para bloquear a criação da pasta
touch "Assets/ml-agents-develop"
echo "🔒 Pasta bloqueada com arquivo dummy"
