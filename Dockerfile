# ====== Base runtime image ======
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# تحديد المنفذ الذي يجب أن يستمع عليه التطبيق (HTTPS - 443)
EXPOSE 443
ENV ASPNETCORE_URLS=https://+:443

# ====== Build image ======
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# نسخ ملف المشروع وعمل restore
COPY ["Eval.csproj", "./"]
RUN dotnet restore "Eval.csproj"

# نسخ باقي الملفات وقيام الـ publish
COPY . .
RUN dotnet publish "Eval.csproj" -c Release -o /app/publish

# ====== Final runtime image ======
FROM base AS final
WORKDIR /app

# نسخ الملفات المنشورة من الحاوية المؤقتة
COPY --from=build /app/publish .

# تشغيل التطبيق
ENTRYPOINT ["dotnet", "Eval.dll"]
