using System.Globalization;

namespace HesapMakinesi;

// Hesap makinesinin bütün mantığı burada.
// Standart ve Bilimsel sayfa aynı sınıfı kullanır; her sayfa kendi ekranını (Label) verir,
// kendi HesapMotoru nesnesini oluşturur. Böylece hesap kodu tek yerde durur.
public class HesapMotoru
{
    readonly Label ekran;            // sonucun yazıldığı Label

    // Hesap durumu: tuşlar arasında taşınan bilgiler
    double ilkSayi = 0;              // işlem tuşundan önce girilen sayı
    string bekleyenIslem = "";       // "+", "−", "×", "÷", "mod", "xʸ" ya da boş
    bool yeniSayiBasliyor = false;   // true ise sıradaki rakam ekranı temizler
    bool ikinciSayiVar = false;      // işlem tuşundan sonra yeni bir sayı girildi mi

    // Ondalık ayırıcı virgül olsun diye Türkçe kültür (3,5 gibi)
    static readonly CultureInfo tr = new CultureInfo("tr-TR");

    public HesapMotoru(Label ekran)
    {
        this.ekran = ekran;
    }

    // ---------- Sayı girişi ----------

    public void Rakam(string rakam)
    {
        if (ekran.Text == "0" || yeniSayiBasliyor)
        {
            ekran.Text = rakam;          // ekranı temizleyip yeni sayıya başla
            yeniSayiBasliyor = false;
        }
        else
        {
            ekran.Text += rakam;         // sayının sonuna ekle
        }
        ikinciSayiVar = true;
    }

    // Ondalık virgül: yeni sayıda "0," ile başlar, ikinci virgül eklenmez
    public void Virgul()
    {
        if (yeniSayiBasliyor)
        {
            ekran.Text = "0,";
            yeniSayiBasliyor = false;
        }
        else if (!ekran.Text.Contains(','))
        {
            ekran.Text += ",";
        }
        ikinciSayiVar = true;
    }

    // π ve e gibi sabitleri ekrana yazar
    public void Sabit(double deger)
    {
        SonucuYaz(deger);
        yeniSayiBasliyor = true;
        ikinciSayiVar = true;
    }

    // ⌫: son karakteri siler; hiçbir şey kalmazsa 0
    public void Sil()
    {
        if (yeniSayiBasliyor)
            return;                      // sonuç ekrandayken silme yapılmaz

        string metin = ekran.Text.Substring(0, ekran.Text.Length - 1);
        ekran.Text = (metin == "" || metin == "-") ? "0" : metin;
    }

    // ---------- Temizleme ----------

    // C: her şeyi sıfırla
    public void Temizle()
    {
        ilkSayi = 0;
        bekleyenIslem = "";
        yeniSayiBasliyor = false;
        ikinciSayiVar = false;
        ekran.Text = "0";
    }

    // CE: yalnız ekrandaki sayıyı sil, bekleyen işlem kalsın
    public void EkranTemizle()
    {
        ekran.Text = "0";
        yeniSayiBasliyor = false;
    }

    // ---------- İki sayılı işlemler ----------

    // + − × ÷ mod xʸ
    public void Islem(string islem)
    {
        // 2 + 3 × yazıldıysa × basılınca önce 2 + 3'ü hesapla
        if (bekleyenIslem != "" && ikinciSayiVar)
            Hesapla();

        ilkSayi = EkrandakiSayi();
        bekleyenIslem = islem;
        yeniSayiBasliyor = true;
        ikinciSayiVar = false;
    }

    public void Esittir()
    {
        if (bekleyenIslem == "")
            return;                      // bekleyen işlem yoksa yapacak bir şey yok

        Hesapla();
        bekleyenIslem = "";
        yeniSayiBasliyor = true;
        ikinciSayiVar = false;
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
            case "mod":
                if (ikinciSayi == 0) { HataGoster("Sıfıra bölünemez"); return; }
                SonucuYaz(ilkSayi % ikinciSayi);
                break;
            case "xʸ":
                SonucuYaz(Math.Pow(ilkSayi, ikinciSayi));   // (−8)^0,5 gibi tanımsızlar SonucuYaz'da yakalanır
                break;
        }
    }

    // ---------- Tek sayılı işlemler ----------

    // Ekrandaki sayıya uygulanan işlemler (Standart ve Bilimsel tuşların hepsi)
    public void TekSayi(string tus)
    {
        double x = EkrandakiSayi();
        double radyan = x * Math.PI / 180;   // trigonometri derece cinsinden (Windows'taki DEG)

        switch (tus)
        {
            case "+/−":
                SonucuYaz(-x);
                ikinciSayiVar = true;
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

            case "|x|":
                SonucuYaz(Math.Abs(x));
                break;

            case "n!":
                // Faktöriyel yalnız 0–170 arası tam sayılarda (171! double'a sığmaz)
                if (x < 0 || x > 170 || x != Math.Floor(x)) { HataGoster("Geçersiz giriş"); return; }
                double faktoriyel = 1;
                for (int i = 2; i <= (int)x; i++)
                    faktoriyel *= i;
                SonucuYaz(faktoriyel);
                break;

            case "10ˣ":
                SonucuYaz(Math.Pow(10, x));
                break;

            case "log":
                if (x <= 0) { HataGoster("Geçersiz giriş"); return; }
                SonucuYaz(Math.Log10(x));
                break;

            case "ln":
                if (x <= 0) { HataGoster("Geçersiz giriş"); return; }
                SonucuYaz(Math.Log(x));
                break;

            // sin 180 gibi değerler 0,000000000000000122 çıkmasın diye 10 basamağa yuvarlanır
            case "sin":
                SonucuYaz(Math.Round(Math.Sin(radyan), 10));
                break;

            case "cos":
                SonucuYaz(Math.Round(Math.Cos(radyan), 10));
                break;

            case "tan":
                // tan 90, tan 270 tanımsız: cos 0 olduğu yerler
                if (Math.Abs(Math.Cos(radyan)) < 1e-12) { HataGoster("Geçersiz giriş"); return; }
                SonucuYaz(Math.Round(Math.Tan(radyan), 10));
                break;
        }

        yeniSayiBasliyor = true;         // sonuçtan sonra yazılan rakam yeni sayı başlatır
        ikinciSayiVar = true;
    }

    // ---------- Yardımcılar ----------

    // Ekrandaki yazıyı sayıya çevirir; sayı değilse (ör. hata mesajı) 0 döner
    private double EkrandakiSayi()
    {
        if (double.TryParse(ekran.Text, NumberStyles.Float, tr, out double sayi))
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
        ekran.Text = sonuc.ToString("G12", tr);
    }

    // Hata mesajını gösterir ve hesap durumunu sıfırlar; sıradaki rakam yeni hesap başlatır
    private void HataGoster(string mesaj)
    {
        ekran.Text = mesaj;
        ilkSayi = 0;
        bekleyenIslem = "";
        yeniSayiBasliyor = true;
        ikinciSayiVar = false;
    }
}