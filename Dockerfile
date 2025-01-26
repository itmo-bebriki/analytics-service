FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ./*.props ./

COPY ["src/Itmo.Bebriki.Analytics/Itmo.Bebriki.Analytics.csproj", "src/Itmo.Bebriki.Analytics/"]

COPY ["src/Application/Itmo.Bebriki.Analytics.Application/Itmo.Bebriki.Analytics.Application.csproj", "src/Application/Itmo.Bebriki.Analytics.Application/"]
COPY ["src/Application/Itmo.Bebriki.Analytics.Application.Abstractions/Itmo.Bebriki.Analytics.Application.Abstractions.csproj", "src/Application/Itmo.Bebriki.Analytics.Application.Abstractions/"]
COPY ["src/Application/Itmo.Bebriki.Analytics.Application.Contracts/Itmo.Bebriki.Analytics.Application.Contracts.csproj", "src/Application/Itmo.Bebriki.Analytics.Application.Contracts/"]
COPY ["src/Application/Itmo.Bebriki.Analytics.Application.Models/Itmo.Bebriki.Analytics.Application.Models.csproj", "src/Application/Itmo.Bebriki.Analytics.Application.Models/"]

COPY ["src/Presentation/Itmo.Bebriki.Analytics.Presentation.Kafka/Itmo.Bebriki.Analytics.Presentation.Kafka.csproj", "src/Presentation/Itmo.Bebriki.Analytics.Presentation.Kafka/"]
COPY ["src/Presentation/Itmo.Bebriki.Analytics.Presentation.Grpc/Itmo.Bebriki.Analytics.Presentation.Grpc.csproj", "src/Presentation/Itmo.Bebriki.Analytics.Presentation.Grpc/"]

COPY ["src/Infrastructure/Itmo.Bebriki.Analytics.Infrastructure.Persistence/Itmo.Bebriki.Analytics.Infrastructure.Persistence.csproj", "src/Infrastructure/Itmo.Bebriki.Analytics.Infrastructure.Persistence/"]

RUN dotnet restore "src/Itmo.Bebriki.Analytics/Itmo.Bebriki.Analytics.csproj"

COPY . .
WORKDIR "/src/src/Itmo.Bebriki.Analytics"
RUN dotnet build "Itmo.Bebriki.Analytics.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Itmo.Bebriki.Analytics.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Itmo.Bebriki.Analytics.dll"]
