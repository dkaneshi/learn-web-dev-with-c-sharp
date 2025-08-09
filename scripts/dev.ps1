#!/usr/bin/env pwsh

Write-Host "🚀 Starting Learn C# for Web Dev - Development Environment" -ForegroundColor Green

Set-Location (Split-Path $PSScriptRoot -Parent)

if (-not (Test-Path "./data")) {
    Write-Host "📁 Creating data directory..." -ForegroundColor Yellow
    New-Item -ItemType Directory -Force -Path "./data", "./logs", "./lab-content"
}

if (-not (Test-Path "./data/learncsharp.db")) {
    Write-Host "🗄️ Initializing database..." -ForegroundColor Yellow
    Set-Location "src/LearnCSharp.Web"
    dotnet ef database update
    Set-Location "../.."
}

Write-Host "🔧 Building application..." -ForegroundColor Yellow
dotnet build

Write-Host "🌐 Starting application..." -ForegroundColor Green
Set-Location "src/LearnCSharp.Web"
dotnet run --urls="https://localhost:5001;http://localhost:5000"