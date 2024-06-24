using BLL.IRepository;
using BLL.Repository;
using BLL.UnitOfwrok;
using DAL.AuthEntity;
using DAL.Data;
using GraduationProject.API.Extentions;
using GraduationProject.API.Profiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(/*option =>
 option.AddSecurityDefinition(name: "Bearer", new OpenApiSecurityScheme
 {
     Name = "Authorization" , 
     Type = SecuritySchemeType.ApiKey, 
     Scheme = "Bearer", 
     BearerFormat = "JWT", 
     In = ParameterLocation.Header, 
     Description = "Enter your Jwt key"
 })*/);

builder.Services.AddAutoMapper(o => o.AddProfile(new MappingProFiles())); 

//inject DBContext
builder.Services.AddDbContext<AppDbContext>(option =>
 option.UseSqlServer(builder.Configuration.GetConnectionString("ConStr")));

/*builder.Services.AddScoped<IDiseaseRepositry, DiseaseRepositry>(); 
builder.Services.AddScoped<IPostRepositry, PostRepostiry>();*/ 
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); 
/*builder.Services.AddScoped<ICommentRepositry, CommentRepositry>();
builder.Services.AddScoped<IReviewRepository, ReviewsRepositry>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();*/
builder.Services.AddHttpClient();
// Inject GenreicREposity
//builder.Services.AddScoped(typeof(IGenricRepository<>), typeof(GenricRepository<>));
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

app.UseStaticFiles();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseCors("MyPlicy");

app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllers();

app.Run();
