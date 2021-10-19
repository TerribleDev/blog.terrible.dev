FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["./TerribleDev.Blog.Web.csproj", "."]
RUN dotnet restore "TerribleDev.Blog.Web.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "TerribleDev.Blog.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "TerribleDev.Blog.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TerribleDev.Blog.Web.dll"]
