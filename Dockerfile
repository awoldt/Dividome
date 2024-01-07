FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_URLS=http://*:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["dividome.csproj", "."]
RUN dotnet restore "./dividome.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "dividome.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "dividome.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dividome.dll"]