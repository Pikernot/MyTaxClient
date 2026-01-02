using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyTaxClient.Models;
using MyTaxClient.Models.Dto;
using MyTaxClient.Models.Dto.Authorization;

namespace MyTaxClient;

public partial class MyTaxClient(
    IOptions<MyTaxClientOptions> options,
    TimeProvider time,
    ILogger<MyTaxClient> logger)
{
    public async Task<ApproveReceiptResult> ApproveReceipt(ApproveReceiptRequest request, CancellationToken ct = default)
    {
        try
        {
            await EnsureAuthorized(ct);
            var client = _http.Value;

            var now = time.GetUtcNow();
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "income");
            requestMessage.Content = JsonContent.Create(new IncomeRequest
            {
                OperationTime = request.PaymentTime.ToString(TimeFormat),
                RequestTime = now.ToString(TimeFormat),
                Services = request.Services.ToList()
            }, null, JsonOptions);

            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _authToken);
            requestMessage.Headers.Referrer = new Uri("https://lknpd.nalog.ru/sales/create");

            using var response = await client.SendAsync(requestMessage, ct);
            if (response.IsSuccessStatusCode is false)
            {
                logger.LogError("MyTax: An error occured trying to approve receipt: {StatusCode} {MyTaxResponseText}.", response.StatusCode, await response.Content.ReadAsStringAsync(ct));
                return new ApproveReceiptResult(false, null, null);
            }

            var responseJson = await response.Content.ReadAsStringAsync(ct);
            var myTaxResponse = JsonSerializer.Deserialize<IncomeResponse>(responseJson, JsonOptions)!;
            
            var responseString = $"{response.StatusCode}\n{responseJson}";
            logger.LogInformation("MyTax: Receipt approved. UUID: {ReceiptUuid}. MyTax response: {Response}",
                myTaxResponse.ApprovedReceiptUuid, responseString);
            
            return new ApproveReceiptResult(
                IsSuccess: true,
                ReceiptUuid: myTaxResponse.ApprovedReceiptUuid,
                Inn: _inn);
        }
        catch (Exception e)
        {
            logger.LogError(e, "MyTax: An error occured trying to approve receipt.");
            return new ApproveReceiptResult(false, null, null);
        }
    }
    
    public async Task<CancelReceiptResult> CancelReceipt(CancelReceiptRequest request, CancellationToken ct = default)
    {
        try
        {
            await EnsureAuthorized(ct);
            var client = _http.Value;

            var now = time.GetUtcNow();
            var nowString = now.ToString("yyyy-MM-ddTHH:mm:ssZ");

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/cancel");
            requestMessage.Content = JsonContent.Create(new CancelRequest(
                request.ReceiptUuid,
                request.CancellationTime.ToString(TimeFormat),
                nowString,
                request.CancelReason), null, JsonOptions);

            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _authToken);
            requestMessage.Headers.Referrer = new Uri("https://lknpd.nalog.ru/sales");

            using var response = await client.SendAsync(requestMessage, ct);

            var responseString = await response.Content.ReadAsStringAsync(ct);
            var responseContent = $"{response.StatusCode}\n{responseString}";
            logger.LogInformation("MyTax: Receipt canceled. UUID: {ReceiptUuid}. Full response: {Response}",
                request.ReceiptUuid, responseString);
            return new CancelReceiptResult(response.IsSuccessStatusCode, responseContent);
        }
        catch (Exception e)
        {
            logger.LogError(e, "MyTax: An error occured trying to cancel receipt with UUID: {ReceiptUuid}.",
                request.ReceiptUuid);
            return new CancelReceiptResult(false, null);
        }
    }

    private readonly Lazy<HttpClient> _http = new(() =>
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(options.Value.ApiUrl!)
        };

        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        client.DefaultRequestHeaders.Accept.ParseAdd("text/plain");
        client.DefaultRequestHeaders.Accept.ParseAdd("*/*");
        
        client.DefaultRequestHeaders.AcceptLanguage.Clear();
        client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("ru");
        client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en;q=0.9");
        
        return client;
    });
    
    private const string TimeFormat = "yyyy-MM-ddTHH:mm:ssZ";
    
    private readonly string _deviceId = GenerateDeviceId(options.Value.DeviceIdPrefix!);
    private static string GenerateDeviceId(string prefix)
    {
        const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        var count = 21 - prefix.Length;
        if (count <= 0)
        {
            return prefix;
        }
        return prefix + new string(RandomNumberGenerator.GetItems<char>(chars.ToCharArray(), count));
    }

    private static DeviceInfo GetDeviceInfo(string deviceId) => new()
    {
        AppVersion = "1.0.0",
        SourceType = "WEB",
        SourceDeviceId = deviceId,
        MetaDetails = new DeviceInfo.DeviceInfoMetaDetails(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0")
    };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        IgnoreReadOnlyProperties = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };
}