# Лабораторная работа №5. Разработка микросервисного приложения

**Вариант 13 — «Учет продуктов в магазине. Категории изделий».**
Выполнил: Кирюшин А. П., группа ПИБД-23.

## 1. Задание

Реализовать несколько микросервисов, настроить их взаимодействие и одновременный
запуск:

1. первый микросервис — категории изделий, данные хранятся в PostgreSQL;
2. второй микросервис — продукты:
   * CRUD по продуктам с валидацией;
   * второй контроллер — выдача файлов отчётов;
   * категория продукта берётся из первого микросервиса;
3. PostgreSQL и оба сервиса запускаются через Docker Compose.

Второй сервис общается с первым через **общую базу данных PostgreSQL**: таблица
`Categories` принадлежит первому сервису, таблица `Products` — второму.

## 2. Архитектура

```
┌──────────────────────────────────────────────────────────────┐
│                     docker-compose.yml                       │
│                                                              │
│  ┌─────────────────┐      ┌────────────────────────────────┐ │
│  │   PostgreSQL    │◄─────│      categories-api            │ │
│  │  productcatalog │      │  (первый микросервис, lab4)    │ │
│  │                 │      │  /api/categories               │ │
│  │  Categories     │      └────────────────────────────────┘ │
│  │  Products       │◄─────┌────────────────────────────────┐ │
│  │                 │      │       products-api             │ │
│  └─────────────────┘      │  (второй микросервис, lab5)    │ │
│                           │  /api/products                 │ │
│                           │  /api/reports/*                │ │
│                           └────────────────────────────────┘ │
└──────────────────────────────────────────────────────────────┘
```

Первый сервис — `ProductCatalog.CategoriesApi` из лабораторной №4. В рамках
лабораторной №5 он переведён с SQLite на PostgreSQL; CRUD категорий остался
прежним. Второй сервис — `ProductCatalog.ProductsApi`.

Docker Compose одновременно запускает:

| Сервис | Контейнер | Порты |
|---|---|---|
| PostgreSQL 16 | `lab5-postgres` | `5433:5432` |
| Категории | `lab5-categories-api` | `8080:8080` |
| Продукты | `lab5-products-api` | `8081:8080` |

Порт `5433` на хосте выбран, чтобы не конфликтовать с локальным PostgreSQL,
который может занимать `5432`. Внутри сети Compose сервисы обращаются к БД по
имени `postgres:5432`.

## 3. База данных

Используется общий контейнер `postgres:16`, база `productcatalog`.

```sql
CREATE TABLE Categories (
    Id   SERIAL PRIMARY KEY,
    Name TEXT NOT NULL UNIQUE
);

CREATE TABLE Products (
    Id          SERIAL PRIMARY KEY,
    Name        TEXT NOT NULL,
    Description TEXT NOT NULL DEFAULT '',
    Category    TEXT NOT NULL,
    Quantity    INTEGER NULL
);
```

Первый сервис отвечает за CRUD категорий. Второй сервис проверяет, что
переданная категория существует в `Categories`, и хранит её в `Products` как
текст. При первом запуске создаются стартовые категории:
«Молочные продукты», «Хлебобулочные изделия», «Напитки», «Бакалея».

## 4. Первый микросервис — категории

Базовый маршрут: `/api/categories`.

| Метод | Маршрут | Назначение |
|---|---|---|
| `GET` | `/api/categories` | Список категорий |
| `GET` | `/api/categories/{name}` | Категория по названию |
| `POST` | `/api/categories` | Добавить категорию |
| `PUT` | `/api/categories/{name}` | Переименовать |
| `DELETE` | `/api/categories/{name}` | Удалить |

Swagger: `http://localhost:8080/swagger`.

## 5. Второй микросервис — продукты

### 5.1. Контроллер продуктов

Базовый маршрут: `/api/products`.

| Метод | Маршрут | Назначение |
|---|---|---|
| `GET` | `/api/products` | Список всех продуктов |
| `GET` | `/api/products/{id}` | Продукт по Id |
| `POST` | `/api/products` | Добавить продукт |
| `PUT` | `/api/products/{id}` | Изменить продукт |
| `DELETE` | `/api/products/{id}` | Удалить продукт |

Тело `POST`/`PUT`:

```json
{
  "name": "Молоко 3,2%",
  "description": "Пастеризованное молоко 1 л",
  "category": "Молочные продукты",
  "quantity": 10
}
```

