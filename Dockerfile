FROM microsoft/dotnet:2.2-sdk As build  
WORKDIR /app

COPY *.csproj /app
RUN dotnet restore

COPY . /app
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "aspnetcore.dll"]