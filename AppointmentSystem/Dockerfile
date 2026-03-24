# Використовуємо SDK .NET 10 для збірки
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Копіюємо файл проекту та відновлюємо залежності
COPY ["AppointmentSystem.csproj", "."]
RUN dotnet restore "./AppointmentSystem.csproj"

# Копіюємо всі файли та публікуємо проект
COPY . .
RUN dotnet publish "AppointmentSystem.csproj" -c Release -o /app/publish

# Створюємо фінальний образ для запуску
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

# Вказуємо порт (Render зазвичай використовує 8080 або 10000)
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "AppointmentSystem.dll"]