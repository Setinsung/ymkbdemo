using System.Text.Json;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using YMKB.ConsoleTests.Client;
using YMKB.ConsoleTests.Client.Models;

Console.WriteLine("Hello, World!");
var auth = new AnonymousAuthenticationProvider();
var adapter = new HttpClientRequestAdapter(auth)
{
    BaseUrl = "http://localhost:5045"
};
var client = new YMKBApiClient(adapter);
// var weatherForecasts = await client.WeatherForecast.GetAsync();
// foreach (var item in weatherForecasts)
// {
//     Console.WriteLine($"{item.Date} - {item.TemperatureC} - {item.Summary}");
// }
var res = await client.Auth.Login.PostAsync(new AuthRequest(){ UserName = "Administrator", Password = "P@ssw0rd!" });
Console.WriteLine(JsonSerializer.Serialize(res));

var res2 = await client.AIModels.GetAsync();
Console.WriteLine(JsonSerializer.Serialize(res2));