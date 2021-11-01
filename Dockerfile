#FROM mcr.microsoft.com/dotnet/aspnet:3.1-buster-slim AS base
FROM mcr.microsoft.com/dotnet/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 8091

#FROM mcr.microsoft.com/dotnet/sdk:3.1-buster AS build
FROM mcr.microsoft.com/dotnet/sdk:3.1-alpine AS build

WORKDIR /out
COPY Redis ./
RUN dotnet restore "Redis.csproj"
COPY . .
WORKDIR "/out/Redis"
RUN dotnet build "Redis.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Redis.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS http://*:8091

RUN apk add icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "Redis.dll"]

