namespace HesapMakinesi;

public partial class StandartPage : ContentPage
{
    // Hesap mantığı HesapMotoru.cs'te; bu sayfa yalnız tuşları motora iletir
    readonly HesapMotoru motor;

    public StandartPage()
    {
        InitializeComponent();
        motor = new HesapMotoru(lblEkran);   // motor bu sayfanın ekranına yazar
    }

    // Her tuş, üstündeki yazıyı (Text) motorun ilgili metoduna gönderir
    private void Rakam_Clicked(object? sender, EventArgs e) => motor.Rakam(((Button)sender!).Text);
    private void Islem_Clicked(object? sender, EventArgs e) => motor.Islem(((Button)sender!).Text);
    private void TekSayi_Clicked(object? sender, EventArgs e) => motor.TekSayi(((Button)sender!).Text);
    private void Esittir_Clicked(object? sender, EventArgs e) => motor.Esittir();
    private void Virgul_Clicked(object? sender, EventArgs e) => motor.Virgul();
    private void Sil_Clicked(object? sender, EventArgs e) => motor.Sil();
    private void Temizle_Clicked(object? sender, EventArgs e) => motor.Temizle();
    private void EkranTemizle_Clicked(object? sender, EventArgs e) => motor.EkranTemizle();
}