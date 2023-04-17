using System.Net.Http;
using System.Text.Json;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 設定 HttpClient 以傳送 POST 請求
var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("http://localhost:8888");

app.MapGet("TEST", async () => { return "TEST"; });

app.MapPost("/MashupTrans/JiraWebHook", async (HttpContext context) =>
{
    // 讀取 POST 請求的 Body 內容
    var requestBody = await context.Request.ReadFromJsonAsync<JsonElement>();

    // 將讀取到的 JSON 內容序列化為字串
    var requestBodyString = JsonSerializer.Serialize(requestBody);

    // 將字串轉換為 HttpContent
    var httpContent = new StringContent(requestBodyString, Encoding.UTF8, "application/json");

    // 傳送 POST 請求到指定的 API 位址
    var httpResponse = await httpClient.PostAsync("https://localhost:9090/MaShup_Trans/TransWS.asmx/JiraCallback", httpContent);

    // 讀取回應內容
    var responseContent = await httpResponse.Content.ReadAsStringAsync();

    // 將回應內容寫回給用戶端
    await context.Response.WriteAsync(responseContent);
});

app.Run();