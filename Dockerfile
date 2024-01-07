FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_URLS=http://*:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["dividend-calendar.csproj", "."]
RUN dotnet restore "./dividend-calendar.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "dividend-calendar.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "dividend-calendar.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dividend-calendar.dll"]