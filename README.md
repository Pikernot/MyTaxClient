# MyTaxClient – клиент "Мой налог" [![NuGet](https://img.shields.io/nuget/v/MyTaxClient.svg)](https://www.nuget.org/packages/MyTaxClient)

Клиент для работы с API сервиса "Мой налог" от ФНС РФ для самозанятых. Позволяет автоматически регистрировать и аннулировать чеки.

## Быстрый старт

1. Установите пакет:
```bash
dotnet add package MyTaxClient
```

2. Зарегистрируйте клиент (Program.cs):
```csharp
using MyTaxClient;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMyTaxClient(builder.Configuration);
```

3. Сконфигурируйте (appsettings.json):
```json
{
  "MyTax": {
    "Username": "ВАШ_ИНН",
    "Password": "ВАШ_ПАРОЛЬ",
    "DeviceIdPrefix": "test_", // Опционально
    "ApiUrl": "https://lknpd.nalog.ru/api/v1/" // Опционально
  }
}
```

Или через переменные окружения:
```
MyTax__Username=ВАШ_ИНН
MyTax__Password=ВАШ_ПАРОЛЬ
```

## Конфигурация

Все параметры конфигурации (`MyTaxClientOptions`):

| Параметр        | Описание                                                        |
|-----------------|-----------------------------------------------------------------|
| Username        | ИНН самозанятого (обязательный)                                 |
| Password        | Пароль от сервиса "Мой налог" (обязательный)                    |
| DeviceIdPrefix  | Префикс для генерации DeviceId (по умолчанию пустая строка)     |
| ApiUrl          | URL API сервиса (по умолчанию `https://lknpd.nalog.ru/api/v1/`) |

Пример ручной настройки без `IConfiguration`:
```csharp
builder.Services.AddMyTaxClient(configure: options => 
{
    options.Username = "ВАШ_ИНН";
    options.Password = "ВАШ_ПАРОЛЬ";
});
```

## Примеры использования

### Регистрация чека
```csharp
await myTaxClient.ApproveReceiptAsync(new ApproveReceiptRequest
{
    Services: [ new Service(Name: "Услуга1", Quantity: 1, Amount: 100.00M) ],
    PaymentTime: DateTimeOffset.UtcNow
});
```

### Аннулирование чека
```csharp
await myTaxClient.CancelReceipt(new CancelReceiptRequest
{
    ReceiptUuid: "tv8aiukhu3"
    CancellationTime: DateTimeOffset.UtcNow
});
```