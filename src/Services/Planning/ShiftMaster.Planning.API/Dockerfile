FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/Planning/ShiftMaster.Planning.API/ShiftMaster.Planning.API.csproj", "Planning/"]
COPY ["src/Shared/ShiftMaster.Shared/ShiftMaster.Shared.csproj", "Shared/"]
RUN dotnet restore "Planning/ShiftMaster.Planning.API.csproj"
COPY src/ .
WORKDIR "/src/Planning"
RUN dotnet build "ShiftMaster.Planning.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShiftMaster.Planning.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "ShiftMaster.Planning.API.dll"]
