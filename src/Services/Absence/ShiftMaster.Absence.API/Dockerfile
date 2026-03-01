FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/Absence/ShiftMaster.Absence.API/ShiftMaster.Absence.API.csproj", "Absence/"]
COPY ["src/Shared/ShiftMaster.Shared/ShiftMaster.Shared.csproj", "Shared/"]
RUN dotnet restore "Absence/ShiftMaster.Absence.API.csproj"
COPY src/ .
WORKDIR "/src/Absence"
RUN dotnet build "ShiftMaster.Absence.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShiftMaster.Absence.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "ShiftMaster.Absence.API.dll"]
