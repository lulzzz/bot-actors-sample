FROM microsoft/aspnetcore:2.0.5

WORKDIR /app
COPY bin/Release/netcoreapp2.1/publish .

ENV ASPNETCORE_URLS http://+:5000

EXPOSE 5000

ENTRYPOINT ["dotnet", "SupportBot.dll"]