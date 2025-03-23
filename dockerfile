# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /source

# copy csproj and restore as distinct layers
COPY . .
RUN dotnet restore -a $TARGETARCH

# copy and publish app and libraries
RUN dotnet publish -a $TARGETARCH --no-restore -o /app


# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 8080
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
ENTRYPOINT ["./AttendanceDatabase"]


#
# how to use this:

# build image using dockerfile
#> docker build -t mocajax-image -f Dockerfile .

# run the image and listen on localhost:8384
#> docker run -it --rm -p 5000:8080 --name mocajax-container mocajax-image


# explicitly create container from the image (this is clunky)
#> docker create --name mocajax-container mocajax-image
