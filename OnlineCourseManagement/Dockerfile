# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OnlineCourseManagement.csproj", "./"]
RUN dotnet restore "OnlineCourseManagement.csproj"
COPY . .
RUN dotnet build "OnlineCourseManagement.csproj" -c Release -o /app/build

# Publish image
FROM build AS publish
RUN dotnet publish "OnlineCourseManagement.csproj" -c Release -o /app/publish

# Final runtime container
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "OnlineCourseManagement.dll"]
