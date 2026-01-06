using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GoogleSheetsService
{
    private static SheetsService _sheetsService;
    private readonly HttpClient _httpClient;
    public GoogleSheetsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public static async Task<SheetsService> GetSheetsServiceAsync()
    {
        if (_sheetsService == null)
        {
            var credential = await GetCredentialsAsync();

            // Create Google Sheets API service
            _sheetsService = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Sheets API .NET Quickstart"
            });
        }

        return _sheetsService;
    }

    private static async Task<UserCredential> GetCredentialsAsync()
    {
        string[] Scopes = { SheetsService.Scope.Spreadsheets };

        // جرّب تقرأ الـ JSON من Environment Variable أولاً
        string? json = Environment.GetEnvironmentVariable("GOOGLE_CREDENTIALS_JSON");

        GoogleClientSecrets clientSecrets;

        if (!string.IsNullOrEmpty(json))
        {
            // نستخدم الـ JSON القادم من الـ env var (للـ Render)
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            clientSecrets = GoogleClientSecrets.FromStream(ms);
        }
        else
        {
            // تشغيل محلي: استخدم ملف credentials.json الموجود عندك على الجهاز
            const string clientSecretJson = "credentials.json";
            clientSecrets = GoogleClientSecrets.FromFile(clientSecretJson);
        }

        var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            clientSecrets.Secrets,
            Scopes,
            "user",
            CancellationToken.None,
            new FileDataStore("SheetsApiCredentials", true));

        return credential;
    }
    // Get sheet data asynchronously
    public async Task<IList<IList<object>>> GetSheetDataAsync(string spreadsheetId, string range)
    {
        // Request to get data from Google Sheets
        await GetSheetsServiceAsync();
        var request = _sheetsService.Spreadsheets.Values.Get(spreadsheetId, range);

        try
        {
            // Execute the request
            var response = await request.ExecuteAsync();
            return response.Values;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving sheet data: {ex.Message}");
            return null; // Or handle error accordingly
        }
    }
    public async Task AppendDataAsync(string spreadsheetId, string range, IList<IList<object>> values)
    {
        try
        {
            var service = await GetSheetsServiceAsync();

            // Prepare the request body with the correct format
            var requestBody = new ValueRange() { Values = values };

            // Create the append request
            var appendRequest = service.Spreadsheets.Values.Append(requestBody, spreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;

            // Execute the request
            var response = await appendRequest.ExecuteAsync();

            // Log the number of updated cells
            Console.WriteLine($"Data appended: {response.Updates.UpdatedCells} cells updated.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error appending data: {ex.Message}");
            throw;  // Rethrow the exception for further handling
        }
    }

}
