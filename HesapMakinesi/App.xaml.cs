namespace HesapMakinesi;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		// Son seçilen temayla başla; hiç seçilmediyse sistem teması
		UserAppTheme = TemaCevir(Preferences.Default.Get("tema", "System"));
	}

	// Preferences'ta saklanan metni ("Light", "Dark", "System") MAUI'nin tema değerine çevirir
	public static AppTheme TemaCevir(string tema)
	{
		return tema switch
		{
			"Light" => AppTheme.Light,
			"Dark" => AppTheme.Dark,
			_ => AppTheme.Unspecified       // Unspecified = sistem ayarını izle
		};
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}