Валидация:

* название и категория обязательны;
* категория должна существовать в таблице `Categories` (данные первого
  микросервиса);
* количество может быть `null`, но не может быть отрицательным.

### 5.2. Контроллер отчётов

Базовый маршрут: `/api/reports`. Файлы формируются сразу при вызове метода,
без плагинной инфраструктуры.

| Метод | Маршрут | Результат |
|---|---|---|
| `GET` | `/api/reports/products-in-stock` | Excel: продукты в наличии, название и описание |
| `GET` | `/api/reports/out-of-stock-pie` | PDF: круговая диаграмма отсутствующих продуктов по категориям |
| `GET` | `/api/reports/all-products` | Word: все продукты, таблица с шапкой из первой строки и первого столбца |

Word-таблица:

* первая строка: пустая ячейка, «Название», «Категория», «Количество»;
* первый столбец: идентификаторы продуктов;
* если количество не задано — выводится «отсутствует».

Swagger второго сервиса: `http://localhost:8081/swagger`.

## 6. Состав решения

```
lab5/
├── Lab5.sln
├── README.md
├── docker-compose.yml
└── src/
    └── ProductCatalog.ProductsApi/
        ├── Controllers/
        │   ├── ProductsController.cs
        │   └── ReportsController.cs
        ├── Data/
        │   └── ProductsDatabase.cs
        ├── Models/
        │   └── Product.cs
        ├── Reports/
        │   └── ReportGenerator.cs
        ├── Properties/
        │   └── launchSettings.json
        ├── Dockerfile
        ├── .dockerignore
        ├── ProductCatalog.ProductsApi.csproj
        ├── ProductCatalog.ProductsApi.http
        ├── Program.cs
        └── appsettings.json
```

Первый микросервис находится в `lab4/src/ProductCatalog.CategoriesApi` и
включён в `Lab5.sln`. Его Dockerfile используется Docker Compose.

## 7. Запуск через Docker Compose

Из корня репозитория:

```powershell
docker compose -f lab5/docker-compose.yml up --build -d
```

Проверка контейнеров:

```powershell
docker compose -f lab5/docker-compose.yml ps
```

Адреса:

* категории + Swagger: `http://localhost:8080/swagger`;
* продукты + Swagger: `http://localhost:8081/swagger`;
* PostgreSQL на хосте: `localhost:5433`.

Остановка и удаление контейнеров/volume:

```powershell
docker compose -f lab5/docker-compose.yml down -v
```

Пароль PostgreSQL в Docker Compose (`153303`) предназначен только для учебного
локального стенда.

## 8. Проверка

После запуска Compose:

```powershell
# категории (данные первого сервиса)
curl http://localhost:8080/api/categories

# продукты
curl http://localhost:8081/api/products

# добавить продукт
curl -X POST http://localhost:8081/api/products `
  -H "Content-Type: application/json" `
  -d '{"name":"Молоко 3,2%","description":"Пастеризованное молоко 1 л","category":"Молочные продукты","quantity":10}'

# отчёты
curl -o products-in-stock.xlsx http://localhost:8081/api/reports/products-in-stock
curl -o out-of-stock-pie.pdf http://localhost:8081/api/reports/out-of-stock-pie
curl -o all-products.docx http://localhost:8081/api/reports/all-products
```

Проверено:

* PostgreSQL запускается в контейнере, healthcheck проходит;
* оба микросервиса запускаются одновременно через Docker Compose;
* `GET /api/categories` возвращает стартовые категории;
* `POST /api/products` создаёт продукт, `GET` возвращает список;
* `PUT` изменяет продукт, `DELETE` удаляет;
* продукт с несуществующей категорией отклоняется с `400 Bad Request`;
* Excel-отчёт содержит только продукты в наличии;
* Word-отчёт содержит все продукты, идентификаторы и «отсутствует» для null;
* PDF-отчёт корректно открывается и содержит круговую диаграмму;
* Swagger доступен у обоих сервисов (`200 OK`);
* сборка решения — **0 ошибок, 0 предупреждений**.

## 9. Краткий вывод

Реализовано микросервисное приложение из двух сервисов и контейнера
PostgreSQL. Первый сервис предоставляет CRUD категорий, второй — CRUD продуктов
с валидацией категории и три отчёта (Excel, PDF, Word). Взаимодействие
организовано через общую БД, запуск всех контейнеров — через Docker Compose.
