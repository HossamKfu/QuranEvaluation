using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Eval.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleSheetsController : ControllerBase
    {
        private readonly GoogleSheetsService _googleSheetsService;
        private readonly HttpClient _httpClient;
        private const string WebAppBaseUrl =
            "https://script.google.com/macros/s/AKfycbxubeVBj2NrgWGRYM4Xf8nTojSD6OJJz0vP3t2sYsokm0ligVkmoq1J4uH8Uj5QZ2rw/exec";

        public GoogleSheetsController(GoogleSheetsService googleSheetsService, HttpClient httpClient)
        {
            _googleSheetsService = googleSheetsService;
            _httpClient = httpClient;
        }
        [HttpGet("triggerTeacherReport")]
        public async Task TriggerTeacherReportAsync(string teacherName)
        {
            // تأكد إن الاسم مشفّر صح (خاصة لأنه عربي)
            var encodedTeacher = Uri.EscapeDataString(teacherName);
            var url = $"{WebAppBaseUrl}?teacher={encodedTeacher}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            // لو السكربت يرجّع PDF أو JSON تقدر تتعامل هنا:
            // var bytes = await response.Content.ReadAsByteArrayAsync();
            // var text = await response.Content.ReadAsStringAsync();
        }

        //public async Task TriggerTeacherReportAsync(string teacherName)
        //{
        //    // تأكد إن الاسم مشفّر صح (خاصة لأنه عربي)
        //    var encodedTeacher = Uri.EscapeDataString(teacherName);
        //    var url = $"{WebAppBaseUrl}?teacher={encodedTeacher}";

        //    var response = await _httpClient.GetAsync(url);
        //    response.EnsureSuccessStatusCode();

        //    // لو السكربت يرجّع PDF أو JSON تقدر تتعامل هنا:
        //    // var bytes = await response.Content.ReadAsByteArrayAsync();
        //    // var text = await response.Content.ReadAsStringAsync();
        //}
        // POST api/googlesheets/submit

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitData([FromBody] Dictionary<string, object> formData)
        {
            try
            {
                var spreadsheetId = "15I27Y59nOigSX0OCVz7O7sRN7IcQ0R3pH9AoKACclas";  // Replace with your actual Google Sheet ID
                var range = "WebApi";  // Specify the correct range
                var sheetData = await _googleSheetsService.GetSheetDataAsync(spreadsheetId, range);
                int lastRow = sheetData.Count() + 1; // Add 1 to get the next available row
                // Prepare the data to be inserted into Google Sheets
                var values = new List<IList<object>>();

                // Extract values from the formData and ensure they are primitives
                var row = formData.Values.Select(value =>
                {
                    // Try parsing the value as an integer first
                    if (int.TryParse(value.ToString(), out int intValue))
                    {
                        return (object)intValue;  // Return as integer if successfully parsed
                    }

                    // If parsing fails, return the value as a string
                    return value.ToString();

                }).Cast<object>().ToList();

                values.Add(row);  // Add the row to the list of values

                // Send the data to Google Sheets
                var appendRange = $"WebApi!A{lastRow}";
                await _googleSheetsService.AppendDataAsync(spreadsheetId, appendRange, values);
                await TriggerTeacherReportAsync(formData["teacherName"].ToString());
                return Ok(new { message = "Data submitted successfully!" });
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }
      
    }
}
