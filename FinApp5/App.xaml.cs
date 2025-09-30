using FinApp5.Data;
using FinApp5.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using FinApp5.Views;

namespace FinApp5
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            Services = serviceProvider;

            MainPage = new AppShell();

            Shell.Current.GoToAsync("//MainPage");
            Routing.RegisterRoute(nameof(MenuPpal), typeof(MenuPpal));

            //MainPage = new NavigationPage(serviceProvider.GetRequiredService<MainPage>());
        }
        static SQLiteHelper db;
        public static SQLiteHelper SQLiteDB
        {
            get
            {
                if (db == null)
                {
                    try
                    {
                        db = new SQLiteHelper(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "db.db3"));
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                return db;
            }
        }

    }
}
