FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /src

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

RUN dotnet restore ArtLink.Tests/ArtLink.Tests.csproj

COPY . .

RUN dotnet build ArtLink.Tests/ArtLink.Tests.csproj -c Release --no-restore
WORKDIR /src/ArtLink.Tests

RUN echo '#!/bin/bash\n\
echo "Running Unit Tests..."\n\
dotnet test --filter "Category=Unit" --logger "trx;LogFileName=unit-tests.trx" --results-directory /test-results\n\
exit $?' > /run-unit-tests.sh && chmod +x /run-unit-tests.sh

RUN echo '#!/bin/bash\n\
echo "Waiting for database..."\n\
sleep 10\n\
echo "Running Integration Tests..."\n\
dotnet test --filter "Category=Integration" --logger "trx;LogFileName=integration-tests.trx" --results-directory /test-results\n\
exit $?' > /run-integration-tests.sh && chmod +x /run-integration-tests.sh

RUN echo '#!/bin/bash\n\
echo "Waiting for database..."\n\
sleep 10\n\
echo "Running E2E Tests..."\n\
dotnet test --filter "Category=EndToEnd" --logger "trx;LogFileName=e2e-tests.trx" --results-directory /test-results\n\
exit $?' > /run-e2e-tests.sh && chmod +x /run-e2e-tests.sh 

RUN mkdir -p /test-results
