#!/bin/bash

set -e

echo "🧪 Running tests for Learn C# for Web Dev"

cd "$(dirname "$0")/.."

echo "🔧 Building solution..."
dotnet build

echo "📋 Running unit tests..."
dotnet test tests/LearnCSharp.Tests/LearnCSharp.Tests.csproj --logger "console;verbosity=detailed"

echo "✅ All tests completed!"