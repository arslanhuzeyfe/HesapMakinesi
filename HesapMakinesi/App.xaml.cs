namespace HesapMakinesi;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		// Son seçilen temayla başla; hiç seçilmediyse sistem teması
		UserAppTheme = TemaCevir(Preferences.Default.Get("tema", "System"));

		// Ayarlar'da tema değişince Windows'un kendi çizdiği parçaları da güncelle
		RequestedThemeChanged += (s, e) => YerelTemayiEsitle();
	}

	// Windows, ☰ menü tuşu ve menü şeridi gibi kendi parçalarını uygulamanın değil sistemin
	// temasıyla çizer; koyu sistemde Açık tema seçilince ☰ görünmez oluyordu. Burada pencerenin
	// kökündeki WinUI temasını uygulamanın temasına eşitliyoruz. Diğer platformlarda bir şey yapmaz.
	void YerelTemayiEsitle()
	{
#if WINDOWS
		if (this.Windows.FirstOrDefault()?.Handler?.PlatformView is Microsoft.UI.Xaml.Window pencere
			&& pencere.Content is Microsoft.UI.Xaml.FrameworkElement kok)
		{
			kok.RequestedTheme = UserAppTheme switch
			{
				AppTheme.Light => Microsoft.UI.Xaml.ElementTheme.Light,
				AppTheme.Dark => Microsoft.UI.Xaml.ElementTheme.Dark,
				_ => Microsoft.UI.Xaml.ElementTheme.Default      // Sistem: Windows'un temasını izle
			};
		}
#endif
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
		// Masaüstünde bu boyutla açılır, sonra kullanıcı istediği gibi büyütüp küçültebilir (Android'de etkisi yok)
		var pencere = new Window(new AppShell())
		{
			Width = 400,
			Height = 640
		};
		pencere.Created += (s, e) => YerelTemayiEsitle();   // açılışta kayıtlı temayı Windows parçalarına da uygula
		return pencere;
	}
}
