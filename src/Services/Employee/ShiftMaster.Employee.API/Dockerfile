FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/Employee/ShiftMaster.Employee.API/ShiftMaster.Employee.API.csproj", "Employee/"]
COPY ["src/Shared/ShiftMaster.Shared/ShiftMaster.Shared.csproj", "Shared/"]
RUN dotnet restore "Employee/ShiftMaster.Employee.API.csproj"
COPY src/ .
WORKDIR "/src/Employee"
RUN dotnet build "ShiftMaster.Employee.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShiftMaster.Employee.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "ShiftMaster.Employee.API.dll"]
