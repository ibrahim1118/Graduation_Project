using DAL.AuthEntity;
using DAL.Data;
using GraduationProject.API.Extentions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//inject DBContext
builder.Services.AddDbContext<AppDbContext>(option =>
 option.UseSqlServer(builder.Configuration.GetConnectionString("ConStr")));

//Add Identity and JWT Services
builder.Services.AddIdentityService(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllers();

app.Run();
