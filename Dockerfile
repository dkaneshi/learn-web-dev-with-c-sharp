FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/LearnCSharp.Web/LearnCSharp.Web.csproj", "src/LearnCSharp.Web/"]
COPY ["src/LearnCSharp.Core/LearnCSharp.Core.csproj", "src/LearnCSharp.Core/"]
RUN dotnet restore "src/LearnCSharp.Web/LearnCSharp.Web.csproj"

COPY . .
WORKDIR "/src/src/LearnCSharp.Web"
RUN dotnet build "LearnCSharp.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LearnCSharp.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update && apt-get install -y \
    wget \
    apt-transport-https \
    && wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y dotnet-sdk-8.0 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LearnCSharp.Web.dll"]