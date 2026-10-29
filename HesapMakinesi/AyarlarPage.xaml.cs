namespace HesapMakinesi;

public partial class AyarlarPage : ContentPage
{
    public AyarlarPage()
    {
        InitializeComponent();

        // Sayfa açılınca kayıtlı temanın düğmesini seçili göster
        string tema = Preferences.Default.Get("tema", "System");
        rbAcik.IsChecked = tema == "Light";
        rbKoyu.IsChecked = tema == "Dark";
        rbSistem.IsChecked = tema == "System";
    }

    // Üç düğmenin ortak işleyicisi
    private void Tema_CheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (!e.Value)
            return;                      // seçimi kalkan düğmeyi yok say, yalnız yeni seçilen önemli

        string tema = (string)((RadioButton)sender!).Value;

        Application.Current!.UserAppTheme = App.TemaCevir(tema);   // bütün sayfalara anında yansır
        Preferences.Default.Set("tema", tema);                      // uygulama kapansa da hatırlanır
    }
}
