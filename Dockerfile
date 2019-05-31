FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine AS build

WORKDIR /app

COPY *.sln .
COPY dist/src/FloorLayout/*.csproj ./dist/src/FloorLayout/
COPY dist/test/FloorLayout.UnitTests/*.csproj ./dist/test/FloorLayout.UnitTests/
COPY src/Calculator/*.csproj ./src/Calculator/
COPY test/Calculator.UnitTests/*.csproj ./test/Calculator.UnitTests/
RUN dotnet restore

COPY . /app/

RUN dotnet build