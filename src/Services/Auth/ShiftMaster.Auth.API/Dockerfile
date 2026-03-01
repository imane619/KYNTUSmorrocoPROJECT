FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/Auth/ShiftMaster.Auth.API/ShiftMaster.Auth.API.csproj", "Auth/"]
COPY ["src/Shared/ShiftMaster.Shared/ShiftMaster.Shared.csproj", "Shared/"]
RUN dotnet restore "Auth/ShiftMaster.Auth.API.csproj"
COPY src/ .
WORKDIR "/src/Auth"
RUN dotnet build "ShiftMaster.Auth.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShiftMaster.Auth.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "ShiftMaster.Auth.API.dll"]
