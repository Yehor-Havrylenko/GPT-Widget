# Этап 1: Сборка приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Копируем все файлы в директорию приложения
COPY ./../../Backend/OpenAIChatAPI/. ./

# Восстанавливаем зависимости
RUN dotnet restore

# Компилируем приложение
RUN dotnet publish --configuration Release --output /app/publish

# Этап 2: Использование runtime-образа для запуска приложения
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Копируем собранные файлы из этапа сборки
COPY --from=build /app/publish .

# Запускаем приложение
ENTRYPOINT ["dotnet", "OpenAIChatAPI.dll"]
