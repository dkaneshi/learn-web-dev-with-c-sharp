#!/bin/bash

set -e

echo "🚀 Starting Learn C# for Web Dev - Development Environment"

cd "$(dirname "$0")/.."

if [ ! -d "./data" ]; then
    echo "📁 Creating data directory..."
    mkdir -p ./data ./logs ./lab-content
fi

if [ ! -f "./data/learncsharp.db" ]; then
    echo "🗄️ Initializing database..."
    cd src/LearnCSharp.Web
    dotnet ef database update
    cd ../..
fi

echo "🔧 Building application..."
dotnet build

echo "🌐 Starting application..."
cd src/LearnCSharp.Web
dotnet run --urls="https://localhost:5001;http://localhost:5000"