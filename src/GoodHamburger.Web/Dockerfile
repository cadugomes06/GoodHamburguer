FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY GoodHamburger.sln .
COPY src/GoodHamburger.Web/GoodHamburger.Web.csproj src/GoodHamburger.Web/

RUN dotnet restore src/GoodHamburger.Web/GoodHamburger.Web.csproj

COPY src/GoodHamburger.Web/ src/GoodHamburger.Web/

RUN dotnet publish src/GoodHamburger.Web/GoodHamburger.Web.csproj -c Release -o /app/publish

FROM nginx:alpine AS final
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
