using FinApp5.Data;

namespace FinApp5
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
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
