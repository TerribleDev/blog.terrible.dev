# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY ./src/TerribleDev.Blog.Web/*.csproj .
RUN dotnet restore -r linux-musl-x64 /p:PublishReadyToRunComposite=true

# copy everything else and build app
COPY ./src/TerribleDev.Blog.Web/ .
RUN dotnet publish -c release -o /app -r linux-musl-x64 --self-contained true --no-restore /p:PublishTrimmed=true /p:PublishReadyToRunComposite=true /p:PublishSingleFile=true
RUN date +%s > /app/buildtime.txt
# final stage/image
FROM mcr.microsoft.com/dotnet/runtime-deps:7.0-alpine-amd64
WORKDIR /app
COPY --from=build /app ./

# See: https://github.com/dotnet/announcements/issues/20
# Uncomment to enable globalization APIs (or delete)
# ENV \
#     DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
#     LC_ALL=en_US.UTF-8 \
#     LANG=en_US.UTF-8
# RUN apk add --no-cache icu-libs

ENTRYPOINT ["./TerribleDev.Blog.Web"]