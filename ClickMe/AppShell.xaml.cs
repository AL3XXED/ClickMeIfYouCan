using ClickMe.Views;
namespace ClickMe
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(SpielPage), typeof(SpielPage));
            Routing.RegisterRoute(nameof(Bestenliste), typeof(Bestenliste));
            Routing.RegisterRoute(nameof(Optionen), typeof(Optionen));
        }
    }
}
