#!/bin/bash

# Script para limpar o projeto Unity e resolver conflitos de assembly

echo "Limpando projeto Unity ML-Agents..."

# Remover diretórios de cache e temporários
echo "Removendo cache e arquivos temporários..."
rm -rf "Library/ScriptAssemblies"
rm -rf "Library/PackageCache/com.unity.ml-agents*"
rm -rf "Temp"

# Remover arquivos .csproj gerados automaticamente
echo "Removendo arquivos .csproj..."
rm -f *.csproj
rm -f *.sln

# Regenerar packages-lock.json
echo "Removendo packages-lock.json para regeneração..."
rm -f "Packages/packages-lock.json"

echo "Limpeza concluída!"
echo "Abra o projeto no Unity para regenerar os arquivos necessários."
