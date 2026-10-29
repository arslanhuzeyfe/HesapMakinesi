using System.Globalization;

namespace HesapMakinesi;

public partial class StandartPage : ContentPage
{
    // Hesap durumu: metotlar arasında taşınan bilgiler
    double ilkSayi = 0;              // işlem tuşundan önce girilen sayı
    string bekleyenIslem = "";       // "+", "−", "×", "÷" ya da boş
    bool yeniSayiBasliyor = false;   // true ise sıradaki rakam ekranı temizler

    // Ondalık ayırıcı virgül olsun diye Türkçe kültür (3,5 gibi)
    static readonly CultureInfo tr = new CultureInfo("tr-TR");

    public StandartPage()
    {
        InitializeComponent();
    }

    // ---------- Sayı girişi ----------

    // Rakam tuşlarının ortak işleyicisi
    private void Rakam_Clicked(object sender, EventArgs e)
    {
        string rakam = ((Button)sender).Text;

        if (lblEkran.Text == "0" || yeniSayiBasliyor)
        {
            lblEkran.Text = rakam;       // ekranı temizleyip yeni sayıya başla
            yeniSayiBasliyor = false;
        }
        else
        {
            lblEkran.Text += rakam;      // sayının sonuna ekle
        }
    }

    // Ondalık virgül: yeni sayıda "0," ile başlar, ikinci virgül eklenmez
    private void Virgul_Clicked(object sender, EventArgs e)
    {
        if (yeniSayiBasliyor)
        {
            lblEkran.Text = "0,";
            yeniSayiBasliyor = false;
        }
        else if (!lblEkran.Text.Contains(','))
        {
            lblEkran.Text += ",";
        }
    }

    // ⌫: son karakteri siler; hiçbir şey kalmazsa 0
    private void Sil_Clicked(object sender, EventArgs e)
    {
        if (yeniSayiBasliyor)
            return;                      // sonuç ekrandayken silme yapılmaz

        string metin = lblEkran.Text.Substring(0, lblEkran.Text.Length - 1);
        lblEkran.Text = (metin == "" || metin == "-") ? "0" : metin;
    }

    // ---------- Temizleme ----------

    // C: her şeyi sıfırla
    private void Temizle_Clicked(object sender, EventArgs e)
    {
        ilkSayi = 0;
        bekleyenIslem = "";
        yeniSayiBasliyor = false;
        lblEkran.Text = "0";
    }

    // CE: yalnız ekrandaki sayıyı sil, bekleyen işlem kalsın
    private void EkranTemizle_Clicked(object sender, EventArgs e)
    {
        lblEkran.Text = "0";
        yeniSayiBasliyor = false;
    }

    // ---------- İşlemler ----------

    // + − × ÷ tuşlarının ortak işleyicisi
    private void Islem_Clicked(object sender, EventArgs e)
    {
        // 2 + 3 + yazıldıysa ikinci + basılınca önce 2 + 3'ü hesapla
        if (bekleyenIslem != "" && !yeniSayiBasliyor)
            Hesapla();

        ilkSayi = EkrandakiSayi();
        bekleyenIslem = ((Button)sender).Text;
        yeniSayiBasliyor = true;
    }

    private void Esittir_Clicked(object sender, EventArgs e)
    {
        if (bekleyenIslem == "")
            return;                      // bekleyen işlem yoksa yapacak bir şey yok

        Hesapla();
        bekleyenIslem = "";
        yeniSayiBasliyor = true;
    }

    // Tek sayıyla çalışan tuşlar: +/−, %, 1/x, x², √x
    private void TekSayi_Clicked(object sender, EventArgs e)
    {
        double x = EkrandakiSayi();

        switch (((Button)sender).Text)
        {
            case "+/−":
                SonucuYaz(-x);
                return;                  // işaret değişince sayı girişi sürebilir

            case "%":
                // Toplama/çıkarmada ilk sayının yüzdesi (200 + 10% → 20), diğerlerinde x/100
                if (bekleyenIslem == "+" || bekleyenIslem == "−")
                    SonucuYaz(ilkSayi * x / 100);
                else
                    SonucuYaz(x / 100);
                break;

            case "1/x":
                if (x == 0) { HataGoster("Sıfıra bölünemez"); return; }
                SonucuYaz(1 / x);
                break;

            case "x²":
                SonucuYaz(x * x);
                break;

            case "√x":
                if (x < 0) { HataGoster("Geçersiz giriş"); return; }
                SonucuYaz(Math.Sqrt(x));
                break;
        }

        yeniSayiBasliyor = true;         // sonuçtan sonra yazılan rakam yeni sayı başlatır
    }

    // ilkSayi ile ekrandaki sayıyı bekleyen işlemle hesaplar, sonucu ekrana yazar
    private void Hesapla()
    {
        double ikinciSayi = EkrandakiSayi();

        switch (bekleyenIslem)
        {
            case "+": SonucuYaz(ilkSayi + ikinciSayi); break;
            case "−": SonucuYaz(ilkSayi - ikinciSayi); break;
            case "×": SonucuYaz(ilkSayi * ikinciSayi); break;
            case "÷":
                if (ikinciSayi == 0) { HataGoster("Sıfıra bölünemez"); return; }
                SonucuYaz(ilkSayi / ikinciSayi);
                break;
        }
    }

    // ---------- Yardımcılar ----------

    // Ekrandaki yazıyı sayıya çevirir; sayı değilse (ör. hata mesajı) 0 döner
    private double EkrandakiSayi()
    {
        if (double.TryParse(lblEkran.Text, NumberStyles.Float, tr, out double sayi))
            return sayi;
        return 0;
    }

    // Sonucu ekrana yazar; 0,1 + 0,2 gibi durumlarda 0,30000000000000004 yerine 0,3 görünsün diye 12 basamağa yuvarlar
    private void SonucuYaz(double sonuc)
    {
        if (double.IsNaN(sonuc) || double.IsInfinity(sonuc))
        {
            HataGoster("Geçersiz giriş");   // taşma ya da tanımsız sonuç
            return;
        }
        lblEkran.Text = sonuc.ToString("G12", tr);
    }

    // Hata mesajını gösterir ve hesap durumunu sıfırlar; sıradaki rakam yeni hesap başlatır
    private void HataGoster(string mesaj)
    {
        lblEkran.Text = mesaj;
        ilkSayi = 0;
        bekleyenIslem = "";
        yeniSayiBasliyor = true;
    }
}