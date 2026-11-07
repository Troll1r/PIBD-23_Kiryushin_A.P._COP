# Лабораторная работа №4. Разработка микросервиса

**Вариант 13 — «Учет продуктов в магазине. Категории изделий».**
Выполнил: Кирюшин А. П., группа ПИБД-23.

## 1. Задание

Реализовать простой микросервис для CRUD-операций над справочной сущностью
«Категория изделия» (одно текстовое поле — название категории), упаковать его
в Docker-контейнер и проверить работу через Swagger/Postman.

Микросервис:

* ASP.NET Core Web API на .NET 9;
* без дополнительных слоёв — работа с БД находится прямо в контроллере;
* хранит данные в SQLite;
* имеет Swagger UI;
* собирается и запускается в Docker-контейнере.

## 2. Сущность

Категория изделия — справочник из одной текстовой сущности:

| Поле | Тип | Описание |
|---|---|---|
| `Id` | `long` | Идентификатор (внутренний) |
| `Name` | `string` | Название категории, уникальное |

Таблица SQLite:

```sql
CREATE TABLE Categories (
    Id   INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE
);
```

При первом создании БД добавляются стартовые категории:
«Молочные продукты», «Хлебобулочные изделия», «Напитки», «Бакалея».

## 3. REST API

Базовый маршрут: `/api/categories`.

| Метод | Маршрут | Назначение |
|---|---|---|
| `GET` | `/api/categories` | Получить все категории |
| `GET` | `/api/categories/{name}` | Получить категорию по названию |
| `POST` | `/api/categories` | Добавить категорию |
| `PUT` | `/api/categories/{name}` | Изменить категорию: `{name}` — старое название, тело — новое |
| `DELETE` | `/api/categories/{name}` | Удалить категорию |

Тело запроса для `POST` и `PUT`:

```json
{
  "name": "Кондитерские изделия"
}
```

Коды ответов:

* `GET` — `200 OK` или `404 Not Found`;
* `POST` — `201 Created` (созданная запись), `400 Bad Request`, `409 Conflict`;
* `PUT` — `204 No Content`, `400`, `404`, `409`;
* `DELETE` — `204 No Content` или `404`.

## 4. Структура проекта

```
lab4/
├── Lab4.sln
├── README.md
└── src/
    └── ProductCatalog.CategoriesApi/
        ├── Controllers/
        │   └── CategoriesController.cs
        ├── Models/
        │   └── Category.cs
        ├── Properties/
        │   └── launchSettings.json
        ├── Dockerfile
        ├── .dockerignore
        ├── ProductCatalog.CategoriesApi.csproj
        ├── ProductCatalog.CategoriesApi.http
        ├── Program.cs
        └── appsettings.json
```

`CategoriesController` содержит всю логику CRUD и работу с SQLite. Отдельных
репозиториев, сервисов и Unit of Work нет — по заданию логика находится прямо
в контроллере.

## 5. Swagger

Swagger UI включён всегда (в том числе в контейнере):

* локально: `http://localhost:5209/swagger`;
* в контейнере: `http://localhost:8080/swagger`.

OpenAPI JSON: `/swagger/v1/swagger.json`.

## 6. Запуск локально

Требования: .NET 9 SDK.

```powershell
dotnet build lab4/Lab4.sln
dotnet run --project lab4/src/ProductCatalog.CategoriesApi/ProductCatalog.CategoriesApi.csproj
```

Адрес локального запуска задан в `launchSettings.json`:
`http://localhost:5209`.

Проверка через `curl`:

```powershell
curl http://localhost:5209/api/categories

curl -X POST http://localhost:5209/api/categories `
  -H "Content-Type: application/json" `
  -d '{"name":"Кондитерские изделия"}'

curl -X PUT http://localhost:5209/api/categories/Кондитерские%20изделия `
  -H "Content-Type: application/json" `
  -d '{"name":"Сладости"}'

curl -X DELETE http://localhost:5209/api/categories/Сладости
```

Также можно использовать `ProductCatalog.CategoriesApi.http` в Visual Studio
или Postman.

## 7. Docker

Dockerfile находится рядом с `.csproj`:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ProductCatalog.CategoriesApi.csproj", "./"]
RUN dotnet restore "ProductCatalog.CategoriesApi.csproj"
COPY . .
RUN dotnet publish "ProductCatalog.CategoriesApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

USER root
RUN mkdir -p /data && chown -R app:app /data
USER app

ENV ASPNETCORE_URLS=http://+:8080
ENV CATEGORIES_DB_PATH=/data/categories.db
EXPOSE 8080
ENTRYPOINT ["dotnet", "ProductCatalog.CategoriesApi.dll"]
```

Сборка и запуск:

```powershell
docker build -t product-categories-api lab4/src/ProductCatalog.CategoriesApi

docker run -d --name product-categories-api `
  -p 8080:8080 `
  -v categories-data:/data `
  product-categories-api
```

Проверка:

* Swagger UI: `http://localhost:8080/swagger`;
* API: `http://localhost:8080/api/categories`.

Журнал контейнера:

```powershell
docker logs product-categories-api
```

Остановка и удаление:

```powershell
docker rm -f product-categories-api
docker volume rm categories-data
```

## 8. Проверка

Проверено локально и в Docker-контейнере:

* `GET /api/categories` возвращает 4 стартовые категории;
* `POST` добавляет категорию и возвращает `201 Created`;
* `GET /api/categories/{name}` находит добавленную запись;
* `PUT` переименовывает категорию и возвращает `204 No Content`;
* `DELETE` удаляет категорию и возвращает `204 No Content`;
* `GET` после удаления возвращает исходные 4 записи;
* Swagger UI и OpenAPI JSON доступны (`200 OK`);
* контейнер запускается, слушает порт 8080 и сохраняет БД в `/data`
  (volume `categories-data`).

Сборка проекта: **0 ошибок, 0 предупреждений**.

## 9. Краткий вывод

Реализован простой микросервис для CRUD-операций со справочником категорий
изделий. Логика работы с SQLite размещена непосредственно в контроллере,
сервис документирован через Swagger и упакован в Docker-образ. Работа
проверена как при локальном запуске, так и при запуске в контейнере.
