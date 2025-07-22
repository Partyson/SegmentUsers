FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["../NoteApp.API/NoteApp.API.csproj", "NoteApp.API/"]
COPY ["../NoteApp.Application/NoteApp.Application.csproj", "NoteApp.Application/"]
COPY ["../NoteApp.Infrastructure/NoteApp.Infrastructure.csproj", "NoteApp.Infrastructure/"]
COPY ["../NoteApp.Domain/NoteApp.Domain.csproj", "NoteApp.Domain/"]
RUN dotnet restore "NoteApp.API/NoteApp.API.csproj"
COPY . .
WORKDIR "/src/NoteApp.API"
RUN dotnet build "NoteApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NoteApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NoteApp.API.dll"]
