FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ThreadsPoolTest.SetMinThreadsPool/ThreadsPoolTest.SetMinThreadsPool.csproj", "ThreadsPoolTest.SetMinThreadsPool/"]
COPY ["Shared-Kernels/ThreadsPoolTest.CrossCutting.Observability/ThreadsPoolTest.CrossCutting.Observability.csproj", "Shared-Kernels/ThreadsPoolTest.CrossCutting.Observability/"]
COPY ["Applications/ThreadsPoolTest.UseCases/ThreadsPoolTest.UseCases.csproj", "Applications/ThreadsPoolTest.UseCases/"]
RUN dotnet restore "ThreadsPoolTest.SetMinThreadsPool/ThreadsPoolTest.SetMinThreadsPool.csproj"
COPY . .
WORKDIR "/src/ThreadsPoolTest.SetMinThreadsPool"
RUN dotnet build "./ThreadsPoolTest.SetMinThreadsPool.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ThreadsPoolTest.SetMinThreadsPool.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ThreadsPoolTest.SetMinThreadsPool.dll"]
