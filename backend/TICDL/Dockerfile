FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src

COPY ["backend/TICDL/TICDL.csproj", "backend/TICDL/"]
RUN dotnet restore "backend/TICDL/TICDL.csproj"

COPY . .
WORKDIR "/src/backend/TICDL"
RUN dotnet publish "TICDL.csproj" -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 5003
ENV ASPNETCORE_URLS=http://+:5003

ENTRYPOINT ["dotnet", "TICDL.dll"]