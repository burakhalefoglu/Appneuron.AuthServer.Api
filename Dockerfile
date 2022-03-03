FROM  mcr.microsoft.com/dotnet/aspnet:6.0.200-bullseye-slimm AS base
WORKDIR /app
EXPOSE 8000

FROM mcr.microsoft.com/dotnet/sdk:6.0.200-bullseye-slim AS build
WORKDIR /src
COPY ["WebAPI/WebAPI.csproj", "WebAPI/"]
COPY ["Business/Business.csproj", "Business/"]
COPY ["DataAccess/DataAccess.csproj", "DataAccess/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["Entities/Entities.csproj", "Entities/"]
RUN dotnet restore "WebAPI/WebAPI.csproj"
COPY . .
WORKDIR "/src/WebAPI"
RUN dotnet build "WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebAPI.csproj" -c Release -o /app/build

FROM base AS final
WORKDIR /app
COPY --from=publish /app/build .

ENV COMPlus_EnableDiagnostics=0 
ENV ASPNETCORE_URLS="http://*:8000"
ENV ASPNETCORE_ENVIRONMENT Production
ENTRYPOINT ["dotnet", "WebAPI.dll"] 
