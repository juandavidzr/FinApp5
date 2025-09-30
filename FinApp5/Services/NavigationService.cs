using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinApp5.Services
{
    public class NavigationService : INavigationService
    {
        public async Task NavigateToAsync(Page page)
        {
            await Application.Current.MainPage.Navigation.PushAsync(page);
        }

        public async Task GoBackAsync()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
