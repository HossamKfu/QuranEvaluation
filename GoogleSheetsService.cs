using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class GoogleSheetsService
{
    private static SheetsService _sheetsService;
    
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

    private static async Task<Google.Apis.Auth.OAuth2.UserCredential> GetCredentialsAsync()
    {
        // Define the OAuth2 scope
        string[] Scopes = { SheetsService.Scope.Spreadsheets };

        // Path to your credentials file
        string clientSecretJson = "credentials.json";  // Adjust this path if necessary

        // Load client secrets
        var clientSecrets = Google.Apis.Auth.OAuth2.GoogleClientSecrets.FromFile(clientSecretJson).Secrets;

        // Get the user's credentials (OAuth flow)
        var credential = await Google.Apis.Auth.OAuth2.GoogleWebAuthorizationBroker.AuthorizeAsync(
            clientSecrets,
            Scopes,
            "user",
            System.Threading.CancellationToken.None,
            new Google.Apis.Util.Store.FileDataStore("SheetsApiCredentials", true));

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
