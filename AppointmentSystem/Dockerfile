FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Копіюємо проект за повним шляхом
COPY ["AppointmentSystem/AppointmentSystem.csproj", "AppointmentSystem/"]
RUN dotnet restore "AppointmentSystem/AppointmentSystem.csproj"

# Копіюємо все інше
COPY . .
WORKDIR "/src/AppointmentSystem"
RUN dotnet publish "AppointmentSystem.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "AppointmentSystem.dll"]