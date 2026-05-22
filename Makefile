db:
	docker compose up postgres -d

api:
	dotnet watch --project src/CoreLedger.API/CoreLedger.API.csproj

frontend:
	cd src/frontend && pnpm dev

test:
	dotnet test

test-unit:
	dotnet test tests/CoreLedger.UnitTests/

test-integration:
	dotnet test tests/CoreLedger.IntegrationTests/

build:
	dotnet build

dev: db
	dotnet watch --project src/CoreLedger.API/CoreLedger.API.csproj

down:
	docker compose down

.PHONY: db api frontend dev test test-unit test-integration build down
