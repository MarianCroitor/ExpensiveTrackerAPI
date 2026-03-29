
секреты в переменных окружения теперь

локально:
```
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "ключ"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=expensive_tracker;Username=postgres;Password=пароль"
```

проверить: `dotnet user-secrets list`

все в ~/.microsoft/usersecrets, не коммитится

для продакшена .env с переменными, docker будет читать

TODO: переделать все ключи, старые тестовые уже в гите

