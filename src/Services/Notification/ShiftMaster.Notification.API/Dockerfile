FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/Notification/ShiftMaster.Notification.API/ShiftMaster.Notification.API.csproj", "Notification/"]
COPY ["src/Shared/ShiftMaster.Shared/ShiftMaster.Shared.csproj", "Shared/"]
RUN dotnet restore "Notification/ShiftMaster.Notification.API.csproj"
COPY src/ .
WORKDIR "/src/Notification"
RUN dotnet build "ShiftMaster.Notification.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShiftMaster.Notification.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "ShiftMaster.Notification.API.dll"]
