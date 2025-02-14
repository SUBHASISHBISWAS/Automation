using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrailTestFramework.Interfaces
{
    public interface IHandlerFactory
    {
        T CreateHandler<T>() where T : IActionHandler;
    }
}
