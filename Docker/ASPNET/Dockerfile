FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY ./../../Backend/OpenAIChatAPI/. ./


RUN dotnet restore

RUN dotnet build --configuration Release

CMD ["dotnet", "run", "--configuration", "Release", "--urls", "http://0.0.0.0:5000"]