# GenericRepositoryPack/Src/WebAPI/Dockerfile

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# KRİTİK NOKTA: Sadece WebAPI değil, bağımlı olduğu diğer projelerin .csproj dosyalarını da

COPY ["WebAPI/GenericRepositoryPack.WebAPI.csproj", "WebAPI/"]
COPY ["Application/GenericRepositoryPack.ApplicationService.csproj", "Application/"]
COPY ["Domain/GenericRepositoryPack.Domain.csproj", "Domain/"]
COPY ["Infrastructure/GenericRepositoryPack.Infrastructure.csproj", "Infrastructure/"]
COPY ["Persistence/GenericRepositoryPack.Persistence.csproj", "Persistence/"]
COPY ["Shared/GenericRepositoryPack.Shared.csproj", "Shared/"]

# Bağımlılıkları yükle (Restore)
RUN dotnet restore "WebAPI/GenericRepositoryPack.WebAPI.csproj"

# Kalan tüm kaynak kodlarını kopyala
# (Docker context 'GenericRepositoryPack' klasörü olacağı için, tüm projeler kopyalanır)
COPY . .

# Çalışma dizinini API projesinin içine taşı
WORKDIR "/src/WebAPI"

# Build ve Publish
RUN dotnet build "GenericRepositoryPack.WebAPI.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "GenericRepositoryPack.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GenericRepositoryPack.WebAPI.dll"]