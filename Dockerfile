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
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LearnCSharp.Web.dll"]
