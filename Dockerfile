#FROM mcr.microsoft.com/dotnet/aspnet:3.1-buster-slim AS base
FROM mcr.microsoft.com/dotnet/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 8091

#FROM mcr.microsoft.com/dotnet/sdk:3.1-buster AS build
FROM mcr.microsoft.com/dotnet/sdk:3.1-alpine AS build

WORKDIR /out
COPY GoodMorning.Api ./
RUN dotnet restore "GoodMorning.Api.csproj"
COPY . .
WORKDIR "/out/GoodMorning.Api"
RUN dotnet build "GoodMorning.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GoodMorning.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS http://*:8091

RUN apk add icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "GoodMorning.Api.dll"]

