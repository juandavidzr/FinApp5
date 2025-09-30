using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinApp5.Services
{
    public interface INavigationService
    {
        Task NavigateToAsync(Page page);
        Task GoBackAsync();
    }
}
