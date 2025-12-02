**Телеграм бот эхо чат на C#**

**Запуск бота**

В appsettings.json.example (EchoBot/Properties) необходимо указать все данные, 
ну и переименовать файл убрав .example, далее остается создать миграцию (dotnet ef migrations add Init --project=Persistence --startup-project=EchoBot)
находясь в корне проекта и применить ее (dotnet ef database update --project=Persistence --startup-project=EchoBot), 
ну и dontet run (F5 в Visual Studio)
