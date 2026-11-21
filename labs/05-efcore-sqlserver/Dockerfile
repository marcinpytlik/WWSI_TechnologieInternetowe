FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/CourseApi/CourseApi.csproj src/CourseApi/
RUN dotnet restore src/CourseApi/CourseApi.csproj
COPY src/CourseApi/ src/CourseApi/
RUN dotnet publish src/CourseApi/CourseApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "CourseApi.dll"]
