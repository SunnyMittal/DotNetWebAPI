#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DotNetWebAPI.csproj", "./"]
RUN dotnet restore "./DotNetWebAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "DotNetWebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DotNetWebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# install dependencies
RUN apt-get update && apt-get install -y zip curl
RUN mkdir /otel
RUN curl -L -o /otel/otel-dotnet-install.sh https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/releases/download/v1.3.0/otel-dotnet-auto-install.sh
RUN chmod +x /otel/otel-dotnet-install.sh

ENV OTEL_DOTNET_AUTO_HOME=/otel

RUN /bin/bash /otel/otel-dotnet-install.sh
RUN chmod +x /otel/instrument.sh

# run env setup script and then start the dotnet application on container startup
USER app
ENTRYPOINT ["/bin/bash", "/otel/instrument.sh", "dotnet DotNetWebAPI.dll"]
