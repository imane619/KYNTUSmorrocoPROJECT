FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Gateway/ShiftMaster.Gateway/ShiftMaster.Gateway.csproj", "Gateway/"]
COPY ["src/Shared/ShiftMaster.Shared/ShiftMaster.Shared.csproj", "Shared/"]
RUN dotnet restore "Gateway/ShiftMaster.Gateway.csproj"
COPY src/ .
WORKDIR "/src/Gateway"
RUN dotnet build "ShiftMaster.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShiftMaster.Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShiftMaster.Gateway.dll"]
