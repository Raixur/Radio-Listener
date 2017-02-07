FROM microsoft/dotnet:runtime

ENV ASPNETCORE_URLS http://*:5000

EXPOSE 5000

WORKDIR /app
ENTRYPOINT ["dotnet", "radio-scheduler.dll"]
COPY . /app
