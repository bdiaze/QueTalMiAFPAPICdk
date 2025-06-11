using Amazon.S3;
using QueTalMiAFPAPI.Helpers;
using QueTalMiAFPAPI.Interfaces;
using QueTalMiAFPAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

string parameterArnApiAllowedDomains = Environment.GetEnvironmentVariable("PARAMETER_ARN_API_ALLOWED_DOMAINS") ?? throw new ArgumentNullException("PARAMETER_ARN_API_ALLOWED_DOMAINS");
string[] allowedDomains = (await ParameterStore.ObtenerParametro(parameterArnApiAllowedDomains)).Split(",");

builder.Services.AddControllers();

builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

builder.Services.AddCors(item => {
    item.AddPolicy("CORSPolicy", builder => {
        builder.WithOrigins(allowedDomains)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddSingleton<S3BucketHelper, S3BucketHelper>();

builder.Services.AddSingleton<ConnectionString, ConnectionString>();
builder.Services.AddSingleton<IComisionDAO, ComisionDAO>();
builder.Services.AddSingleton<ICuotaDAO, CuotaDAO>();
builder.Services.AddSingleton<ICuotaUfComisionDAO, CuotaUfComisionDAO>();
builder.Services.AddSingleton<IMensajeUsuarioDAO, MensajeUsuarioDAO>();
builder.Services.AddSingleton<ITipoMensajeDAO, TipoMensajeDAO>();
builder.Services.AddSingleton<IUfDAO, UfDAO>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CORSPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
