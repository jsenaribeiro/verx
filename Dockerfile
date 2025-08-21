FROM mcr.microsoft.com/dotnet/sdk:8.0 AS compilation
COPY /src /src
WORKDIR /src/FluxoCaixa.WebApi
RUN dotnet publish -c Release -o /app /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS execution
WORKDIR /app
EXPOSE 80
EXPOSE 443
WORKDIR /app
COPY --from=compilation /app .
ENTRYPOINT ["dotnet", "FluxoCaixa.WebApi.dll"]
