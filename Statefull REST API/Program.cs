using Microsoft.EntityFrameworkCore;
using Stateful_REST_API.Models.EntityFramework;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<SeriesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SeriesDbContextRemote")));


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


    app.UseSwagger();
    app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();