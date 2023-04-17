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

// �]�w HttpClient �H�ǰe POST �ШD
var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("http://localhost:8888");

app.MapGet("TEST", async () => { return "TEST"; });

app.MapPost("/MashupTrans/JiraWebHook", async (HttpContext context) =>
{
    // Ū�� POST �ШD�� Body ���e
    var requestBody = await context.Request.ReadFromJsonAsync<JsonElement>();

    // �NŪ���쪺 JSON ���e�ǦC�Ƭ��r��
    var requestBodyString = JsonSerializer.Serialize(requestBody);

    // �N�r���ഫ�� HttpContent
    var httpContent = new StringContent(requestBodyString, Encoding.UTF8, "application/json");

    // �ǰe POST �ШD����w�� API ��}
    var httpResponse = await httpClient.PostAsync("https://localhost:9090/MaShup_Trans/TransWS.asmx/JiraCallback", httpContent);

    // Ū���^�����e
    var responseContent = await httpResponse.Content.ReadAsStringAsync();

    // �N�^�����e�g�^���Τ��
    await context.Response.WriteAsync(responseContent);
});

app.Run();