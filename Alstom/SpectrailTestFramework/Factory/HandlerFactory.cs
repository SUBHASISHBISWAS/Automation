using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SpectrailTestFramework.Interfaces;

namespace SpectrailTestFramework.Factory
{
    public class HandlerFactory : IHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public HandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T CreateHandler<T>() where T : IActionHandler
        {
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}
