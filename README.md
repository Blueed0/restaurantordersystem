# Restaurant Orders

Üniversite nesne yönelimli programlama ara sınav projesi. Konu: **Topic 14 – Restaurant Orders**. Uygulama, menü, müşteri ve sipariş işlemlerini bellek üzerinde tutan bir .NET 8 konsol programıdır. Veriler kurgusaldır. Harici NuGet paketi kullanılmaz.

## Authors

- Dilay Büyükbayram
- Muhammed Ali Batazun

## Gereksinimler

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Terminal / PowerShell / Komut İstemi (veya Visual Studio / VS Code / Cursor)

## Başka bilgisayarda çalıştırma

1. Bilgisayara **.NET 8 SDK** kurun.
2. Projeyi alın (`git clone` veya ZIP indirip açın).
3. Terminalde proje klasörüne girin.
4. Şu komutları çalıştırın:

```bash
cd src/RestaurantOrders
dotnet build
dotnet run
```

Windows’ta `dotnet run` engellenirse proje kökünden:

```powershell
.\baslat.cmd
```

Program açılışta örnek menüyü ve örnek müşterileri yükler. Listeler numaralıdır; seçim için numarayı yazıp Enter’a basın. Başarılı işlemler yeşil, hatalı işlemler kırmızı yazılır.

## Menü

1. Menüyü listele. Food ve Drink aynı listede, kendi metinleriyle yazılır.
2. Menüye yemek veya içecek ekle.
3. Ürün fiyatını güncelle. Yeni fiyat 0'dan büyük olmalıdır.
4. Menüden ürün sil.
5. Müşteri ekle veya müşterileri listele.
6. Kayıtlı bir müşteri için yeni sipariş aç. Numara otomatik verilir (`O001`, `O002`, ...).
7. Siparişe ürün ekle. Aynı ürün varsa miktar artar. Toplam 20'yi geçemez.
8. Sipariş satırının miktarını değiştir veya ürünü çıkar. Yalnızca `Created` durumunda.
9. İndirim uygula. Oran %0 ile %50 arasındadır.
10. Sipariş durumunu güncelle. İzin verilen geçişler: Created → Preparing, Created → Cancelled, Preparing → Served, Served → Paid.
11. Fiş yazdır.
12. Tüm siparişleri listele.
13. Otomatik testleri çalıştır.
0. Çıkış.

## Testler

Ana menüden **13** seçilir. Her test yeni bir `Restaurant` nesnesiyle çalışır. Konsolda test no, açıklama, girdi, beklenen sonuç, gerçek sonuç ve PASS/FAIL yazılır. Aynı metin, programın çalıştığı klasöre `test-results.txt` olarak kaydedilir.

`src/RestaurantOrders` klasöründen çalıştırıldıysa dosya o klasörde oluşur. Son satır `12 / 12 test geçti` olmalıdır. Ayrıntılı tablo `docs/TestCases.md` dosyasındadır.

## Klasör yapısı

```text
RestaurantOrders.sln
README.md
docs/
  UML.md
  TestCases.md
  Report.md
src/RestaurantOrders/
  RestaurantOrders.csproj
  Program.cs
  Models/
  Services/
  Tests/
```

## OOP kavramları

| Kavram | Nerede |
| --- | --- |
| Soyut sınıf | `Models/MenuItem.cs` |
| Kalıtım | `Models/Food.cs`, `Models/Drink.cs` |
| Arayüz | `Models/IPrintable.cs`; `MenuItem` ve `Order` uygular |
| Polimorfizm | `GetTaxRate`, `GetCategory`, `GetDisplayText`, `GetPriceWithTax` |
| Kompozisyon | `Models/Order.cs` içinde `OrderLine` listesi |
| Kapsülleme | Tüm model alanları `private set`; doğrulama constructor ve metotlarda |
| Enum | `Models/OrderStatus.cs` |
| Koleksiyon ve CRUD | `Services/Restaurant.cs` içinde `List<T>` |
| Konsol menüsü | `Program.cs` |
| Testler | `Tests/TestRunner.cs` |

Sınıf, metot ve değişken adları İngilizcedir. Kaynak dosyalardaki açıklama yorumları Türkçedir.
