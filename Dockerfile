FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/CoreLedger.SharedKernel/CoreLedger.SharedKernel.csproj", "src/CoreLedger.SharedKernel/"]
COPY ["src/CoreLedger.Domain/CoreLedger.Domain.csproj", "src/CoreLedger.Domain/"]
COPY ["src/CoreLedger.Application/CoreLedger.Application.csproj", "src/CoreLedger.Application/"]
COPY ["src/CoreLedger.Infrastructure/CoreLedger.Infrastructure.csproj", "src/CoreLedger.Infrastructure/"]
COPY ["src/CoreLedger.API/CoreLedger.API.csproj", "src/CoreLedger.API/"]
RUN dotnet restore "src/CoreLedger.API/CoreLedger.API.csproj"
COPY . .
WORKDIR "/src/src/CoreLedger.API"
RUN dotnet build "CoreLedger.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CoreLedger.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoreLedger.API.dll"]
