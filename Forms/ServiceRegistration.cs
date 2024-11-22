using Forms.ViewModels;
using Forms.ViewModels.AdministrativeStaff;
using Forms.ViewModels.ServiceSupervisor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Base;
using Repositories.Interfaces;
using Repositories.Repositories;
using Services.Interfaces.AccountantServices;
using Services.Interfaces.ServiceSupervisorServices;
using Services.Interfaces.SharedServices;
using Services.MapperProfile;
using Services.Services.AccountantServices;
using Services.Services.ServiceSupervisorServices;
using Services.Services.SharedServices;

namespace Forms
{
    public static class ServiceRegistration
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            AddRepositories(services);
            AddDatabase(services);
            AddAutoMapper(services);
            AddServices(services);
            AddViewModels(services);
        }

        public static void AddViewModels(IServiceCollection services)
        {
            services.AddScoped<LoginViewModel>();
            services.AddScoped<ApartmentViewModel>();
            //services.AddScoped<ParkingViewModel>();
            services.AddScoped<RegisterParkingViewModel>();
        }

        public static void AddServices(IServiceCollection services)
        {
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IApartmentService, ApartmentService>();
            //services.AddScoped<IParkingService, ParkingService>();
            services.AddScoped<IVehicleService, VehicleService>();

        }

        public static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public static void AddDatabase(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["QuanLyChungCuDb"].ConnectionString;
                options.UseLazyLoadingProxies().UseSqlServer(connectionString);
            });
        }

        public static void AddAutoMapper(IServiceCollection services)
        {
            var ensureServiceAssemblyLoaded = typeof(UserProfile);
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }
    }
}
