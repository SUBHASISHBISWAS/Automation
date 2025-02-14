using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SpectrailTestFramework.Interfaces;

namespace SpectrailTestFramework.Factory
{
    public class PageFactory : IPageFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PageFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T CreatePage<T>() where T : IPageObject
        {
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}
