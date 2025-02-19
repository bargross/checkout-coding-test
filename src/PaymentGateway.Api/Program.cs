using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using PaymentGateway.Api.HttpClients;
using PaymentGateway.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMvcCore()    
    .AddNewtonsoftJson(o =>
    {
        o.SerializerSettings.Converters.Add(new StringEnumConverter());
        o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    });

builder.Services.AddSingleton<IBankClient>( _ =>
{
    var client = new HttpClient();
    client.BaseAddress = new Uri("http://localhost:8080/");
    client.Timeout = TimeSpan.FromSeconds(10);
    
    return new BankClient(client);
});

builder.Services.AddSingleton<PaymentsRepository>();
builder.Services.AddSingleton<IPaymentProcessor, PaymentProcessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
