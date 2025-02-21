using Android.App;
using Android.Content.PM;
using Android.OS;

namespace FinApp5
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]

    //[Activity(Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : MauiAppCompatActivity
    {
       
    }
}
