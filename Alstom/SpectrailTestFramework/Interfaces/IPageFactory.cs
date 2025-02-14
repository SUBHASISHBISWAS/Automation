using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrailTestFramework.Interfaces
{
    public interface IPageFactory
    {
        T CreatePage<T>() where T : IPageObject;
    }
}
