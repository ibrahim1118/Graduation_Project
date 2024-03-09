using BLL.IRepository;
using BLL.Repository;
using DAL.AuthEntity;
using DAL.Data;
using GraduationProject.API.Extentions;
using GraduationProject.API.Profiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(o => o.AddProfile(new MappingProFiles())); 

//inject DBContext
builder.Services.AddDbContext<AppDbContext>(option =>
 option.UseSqlServer(builder.Configuration.GetConnectionString("ConStr")));

builder.Services.AddScoped<IDiseaseRepositry, DiseaseRepositry>(); 

// Inject GenreicREposity
builder.Services.AddScoped(typeof(IGenricRepository<>), typeof(GenricRepository<>));
//Add Identity and JWT Services
builder.Services.AddIdentityService(builder.Configuration);

builder.Services.AddCors(Option =>
{
    Option.AddPolicy("MyPlicy", option => {
        option.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseCors("MyPlicy");

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAuthentication(); 
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllers();

app.Run();
