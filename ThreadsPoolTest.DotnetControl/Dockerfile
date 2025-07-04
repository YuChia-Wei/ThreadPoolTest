FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ThreadsPoolTest.DotnetControl/ThreadsPoolTest.DotnetControl.csproj", "ThreadsPoolTest.DotnetControl/"]
COPY ["Shared-Kernels/ThreadsPoolTest.CrossCutting.Observability/ThreadsPoolTest.CrossCutting.Observability.csproj", "Shared-Kernels/ThreadsPoolTest.CrossCutting.Observability/"]
COPY ["Applications/ThreadsPoolTest.UseCases/ThreadsPoolTest.UseCases.csproj", "Applications/ThreadsPoolTest.UseCases/"]
RUN dotnet restore "ThreadsPoolTest.DotnetControl/ThreadsPoolTest.DotnetControl.csproj"
COPY . .
WORKDIR "/src/ThreadsPoolTest.DotnetControl"
RUN dotnet build "./ThreadsPoolTest.DotnetControl.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ThreadsPoolTest.DotnetControl.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ThreadsPoolTest.DotnetControl.dll"]
