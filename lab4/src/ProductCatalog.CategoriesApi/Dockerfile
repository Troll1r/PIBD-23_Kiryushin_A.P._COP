FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ProductCatalog.CategoriesApi.csproj", "./"]
RUN dotnet restore "ProductCatalog.CategoriesApi.csproj"
COPY . .
RUN dotnet publish "ProductCatalog.CategoriesApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "ProductCatalog.CategoriesApi.dll"]
