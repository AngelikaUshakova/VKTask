using Microsoft.EntityFrameworkCore;
using UserManager.Repositories;
using UserManager.Services;
using UserManager.Validators;

namespace UserManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAutoMapper(typeof(MapperProfile));

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICreateUserValidator, CreateUserValidator>();
            builder.Services.AddSingleton<IPasswordService, PasswordService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserGroupRepository, UserGroupRepository>();

            const string connectionStringName = "postgres";

            var connectionString = builder.Configuration.GetConnectionString(connectionStringName);
            if (connectionString == null)
            {
                throw new Exception($"Строка подключения {connectionString} не найдена.");
            }

            // Use switch to allow DateTime mapping
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            builder.Services.AddDbContext<UserManagerContext>(builder => builder.UseNpgsql(connectionString));

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
        }
    }
}