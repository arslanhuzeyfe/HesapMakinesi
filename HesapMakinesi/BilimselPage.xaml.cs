namespace HesapMakinesi;

public partial class BilimselPage : ContentPage
{
    // Standart sayfayla aynı motor; bu sayfanın kendi ekranı ve kendi hesap durumu var
    readonly HesapMotoru motor;

    public BilimselPage()
    {
        InitializeComponent();
        motor = new HesapMotoru(lblEkran);
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

    // π ve e: tuşun yazısına göre sabiti seç
    private void Sabit_Clicked(object? sender, EventArgs e)
    {
        string tus = ((Button)sender!).Text;
        motor.Sabit(tus == "π" ? Math.PI : Math.E);
    }
}