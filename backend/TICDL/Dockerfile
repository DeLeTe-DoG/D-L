FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["TICDL.csproj", "./"]
RUN dotnet restore "TICDL.csproj"

COPY . .
RUN dotnet publish "TICDL.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5003
ENTRYPOINT ["dotnet", "TICDL.dll"]