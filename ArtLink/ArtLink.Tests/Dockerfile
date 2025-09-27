FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /src

# Копируем только csproj для кэширования
COPY ArtLink.Tests/ArtLink.Tests.csproj ArtLink.Tests/
COPY ArtLink.DataAccess/ArtLink.DataAccess.csproj ArtLink.DataAccess/
COPY ArtLink.Domain/ArtLink.Domain.csproj ArtLink.Domain/
COPY Services/ArtLink.Services.Artist/ArtLink.Services.Artist.csproj Services/ArtLink.Services.Artist/
COPY Services/ArtLink.Services.Artwork/ArtLink.Services.Artwork.csproj Services/ArtLink.Services.Artwork/
COPY Services/ArtLink.Services.Contract/ArtLink.Services.Contract.csproj Services/ArtLink.Services.Contract/
COPY Services/ArtLink.Services.Employer/ArtLink.Services.Employer.csproj Services/ArtLink.Services.Employer/
COPY Services/ArtLink.Services.Portfolio/ArtLink.Services.Portfolio.csproj Services/ArtLink.Services.Portfolio/
COPY Services/ArtLink.Services.Search/ArtLink.Services.Search.csproj Services/ArtLink.Services.Search/
COPY Services/ArtLink.Services.Technique/ArtLink.Services.Technique.csproj Services/ArtLink.Services.Technique/

# Восстанавливаем зависимости
RUN dotnet restore ArtLink.Tests/ArtLink.Tests.csproj

# Копируем весь код
COPY . .

# Собираем проект
RUN dotnet build ArtLink.Tests/ArtLink.Tests.csproj -c Release --no-restore

# Устанавливаем рабочую директорию для тестов
WORKDIR /src/ArtLink.Tests

# Запускаем тесты при старте контейнера
CMD ["dotnet", "test", "--no-build", "--logger:trx"]