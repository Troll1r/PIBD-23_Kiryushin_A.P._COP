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
