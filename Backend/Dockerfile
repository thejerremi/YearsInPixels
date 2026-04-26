FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["YearsInPixels.WebAPI/YearsInPixels.WebAPI.csproj", "YearsInPixels.WebAPI/"]
COPY ["YearsInPixels.Application/YearsInPixels.Application.csproj", "YearsInPixels.Application/"]
COPY ["YearsInPixels.Domain/YearsInPixels.Domain.csproj", "YearsInPixels.Domain/"]
COPY ["YearsInPixels.Infrastructure/YearsInPixels.Infrastructure.csproj", "YearsInPixels.Infrastructure/"]

RUN dotnet restore "YearsInPixels.WebAPI/YearsInPixels.WebAPI.csproj"

COPY . .
WORKDIR "/src/YearsInPixels.WebAPI"
RUN dotnet publish "YearsInPixels.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

RUN mkdir -p /app/data
ENV ConnectionStrings__DefaultConnection="Data Source=/app/data/YearsInPixels.db"

EXPOSE 8080

ENTRYPOINT ["dotnet", "YearsInPixels.WebAPI.dll"]