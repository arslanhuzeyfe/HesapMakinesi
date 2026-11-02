# Hesap Makinesi (.NET MAUI)

BSM352 Görsel Programlama, Ödev 1. Windows Hesap Makinesi'ne benzeyen, .NET MAUI ile yazılmış
bir uygulama: Standart ve Bilimsel mod, Shell ile sayfa geçişi, ayrı kaynak sözlüğünde stiller ve
anında değişen, kalıcı Açık / Koyu / Sistem tema.

**Hazırlayan:** Huzeyfe Arslan · 24010310007

**Tanıtım videosu:** [OneDrive'da izle](https://ogrencibartinedu-my.sharepoint.com/:v:/g/personal/24010310007_ogrenci_bartin_edu_tr/IQDUsbt_VXzwT7RhUPSy_4L9AdpJE-IQAZ9jFRSMbaf5u9s?e=WKCvZ6)
(Bartın Üniversitesi hesabıyla açılır)

## Çalıştırma

1. Visual Studio'da ".NET Multi-platform App UI development" iş yükü kurulu olmalı
   (.NET 10 SDK).
2. Windows'ta Ayarlar → Sistem → Geliştiriciler için → **Geliştirici Modu** açık olmalı.
3. Visual Studio → *Clone a repository* → `https://github.com/arslanhuzeyfe/HesapMakinesi`
4. `HesapMakinesi/HesapMakinesi.csproj` projesini açın, hedef olarak **Windows Machine** seçip F5.

Proje Android'de de çalışır (`net10.0-android`).

## Özellikler

| Sayfa | İçerik |
| --- | --- |
| **Standart** | `+ − × ÷`, `%`, `1/x`, `x²`, `√x`, `+/−`, `CE`, `C`, `⌫`, ondalık virgül |
| **Bilimsel** | Standart'a ek olarak `sin cos tan` (derece), `log`, `ln`, `10ˣ`, `xʸ`, `n!`, `mod`, `\|x\|`, `π`, `e` |
| **Ayarlar** | Açık / Koyu / Sistem tema (RadioButton) |

Sayfalar arasında soldaki ☰ menüyle geçilir.

### Hata yönetimi

Uygulama hatalı girişte çökmez, ekranda mesaj gösterir; sıradaki tuşla yeni hesap başlar.

- Sıfıra bölme, `mod 0`, `1/0` → **Sıfıra bölünemez**
- Negatif sayının kökü, `log`/`ln` için sıfır ya da negatif sayı, `tan 90°`, negatif ya da kesirli
  sayının faktöriyeli, taşma (sonsuz/tanımsız sonuç) → **Geçersiz giriş**
- Ekrandaki yazı `double.TryParse` ile okunur; hata mesajı ekrandayken işleme basmak da çökmeye yol
  açmaz.

## Yapı

```
HesapMakinesi/
├── AppShell.xaml          Shell: Flyout menü, üç ShellContent (Standart, Bilimsel, Ayarlar)
├── App.xaml(.cs)          Kaynak sözlüklerinin birleştirilmesi; açılışta kayıtlı temanın yüklenmesi
├── HesapMotoru.cs         Bütün hesap mantığı (iki sayfa da kendi nesnesini kullanır)
├── StandartPage.xaml      6 × 4 Grid tuş takımı
├── BilimselPage.xaml      7 × 5 Grid tuş takımı
├── AyarlarPage.xaml(.cs)  Tema seçimi
└── Resources/Styles/
    ├── Colors.xaml        Renkler; her rengin Açık/Koyu çifti (ZeminAcik, ZeminKoyu, …)
    └── HesapStilleri.xaml Yazı boyutları ve uygulamanın stilleri
```

### Stiller

Sayfalarda sabit renk, yazı tipi ya da yazı boyutu yoktur; hepsi `HesapStilleri.xaml`'dan gelir.

- `TemelButon` (x:Key): bütün tuşların ortak özellikleri.
- **Örtük** `Button` stili: `BasedOn="{StaticResource TemelButon}"`; stil verilmeyen tuşlara
  (C, CE, sin, log…) kendiliğinden uygulanır.
- **Açık** stiller `RakamButonu`, `IslemButonu`, `EsittirButonu`: üçü de `TemelButon`'dan
  `BasedOn` ile türer, yalnız farklı olanı değiştirir.
- Renkler `AppThemeBinding` ile verilir; tema değişince bütün sayfalar yeniden başlatmadan yeni
  renge geçer.

### Tema

`AyarlarPage` seçimde `Application.Current.UserAppTheme` değerini değiştirir ve seçimi
`Preferences` ile saklar. `App` kurucusu açılışta kayıtlı temayı okuyup uygular, böylece uygulama
son seçilen temayla açılır. Windows'ta menü tuşu gibi WinUI parçaları sistem temasını izlediği için
`App.YerelTemayiEsitle` onları da uygulamanın temasına eşitler.
