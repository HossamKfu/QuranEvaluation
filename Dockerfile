# ====== Base runtime image ======
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# Render يحتاج بورت واضح، نخليه 8080
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# ====== Build image ======
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# انسخ ملف المشروع واعمَل restore
COPY ["Eval.csproj", "./"]
RUN dotnet restore "Eval.csproj"

# انسخ باقي الملفات واعمَل publish
COPY . .
RUN dotnet publish "Eval.csproj" -c Release -o /app/publish

# ====== Final runtime image ======
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Eval.dll"]
