namespace MyTaxClient;

public record MyTaxClientOptions
{
    public const string Section = "MyTax";
    /// <summary>
    /// Логин сервиса Мой налог (ИНН самозанятого).
    /// </summary>
    public string? Username { get; set; }
    /// <summary>
    /// Пароль сервиса Мой налог.
    /// </summary>
    public string? Password { get; set; }
    /// <summary>
    /// Префикс необходим для идентификации устройства/приложения, с которого будут осуществляться обращения к API.
    /// Если такой необходимости нет, то префикс можно не задавать. Примеры генерации идентификаторов:
    /// С префиксом test_: test_dfSD36BsFs9dGh2B
    /// Без префикса: hsnG5Hd8fdh7uc4HHd3Nv
    /// </summary>
    public string? DeviceIdPrefix { get; set; }
    public string? ApiUrl { get; set; }
}