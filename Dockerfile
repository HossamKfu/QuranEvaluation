# ====== Base image for runtime ======
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# The default port expected by Cloud Run is 8080, so we need to expose it
EXPOSE 8080

# Configure the application to use a dynamic port (in case of cloud deployment)
ENV ASPNETCORE_URLS=http://+:8080

# ====== Build stage ======
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project file and restore any dependencies (via dotnet restore)
COPY ["Eval.csproj", "./"]
RUN dotnet restore "Eval.csproj"

# Copy the remaining files and publish the application to the /app/publish folder
COPY . .
RUN dotnet publish "Eval.csproj" -c Release -o /app/publish

# ====== Final stage ======
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish . 

# Specify entry point for the application
ENTRYPOINT ["dotnet", "Eval.dll"]
