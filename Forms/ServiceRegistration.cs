﻿using Forms.ViewModels;
using Forms.ViewModels.Accountant;
using Forms.ViewModels.AdministativeStaff;
using Forms.ViewModels.AdministrativeStaff;
using Forms.ViewModels.ServiceSupervisor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Interfaces;
using Repositories.Repositories;
using Repositories.Repositories.Base;
using Services.Interfaces.AccountantServices;
using Services.Interfaces.AdministrativeStaffServices;
using Services.Interfaces.ServiceSupervisorServices;
using Services.Interfaces.SharedServices;
using Services.MapperProfile;
using Services.Services.AccountantServices;
using Services.Services.AdministrativeStaffServices;
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
            services.AddScoped<ParkingViewModel>();
            services.AddScoped<RegisterParkingViewModel>();
            services.AddScoped<RegulationViewModel>();
            services.AddScoped<GeneralInfoViewModel>();
            services.AddScoped<ViolationViewModel>();
            services.AddScoped<DepartmentViewModel>();
            services.AddScoped<ViolationViewModel>();
            services.AddScoped<AccountViewModel>();
        }

        public static void AddServices(IServiceCollection services)
        {
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IApartmentService, ApartmentService>();
            services.AddScoped<IRegisterVehicleService, RegisterVehicleService>();
            services.AddScoped<IParkingService, ParkingService>();
            services.AddScoped<IRegulationService, RegulationService>();
            services.AddScoped<IGeneralInfoService, GeneralInfoService>();
            services.AddScoped<IViolationService, ViolationService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IViolationService, ViolationService>();
            services.AddScoped<IRegulationService, RegulationService>();
        }

        public static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IViolationRepository, ViolationRepository>();
            services.AddScoped<IRegulationRepository, RegulationRepository>();
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
