# Перенос проекта на новое устройство и запуск из Visual Studio

Инструкция для Windows. Все команды можно выполнять в **Developer PowerShell**
(открывается из меню Visual Studio: *Tools → Terminal* или *View → Terminal*).

---

## 1. Что установить на новом устройстве

| Компонент | Зачем |
|---|---|
| **Windows 10/11** | ЛР №1–3 используют Windows Forms |
| **Visual Studio 2022** (17.12 или новее) | Основная среда для всех ЛР |
| Workload **«.NET desktop development»** | WinForms, ЛР №1–3 |
| Workload **«ASP.NET and web development»** | Web API, ЛР №4–5 |
| **.NET 9 SDK** | Все проекты target `net9.0` / `net9.0-windows` |
| **Docker Desktop + WSL2** | Контейнеры ЛР №4–5 |
| Интернет | NuGet-пакеты и Docker-образы при первой сборке |

Проверка после установки:

```powershell
dotnet --info
docker --version
docker info
```

В Docker Desktop должно быть включено использование **Linux containers**.
Для ЛР №1–3 Docker не нужен.

---

## 2. Клонирование и выбор ветки

```powershell
git clone https://github.com/Troll1r/PIBD-23_Kiryushin_A.P._COP.git
cd PIBD-23_Kiryushin_A.P._COP
git branch -a
```

В репозитории есть ветки:

* `main` — пустая;
* `lab1` … `lab5` — лабораторные;
* **`lab5`** — самая полная: в ней лежат папки всех пяти лабораторных.

Для работы сразу со всеми лабораторными:

```powershell
git switch lab5
```

Если нужна отдельная лабораторная:

```powershell
git switch lab1   # или lab2, lab3, lab4, lab5
```

> Перед переключением ветки **закройте Visual Studio**. Иначе файлы могут быть
> заняты и Git не сможет переключить ветку.

---

## 3. Первый запуск в Visual Studio

1. Откройте нужный `.sln` через **File → Open → Project/Solution**.
2. Нажмите правой кнопкой по решению → **Restore NuGet Packages**.
3. **Build → Build Solution** (или `Ctrl+Shift+B`).
4. В Solution Explorer нажмите правой кнопкой по нужному проекту →
   **Set as Startup Project**.
5. Нажмите **F5**.

Если NuGet не видит пакеты, проверьте:

* **Tools → Options → NuGet Package Manager → Package Sources**;
* для ЛР №1 должен быть источник `Lab1Local`, указывающий на
  `<репозиторий>\lab1\nuget-feed`;
* файл `lab1/NuGet.config` уже содержит этот источник.

---

## 4. Что открывать и запускать по лабораторным

| ЛР | Ветка | Решение | Startup project | Что будет |
|---|---|---|---|---|
| №1 | `lab1` | `lab1/Lab1.sln` | `Lab1.TestApp` | WinForms-тест компонента |
| №2 | `lab2` | `lab2/Lab2.sln` | `ComponentHost` | WinForms-приложение учёта продуктов |
| №3 | `lab3` | `lab3/Lab3.sln` | `ComponentHost` | Приложение + форма «Расширения» |
| №4 | `lab4` | `lab4/Lab4.sln` | `ProductCatalog.CategoriesApi` | Swagger категорий |
| №5 | `lab5` | `lab5/Lab5.sln` | — | Запуск через Docker Compose |

### ЛР №1

1. Открыть `lab1/Lab1.sln`.
2. Startup project — `Lab1.TestApp`.
3. F5.
4. Проверить добавление, дубликаты, пустую строку, выбор, set/get, очистку,
   журнал событий.

Локальный NuGet-пакет уже лежит в `lab1/nuget-feed`; отдельно собирать его
перед запуском не нужно.

### ЛР №2

1. Открыть `lab2/Lab2.sln`.
2. Startup project — `ComponentHost`.
3. F5.
4. Войти под `admin` или `user`.
5. SQLite-база `productcatalog.db` создастся автоматически в
   `lab2/src/ComponentHost/bin/Debug/net9.0-windows/`.
6. Для отчёта и продуктов добавить тестовые продукты через
   «Справочники → Продукты».

### ЛР №3

1. Открыть `lab3/Lab3.sln` — в решение входят lab1/lab2 проекты и плагины.
2. Собрать **всё решение** `Lab3.sln`, чтобы сработали post-build копирования
   плагинов.
3. Startup project — `ComponentHost`.
4. F5, войти под `admin`.
5. Открыть «Отчёты → Расширения».
6. В трёх списках должны быть форматы `xlsx`, `xlsx`, `pdf`.
7. Создать документы через SaveFileDialog.

Если плагины не появились:

* пересоберите именно `Lab3.sln`;
* проверьте папку
  `lab2/src/ComponentHost/bin/Debug/net9.0-windows/Plugins`;
* закройте запущенное приложение перед пересборкой — DLL могут быть заняты.

### ЛР №4

Для этой лабораторной используйте **ветку `lab4`** — там сервис работает на
SQLite и не требует установленного PostgreSQL.

1. `git switch lab4`.
2. Открыть `lab4/Lab4.sln`.
3. Startup project — `ProductCatalog.CategoriesApi`.
4. F5.
5. Swagger откроется по адресу из `launchSettings.json`:
   `http://localhost:5209/swagger`.
