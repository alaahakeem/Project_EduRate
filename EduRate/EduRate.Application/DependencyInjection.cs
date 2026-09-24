using System.Reflection;
using EduRate.Application.Common.Behaviours;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace EduRate.Application
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers MediatR (with the validation pipeline behaviour) and every
        /// FluentValidation validator found in this assembly.
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            });

            services.AddValidatorsFromAssembly(assembly);

            return services;
        }
    }
}
