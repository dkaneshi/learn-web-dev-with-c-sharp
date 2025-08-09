.PHONY: help install build test run clean dev seed

help: ## Show this help message
	@echo 'Usage: make [target]'
	@echo ''
	@echo 'Targets:'
	@awk 'BEGIN {FS = ":.*?## "} /^[a-zA-Z_-]+:.*?## / {printf "  \033[36m%-15s\033[0m %s\n", $$1, $$2}' $(MAKEFILE_LIST)

install: ## Install dependencies
	dotnet restore

build: ## Build the application
	dotnet build --configuration Release

test: ## Run tests
	dotnet test --configuration Release --logger "console;verbosity=detailed"

test-watch: ## Run tests in watch mode
	dotnet watch test --project tests/LearnCSharp.Tests

run: ## Run the application
	cd src/LearnCSharp.Web && dotnet run

dev: ## Start development environment
	chmod +x scripts/dev.sh && ./scripts/dev.sh

clean: ## Clean build artifacts
	dotnet clean
	rm -rf src/*/bin src/*/obj tests/*/bin tests/*/obj

seed: ## Seed the database with sample data
	cd src/LearnCSharp.Web && dotnet run --seed-only

format: ## Format code
	dotnet format

docker-build: ## Build Docker image
	docker build -t learncsharp:latest .

docker-run: ## Run Docker container
	docker run -p 5173:8080 --name learncsharp learncsharp:latest

docker-stop: ## Stop and remove Docker container
	docker stop learncsharp || true
	docker rm learncsharp || true

docker-dev: ## Start development environment with Docker Compose
	docker-compose up --build

coverage: ## Generate code coverage report
	dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage
	@echo "Coverage report generated in ./coverage"

migrate: ## Apply database migrations
	cd src/LearnCSharp.Web && dotnet ef database update

migration: ## Create new migration (usage: make migration NAME=MigrationName)
	cd src/LearnCSharp.Web && dotnet ef migrations add $(NAME) --project ../LearnCSharp.Core

reset-db: ## Reset the database (development only)
	rm -f data/learncsharp.db
	cd src/LearnCSharp.Web && dotnet ef database update

lint: ## Run linting
	dotnet format --verify-no-changes --verbosity diagnostic

all: install build test ## Install, build, and test