6. Проверить CRUD категорий.

Контейнерная часть ЛР №4 проверяется отдельно в терминале:

```powershell
docker build -t product-categories-api lab4/src/ProductCatalog.CategoriesApi
docker run -d -p 8080:8080 -v categories-data:/data product-categories-api
```

Swagger в контейнере: `http://localhost:8080/swagger`.

> На ветке `lab5` проект `lab4/src/ProductCatalog.CategoriesApi` уже переведён
> на PostgreSQL. Если открыть его из ветки `lab5` и нажать F5 без PostgreSQL,
> будут ошибки подключения. Для чистой ЛР №4 используйте ветку `lab4`.

### ЛР №5

ЛР №5 — это **несколько контейнеров**, поэтому обычный F5 по одному проекту
не запускает всю связку. Основной способ — Docker Compose.

1. Запустить Docker Desktop (Linux containers).
2. Открыть `lab5/Lab5.sln` в Visual Studio для просмотра/сборки кода.
3. В терминале из корня репозитория:

```powershell
docker compose -f lab5/docker-compose.yml up --build -d
docker compose -f lab5/docker-compose.yml ps
```

4. Swagger:
   * категории — `http://localhost:8080/swagger`;
   * продукты и отчёты — `http://localhost:8081/swagger`.
5. Остановка:

```powershell
docker compose -f lab5/docker-compose.yml down -v
```

#### Если нужно отлаживать API из Visual Studio

Можно запускать два API из VS, но PostgreSQL всё равно должен быть доступен.
Вариант:

1. Поднять только PostgreSQL из Compose:

```powershell
docker compose -f lab5/docker-compose.yml up -d postgres
```

2. В Visual Studio настроить **несколько startup projects**:
   правой кнопкой по решению → **Configure Startup Projects** →
   **Multiple startup projects** → `ProductCatalog.CategoriesApi` и
   `ProductCatalog.ProductsApi` → **Start**.
3. В свойствах каждого проекта (**Project → Properties → Debug → Environment
   variables**) добавить строку подключения к PostgreSQL из Compose:

```text
ConnectionStrings__Postgres=Host=localhost;Port=5433;Database=productcatalog;Username=postgres;Password=153303
```

4. Запустить оба проекта. Swagger будет на портах из `launchSettings.json`:
   категории — `http://localhost:5209`, продукты — `http://localhost:5210`.

Этот вариант удобен для отладки, но для сдачи ЛР №5 всё равно нужно показать
одновременный запуск через **Docker Compose**.

---

## 5. Что не переносится через Git

1. **PDF-файлы заданий** — они не добавлены в репозиторий. Если они нужны на
   новом устройстве, перенесите их отдельно (флешка/облако).
2. **SQLite-базы** — `productcatalog.db` создаётся заново; стартовые категории
   появятся, продукты нужно добавить.
3. **Docker volumes** — данные PostgreSQL/категорий не в Git. После
   `docker compose down -v` база создаётся заново.
4. **Тестовые продукты** — их нет в репозитории; добавьте через Swagger/API или
   через WinForms-приложение.

---

## 6. Частые проблемы

| Проблема | Решение |
|---|---|
| `NU1101` / пакет не найден | Проверить ветку, сделать Restore NuGet; для ЛР №1 проверить источник `lab1/nuget-feed` |
| Visual Studio не видит локальный пакет ЛР №1 | Tools → Options → NuGet Package Manager → Package Sources → добавить `...\lab1\nuget-feed` |
| Docker не запускается | Запустить Docker Desktop, включить WSL2/Linux containers |
| `port is already allocated` | Остановить другой контейнер/службу или изменить порт в `docker-compose.yml` |
| ЛР №5: API не подключается к БД | Проверить `docker compose ps`, healthcheck PostgreSQL, строку `ConnectionStrings__Postgres` |
| ЛР №3: нет плагинов | Пересобрать `Lab3.sln`, закрыть приложение, проверить папку `Plugins` |
| Ошибка доступа к DLL при сборке | Закрыть запущенное приложение/контейнер и пересобрать |
| Пустые отчёты | В БД нет продуктов: добавить их через приложение/API |
| ЛР №4 на ветке lab5 не работает без БД | Использовать ветку `lab4` (SQLite) или поднять PostgreSQL |

---

## 7. Минимальный чек-лист первого запуска

- [ ] Установлены .NET 9 SDK, Visual Studio 2022, нужные workloads.
- [ ] Установлен и запущен Docker Desktop с Linux containers.
- [ ] Репозиторий склонирован, выбрана нужная ветка.
- [ ] Visual Studio открывает соответствующий `.sln`.
- [ ] NuGet-пакеты восстановлены.
- [ ] ЛР №1: F5 на `Lab1.TestApp`.
- [ ] ЛР №2: F5 на `ComponentHost`.
- [ ] ЛР №3: собран `Lab3.sln`, F5 на `ComponentHost`, открыты «Расширения».
- [ ] ЛР №4: F5 на `CategoriesApi` (ветка `lab4`), проверен Swagger; отдельно
      собран Docker-образ.
- [ ] ЛР №5: `docker compose up --build -d`, проверены оба Swagger.
- [ ] При необходимости добавлены тестовые продукты для отчётов.
