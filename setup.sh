#!/bin/bash

# Criar a Solution na raiz
dotnet new sln -n FinancialApp

# Criar a pasta CoreService e entrar nela
mkdir -p CoreService
cd CoreService

# Criar os projetos com seus respectivos templates
dotnet new webapi -n CoreService.API
dotnet new classlib -n CoreService.Application
dotnet new classlib -n CoreService.Domain
dotnet new classlib -n CoreService.Infrastructure

# Voltar para a raiz
cd ..

# Adicionar os projetos à Solution
dotnet sln FinancialApp.sln add CoreService/CoreService.API/CoreService.API.csproj
dotnet sln FinancialApp.sln add CoreService/CoreService.Application/CoreService.Application.csproj
dotnet sln FinancialApp.sln add CoreService/CoreService.Domain/CoreService.Domain.csproj
dotnet sln FinancialApp.sln add CoreService/CoreService.Infrastructure/CoreService.Infrastructure.csproj

echo "Estrutura do FinancialApp criada com sucesso!"
