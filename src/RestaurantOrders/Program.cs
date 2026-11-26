// Bu dosya, kullanıcının restoran işlemlerini seçtiği konsol menüsünü içerir.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using RestaurantOrders.Models;
using RestaurantOrders.Services;
using RestaurantOrders.Tests;

namespace RestaurantOrders
{
    // Konsol menüsünü çalıştıran giriş sınıfı
    public class Program
    {
        // Tüm menü işlemlerinin kullandığı tek restoran nesnesi
        private static Restaurant restaurant = new Restaurant();

        // Girdi kapanırsa menünün sonsuz döngüye girmemesi için tutulur
        private static bool endOfInput = false;

        // Örnek veriyi yükler ve menüyü kullanıcı çıkana kadar gösterir
        public static void Main()
        {
            // Türkçe karakterlerin bozulmaması için çıkış kodlaması UTF-8 yapılır
            Console.OutputEncoding = Encoding.UTF8;

            // Giriş kodlamasına dokunulmaz; Windows konsolunda seçim yazılabilsin diye varsayılan bırakılır

            // Kurgusal menü ve müşteriler belleğe yüklenir
            restaurant.LoadSampleData();

            // Kullanıcıya başlangıç bilgisi yazılır
            Console.WriteLine("Örnek menü ve müşteriler yüklendi.");
            Console.WriteLine("Tüm seçimler numarayla yapılır. Listeden numarayı yazıp Enter'a basın.");

            // Menü döngüsü açık başlar
            bool running = true;

            // Kullanıcı 0 seçene kadar menü tekrar eder
            while (running)
            {
                // Dosya sonuna gelindiyse döngü biter
                if (endOfInput)
                {
                    break;
                }

                // Ana menü seçenekleri yazılır
                ShowMainMenu();

                // Kullanıcıdan ana menü numarası alınır
                int choice = ReadInt("Seçiminiz: ");

                // Seçim okunamadan girdi kapandıysa çıkılır
                if (endOfInput)
                {
                    break;
                }

                // Seçime göre ilgili işlemi çalıştırıyoruz
                switch (choice)
                {
                    case 1:
                        ShowMenuItems();
                        break;
                    case 2:
                        AddMenuItemFromUser();
                        break;
                    case 3:
                        UpdatePriceFromUser();
                        break;
                    case 4:
                        RemoveMenuItemFromUser();
                        break;
                    case 5:
                        ManageCustomers();
                        break;
                    case 6:
                        CreateOrderFromUser();
                        break;
                    case 7:
                        AddItemToOrderFromUser();
                        break;
                    case 8:
                        EditOrderLineFromUser();
                        break;
                    case 9:
                        ApplyDiscountFromUser();
                        break;
                    case 10:
                        ChangeStatusFromUser();
                        break;
                    case 11:
                        PrintReceiptFromUser();
                        break;
                    case 12:
                        ShowOrders();
                        break;
                    case 13:
                        RunTests();
                        break;
                    case 0:
                        running = false;
                        WriteSuccess("Güle güle.");
                        break;
                    default:
                        WriteError("Geçersiz seçim. 0 ile 13 arasında bir sayı girin.");
                        break;
                }

                // Çıkış dışında kullanıcı sonucu okuyabilsin diye beklenir
                if (running)
                {
                    WaitForEnter();
                }
            }
        }

        // Ana menü metnini ekrana yazar
        private static void ShowMainMenu()
        {
            Console.WriteLine();
            Console.WriteLine("===== RESTAURANT ORDERS =====");
            Console.WriteLine("1. Menüyü listele");
            Console.WriteLine("2. Menüye ürün ekle (Food / Drink)");
            Console.WriteLine("3. Ürün fiyatını güncelle");
            Console.WriteLine("4. Menüden ürün sil");
            Console.WriteLine("5. Müşteri ekle / listele");
            Console.WriteLine("6. Yeni sipariş oluştur");
            Console.WriteLine("7. Siparişe ürün ekle");
            Console.WriteLine("8. Siparişteki ürün miktarını değiştir / ürünü çıkar");
            Console.WriteLine("9. İndirim uygula");
            Console.WriteLine("10. Sipariş durumunu güncelle");
            Console.WriteLine("11. Fiş yazdır");
            Console.WriteLine("12. Tüm siparişleri listele");
            Console.WriteLine("13. Otomatik testleri çalıştır");
            Console.WriteLine("0. Çıkış");
        }

        // Menüdeki ürünleri numaralı ve polimorfik metinle listeler
        private static void ShowMenuItems()
        {
            IReadOnlyList<MenuItem> menu = restaurant.GetMenu();

            // Boş menüde ürün satırı yazılmaz
            if (menu.Count == 0)
            {
                WriteError("Menü boş.");
                return;
            }

            Console.WriteLine("--- MENÜ ---");
            Console.WriteLine("Seçmek için soldaki numarayı yazın.");

            // Aynı listede Food ve Drink kendi GetDisplayText metnini üretir
            for (int i = 0; i < menu.Count; i = i + 1)
            {
                // Kullanıcı 1'den başlayan numarayla seçer
                IPrintable printable = menu[i];
                Console.WriteLine((i + 1) + ". " + printable.GetDisplayText());
            }
        }

        // Kullanıcıdan yemek veya içecek bilgisi alıp menüye ekler
        private static void AddMenuItemFromUser()
        {
            Console.WriteLine("1. Food (Yemek)");
            Console.WriteLine("2. Drink (İçecek)");
            int type = ReadInt("Ürün türü numarası: ");

            // Yalnızca iki ürün türü vardır
            if (type != 1 && type != 2)
            {
                WriteError("Ürün türü 1 veya 2 olmalıdır.");
                return;
            }

            // Kimlik otomatik üretilir; kullanıcı F01 yazmak zorunda kalmaz
            string id;

            // 1 yemek, 2 içecek kimliği üretir
            if (type == 1)
            {
                id = CreateNextId("F");
            }
            else
            {
                id = CreateNextId("D");
            }

            // Kullanıcıya verilen kimlik gösterilir
            Console.WriteLine("Otomatik kimlik: " + id);

            // Ürün adı kullanıcıdan alınır
            string name = ReadText("Ürün adı: ");

            // Fiyat kullanıcıdan alınır
            decimal price = ReadDecimal("Fiyat: ");

            // Sıfır ve negatif fiyat daha nesne kurulmadan reddedilir
            if (price <= 0)
            {
                WriteError("Fiyat 0'dan büyük olmalıdır.");
                return;
            }

            try
            {
                MenuItem item;

                // Türe göre Food veya Drink nesnesi kurulur
                if (type == 1)
                {
                    // Vejetaryen sorusu da numarayla sorulur
                    bool isVegetarian = ReadYesNo("Vejetaryen mi?");
                    item = new Food(id, name, price, isVegetarian);
                }
                else
                {
                    // İçecek hacmi sorulur
                    int volume = ReadInt("Hacim (ml): ");

                    // Hacim pozitif değilse içecek oluşturulmaz
                    if (volume <= 0)
                    {
                        WriteError("Hacim 0'dan büyük olmalıdır.");
                        return;
                    }

                    item = new Drink(id, name, price, volume);
                }

                string message;
                bool added = restaurant.AddMenuItem(item, out message);
                ShowOperationResult(added, message);
            }
            // Constructor geçersiz değer verirse program çökmez, hata yazılır
            catch (ArgumentException ex)
            {
                WriteError(ex.Message);
            }
        }

        // Seçilen ürünün fiyatını günceller
        private static void UpdatePriceFromUser()
        {
            MenuItem? item;

            // Ürün numarayla seçilmezse işlem yapılmaz
            if (TrySelectMenuItem("Güncellenecek ürün numarası: ", out item) == false || item == null)
            {
                return;
            }

            // Yeni fiyat okunur
            decimal price = ReadDecimal("Yeni fiyat: ");
            string message;
            bool updated = restaurant.UpdateMenuItemPrice(item.Id, price, out message);
            ShowOperationResult(updated, message);
        }

        // Seçilen ürünü menüden siler
        private static void RemoveMenuItemFromUser()
        {
            MenuItem? item;

            // Ürün numarayla seçilmezse silme yapılmaz
            if (TrySelectMenuItem("Silinecek ürün numarası: ", out item) == false || item == null)
            {
                return;
            }

            string message;
            bool removed = restaurant.RemoveMenuItem(item.Id, out message);
            ShowOperationResult(removed, message);
        }

        // Müşteri listeleme ve ekleme alt menüsünü açar
        private static void ManageCustomers()
        {
            Console.WriteLine("1. Müşterileri listele");
            Console.WriteLine("2. Müşteri ekle");
            Console.WriteLine("0. Geri");
            int choice = ReadInt("Seçiminiz: ");

            // 1 müşterileri listeler
            if (choice == 1)
            {
                ShowCustomers();
            }
            // 2 yeni müşteri ekleme ekranını açar
            else if (choice == 2)
            {
                AddCustomerFromUser();
            }
            // 0 alt menüden ana menüye döner
            else if (choice == 0)
            {
                return;
            }
            // Diğer numaralar geçersizdir
            else
            {
                WriteError("Geçersiz seçim.");
            }
        }

        // Kayıtlı müşterileri numaralı listeler
        private static void ShowCustomers()
        {
            IReadOnlyList<Customer> customers = restaurant.GetCustomers();

            // Kayıt yoksa boş liste mesajı verilir
            if (customers.Count == 0)
            {
                WriteError("Kayıtlı müşteri yok.");
                return;
            }

            Console.WriteLine("--- MÜŞTERİLER ---");
            Console.WriteLine("Seçmek için soldaki numarayı yazın.");

            // Her müşteri 1'den başlayan numarayla yazılır
            for (int i = 0; i < customers.Count; i = i + 1)
            {
                Customer customer = customers[i];
                Console.WriteLine((i + 1) + ". " + customer.Id + " | " + customer.FullName + " | " + customer.Phone);
            }
        }

        // Yeni müşteri bilgilerini okuyup listeye ekler
        private static void AddCustomerFromUser()
        {
            // Müşteri kimliği otomatik üretilir
            string id = CreateNextId("C");
            Console.WriteLine("Otomatik kimlik: " + id);

            // Ad soyad kullanıcıdan alınır
            string fullName = ReadText("Ad soyad: ");

            // Telefon kullanıcıdan alınır
            string phone = ReadText("Telefon: ");

            try
            {
                Customer customer = new Customer(id, fullName, phone);
                string message;
                bool added = restaurant.AddCustomer(customer, out message);
                ShowOperationResult(added, message);
            }
            // Constructor geçersiz müşteri bilgisini reddederse mesaj gösterilir
            catch (ArgumentException ex)
            {
                WriteError(ex.Message);
            }
        }

        // Seçilen müşteri için yeni sipariş açar
        private static void CreateOrderFromUser()
        {
            Customer? customer;

            // Müşteri numarayla seçilmezse sipariş açılmaz
            if (TrySelectCustomer("Müşteri numarası: ", out customer) == false || customer == null)
            {
                return;
            }

            Order? order;
            string message;
            bool created = restaurant.CreateOrder(customer.Id, out order, out message);

            // Başarılı oluşturmada sipariş numarası da gösterilir
            if (created && order != null)
            {
                WriteSuccess(message + " Sipariş no: " + order.Id);
            }
            else
            {
                WriteError(message);
            }
        }

        // Seçilen siparişe menüden ürün ekler
        private static void AddItemToOrderFromUser()
        {
            Order? order;

            // Önce sipariş numarayla seçilir
            if (TrySelectOrder("Sipariş numarası: ", out order) == false || order == null)
            {
                return;
            }

            MenuItem? item;

            // Sonra menü ürünü numarayla seçilir
            if (TrySelectMenuItem("Ürün numarası: ", out item) == false || item == null)
            {
                return;
            }

            // Miktar numerik olarak alınır
            int quantity = ReadInt("Miktar (1-20): ");
            string message;
            bool added = restaurant.AddItemToOrder(order.Id, item.Id, quantity, out message);
            ShowOperationResult(added, message);
        }

        // Sipariş satırının miktarını değiştirir veya satırı siler
        private static void EditOrderLineFromUser()
        {
            Console.WriteLine("1. Miktarı değiştir");
            Console.WriteLine("2. Ürünü çıkar");
            int choice = ReadInt("Seçiminiz: ");

            // Bu alt menüde yalnızca iki işlem vardır
            if (choice != 1 && choice != 2)
            {
                WriteError("Seçim 1 veya 2 olmalıdır.");
                return;
            }

            Order? order;

            // Sipariş numarayla seçilir
            if (TrySelectOrder("Sipariş numarası: ", out order) == false || order == null)
            {
                return;
            }

            OrderLine? line;

            // Sipariş satırı numarayla seçilir
            if (TrySelectOrderLine(order, "Sipariş satırı numarası: ", out line) == false || line == null)
            {
                return;
            }

            string message;
            bool ok;

            // 1 miktar günceller, 2 ürünü siparişten çıkarır
            if (choice == 1)
            {
                int quantity = ReadInt("Yeni miktar (1-20): ");
                ok = restaurant.UpdateOrderItemQuantity(order.Id, line.Item.Id, quantity, out message);
            }
            else
            {
                ok = restaurant.RemoveItemFromOrder(order.Id, line.Item.Id, out message);
            }

            ShowOperationResult(ok, message);
        }

        // Seçilen siparişe indirim oranı uygular
        private static void ApplyDiscountFromUser()
        {
            Order? order;

            // Sipariş numarayla seçilir
            if (TrySelectOrder("Sipariş numarası: ", out order) == false || order == null)
            {
                return;
            }

            decimal percent = ReadDecimal("İndirim oranı (0-50): ");
            string message;
            bool applied = restaurant.ApplyDiscountToOrder(order.Id, percent, out message);
            ShowOperationResult(applied, message);
        }

        // Seçilen siparişin durumunu güncellemeyi dener
        private static void ChangeStatusFromUser()
        {
            Order? order;

            // Sipariş numarayla seçilir
            if (TrySelectOrder("Sipariş numarası: ", out order) == false || order == null)
            {
                return;
            }

            Console.WriteLine("1. Preparing");
            Console.WriteLine("2. Served");
            Console.WriteLine("3. Paid");
            Console.WriteLine("4. Cancelled");
            int choice = ReadInt("Yeni durum numarası: ");
            OrderStatus newStatus;

            // 1 hazırlık durumunu seçer
            if (choice == 1)
            {
                newStatus = OrderStatus.Preparing;
            }
            // 2 servis durumunu seçer
            else if (choice == 2)
            {
                newStatus = OrderStatus.Served;
            }
            // 3 ödeme durumunu seçer
            else if (choice == 3)
            {
                newStatus = OrderStatus.Paid;
            }
            // 4 iptal durumunu seçer
            else if (choice == 4)
            {
                newStatus = OrderStatus.Cancelled;
            }
            // Listede olmayan numara durum değiştirmez
            else
            {
                WriteError("Geçersiz durum seçimi.");
                return;
            }

            string message;
            bool changed = restaurant.ChangeOrderStatus(order.Id, newStatus, out message);
            ShowOperationResult(changed, message);
        }

        // Seçilen siparişin fişini yazar
        private static void PrintReceiptFromUser()
        {
            Order? order;

            // Sipariş numarayla seçilir
            if (TrySelectOrder("Fişi yazdırılacak sipariş numarası: ", out order) == false || order == null)
            {
                return;
            }

            // Fiş metni konsola yazılır
            Console.WriteLine(order.GetReceipt());
        }

        // Tüm siparişleri numaralı kısa özetle listeler
        private static void ShowOrders()
        {
            IReadOnlyList<Order> orders = restaurant.GetOrders();

            // Henüz sipariş yoksa liste boş bırakılır
            if (orders.Count == 0)
            {
                Console.WriteLine("Henüz sipariş yok.");
                return;
            }

            Console.WriteLine("--- SİPARİŞLER ---");
            Console.WriteLine("Seçmek için soldaki numarayı yazın.");

            // Her sipariş IPrintable üzerinden kendi özetini yazar
            for (int i = 0; i < orders.Count; i = i + 1)
            {
                IPrintable printable = orders[i];
                Console.WriteLine((i + 1) + ". " + printable.GetDisplayText());
            }
        }

        // Menüden numarayla tek ürün seçer
        private static bool TrySelectMenuItem(string prompt, out MenuItem? item)
        {
            item = null;
            IReadOnlyList<MenuItem> menu = restaurant.GetMenu();

            // Menü boşsa seçim yapılamaz
            if (menu.Count == 0)
            {
                WriteError("Menü boş.");
                return false;
            }

            // Numaralı menü gösterilir
            ShowMenuItems();

            // Kullanıcıdan seçim numarası alınır
            int index = ReadInt(prompt);

            // Girdi kapandıysa seçim başarısızdır
            if (endOfInput)
            {
                return false;
            }

            // Numara 1 ile liste boyutu arasında olmalıdır
            if (index < 1 || index > menu.Count)
            {
                WriteError("Geçersiz ürün numarası. 1 ile " + menu.Count + " arasında seçin.");
                return false;
            }

            // Liste 0'dan başladığı için 1 çıkarılır
            item = menu[index - 1];
            return true;
        }

        // Müşteri listesinden numarayla tek müşteri seçer
        private static bool TrySelectCustomer(string prompt, out Customer? customer)
        {
            customer = null;
            IReadOnlyList<Customer> customers = restaurant.GetCustomers();

            // Müşteri yoksa seçim yapılamaz
            if (customers.Count == 0)
            {
                WriteError("Kayıtlı müşteri yok.");
                return false;
            }

            // Numaralı müşteri listesi gösterilir
            ShowCustomers();

            // Kullanıcıdan seçim numarası alınır
            int index = ReadInt(prompt);

            // Girdi kapandıysa seçim başarısızdır
            if (endOfInput)
            {
                return false;
            }

            // Numara geçerli aralıkta değilse reddedilir
            if (index < 1 || index > customers.Count)
            {
                WriteError("Geçersiz müşteri numarası. 1 ile " + customers.Count + " arasında seçin.");
                return false;
            }

            // Seçilen müşteri döndürülür
            customer = customers[index - 1];
            return true;
        }

        // Sipariş listesinden numarayla tek sipariş seçer
        private static bool TrySelectOrder(string prompt, out Order? order)
        {
            order = null;
            IReadOnlyList<Order> orders = restaurant.GetOrders();

            // Sipariş yoksa seçim yapılamaz
            if (orders.Count == 0)
            {
                WriteError("Henüz sipariş yok.");
                return false;
            }

            // Numaralı sipariş listesi gösterilir
            ShowOrders();

            // Kullanıcıdan seçim numarası alınır
            int index = ReadInt(prompt);

            // Girdi kapandıysa seçim başarısızdır
            if (endOfInput)
            {
                return false;
            }

            // Numara geçerli aralıkta değilse reddedilir
            if (index < 1 || index > orders.Count)
            {
                WriteError("Geçersiz sipariş numarası. 1 ile " + orders.Count + " arasında seçin.");
                return false;
            }

            // Seçilen sipariş döndürülür
            order = orders[index - 1];
            return true;
        }

        // Sipariş satırlarından numarayla tek satır seçer
        private static bool TrySelectOrderLine(Order order, string prompt, out OrderLine? line)
        {
            line = null;
            IReadOnlyList<OrderLine> lines = order.Lines;

            // Satır yoksa seçim yapılamaz
            if (lines.Count == 0)
            {
                WriteError("Bu siparişte ürün yok.");
                return false;
            }

            Console.WriteLine("--- SİPARİŞ SATIRLARI ---");
            Console.WriteLine("Seçmek için soldaki numarayı yazın.");

            // Her satır numaralı yazılır
            for (int i = 0; i < lines.Count; i = i + 1)
            {
                OrderLine current = lines[i];
                Console.WriteLine((i + 1) + ". " + current.Item.Id + " | " + current.Item.Name + " | Adet: " + current.Quantity);
            }

            // Kullanıcıdan satır numarası alınır
            int index = ReadInt(prompt);

            // Girdi kapandıysa seçim başarısızdır
            if (endOfInput)
            {
                return false;
            }

            // Numara geçerli aralıkta değilse reddedilir
            if (index < 1 || index > lines.Count)
            {
                WriteError("Geçersiz satır numarası. 1 ile " + lines.Count + " arasında seçin.");
                return false;
            }

            // Seçilen satır döndürülür
            line = lines[index - 1];
            return true;
        }

        // Verilen önek için bir sonraki kimliği üretir (F04, D04, C04)
        private static string CreateNextId(string prefix)
        {
            // En büyük numara sıfırdan başlar
            int maxNumber = 0;

            // Menü ürünlerinde aynı önek aranır
            IReadOnlyList<MenuItem> menu = restaurant.GetMenu();
            for (int i = 0; i < menu.Count; i = i + 1)
            {
                maxNumber = MaxIdNumber(menu[i].Id, prefix, maxNumber);
            }

            // Müşterilerde aynı önek aranır
            IReadOnlyList<Customer> customers = restaurant.GetCustomers();
            for (int i = 0; i < customers.Count; i = i + 1)
            {
                maxNumber = MaxIdNumber(customers[i].Id, prefix, maxNumber);
            }

            // Bir sonraki numara iki haneli yazılır
            int next = maxNumber + 1;
            return prefix + next.ToString("00");
        }

        // Kimlik önekine ait numarayı okuyup en büyüğü günceller
        private static int MaxIdNumber(string id, string prefix, int currentMax)
        {
            // Önek uyuşmuyorsa mevcut en büyük korunur
            if (id.Length <= prefix.Length)
            {
                return currentMax;
            }

            // Büyük-küçük harf farkı öneği bozmaz
            if (string.Equals(id.Substring(0, prefix.Length), prefix, StringComparison.OrdinalIgnoreCase) == false)
            {
                return currentMax;
            }

            // Önekten sonraki kısım sayıya çevrilir
            string digits = id.Substring(prefix.Length);
            int value;

            // Sayı değilse mevcut en büyük korunur
            if (int.TryParse(digits, out value) == false)
            {
                return currentMax;
            }

            // Daha büyük numara bulunduysa o kullanılır
            if (value > currentMax)
            {
                return value;
            }

            return currentMax;
        }

        // Otomatik testleri çalıştırır
        private static void RunTests()
        {
            TestRunner runner = new TestRunner();
            runner.RunAll();
        }

        // İşlem sonucunu renkle yazar
        private static void ShowOperationResult(bool success, string message)
        {
            // Başarılı işlem yeşil yazılır
            if (success)
            {
                WriteSuccess(message);
            }
            // Başarısız işlem kırmızı yazılır
            else
            {
                WriteError(message);
            }
        }

        // Kullanıcıdan tam sayı okur; hatalı girişte yeniden sorar
        private static int ReadInt(string prompt)
        {
            // Geçerli bir tam sayı girilene kadar soru tekrarlanır
            while (true)
            {
                // Girdi kapandıysa 0 dönüp çağıranın ilerlemesine izin veriyoruz
                if (endOfInput)
                {
                    return 0;
                }

                Console.Write(prompt);
                string? input = Console.ReadLine();

                // Dosya sonu menüyü kilitlemesin
                if (input == null)
                {
                    endOfInput = true;
                    return 0;
                }

                int value;

                // Sayıya çevrilebiliyorsa değeri döndürüyoruz
                if (int.TryParse(input.Trim(), out value))
                {
                    return value;
                }

                WriteError("Lütfen geçerli bir tam sayı girin.");
            }
        }

        // Kullanıcıdan ondalıklı sayı okur; virgül veya nokta kabul edilir
        private static decimal ReadDecimal(string prompt)
        {
            // Geçerli bir sayı girilene kadar soru tekrarlanır
            while (true)
            {
                // Girdi kapandıysa 0 dönülür
                if (endOfInput)
                {
                    return 0m;
                }

                Console.Write(prompt);
                string? input = Console.ReadLine();

                // Dosya sonu menüyü kilitlemesin
                if (input == null)
                {
                    endOfInput = true;
                    return 0m;
                }

                // Türkçe klavyede virgül, İngilizce klavyede nokta kullanılabilir
                string normalized = input.Trim().Replace(',', '.');
                decimal value;

                // Sayıya çevrilebiliyorsa değeri döndürüyoruz
                if (decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                {
                    return value;
                }

                WriteError("Lütfen geçerli bir sayı girin.");
            }
        }

        // Kullanıcıdan boş olmayan metin okur
        private static string ReadText(string prompt)
        {
            // Boş olmayan bir metin girilene kadar soru tekrarlanır
            while (true)
            {
                // Girdi kapandıysa boş metin döndürülür
                if (endOfInput)
                {
                    return "";
                }

                Console.Write(prompt);
                string? input = Console.ReadLine();

                // Dosya sonu menüyü kilitlemesin
                if (input == null)
                {
                    endOfInput = true;
                    return "";
                }

                string trimmed = input.Trim();

                // Boş metin kabul edilmez
                if (trimmed.Length > 0)
                {
                    return trimmed;
                }

                WriteError("Bu alan boş bırakılamaz.");
            }
        }

        // Evet veya hayır cevabını numarayla okur
        private static bool ReadYesNo(string prompt)
        {
            // 1 veya 2 girilene kadar soru tekrarlanır
            while (true)
            {
                Console.WriteLine("1. Evet");
                Console.WriteLine("2. Hayır");
                int answer = ReadInt(prompt + " ");

                // Girdi kapandıysa hayır kabul edilir
                if (endOfInput)
                {
                    return false;
                }

                // 1 olumlu cevaptır
                if (answer == 1)
                {
                    return true;
                }

                // 2 olumsuz cevaptır
                if (answer == 2)
                {
                    return false;
                }

                WriteError("Lütfen 1 veya 2 girin.");
            }
        }

        // Başarı mesajını yeşil yazar
        private static void WriteSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        // Hata mesajını kırmızı yazar
        private static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        // Kullanıcı sonucu gördükten sonra menüye dönmek için Enter bekler
        private static void WaitForEnter()
        {
            // Girdi zaten kapandıysa beklemeyiz
            if (endOfInput)
            {
                return;
            }

            Console.WriteLine();
            Console.Write("Devam etmek için Enter tuşuna basın...");
            string? input = Console.ReadLine();

            // Dosya sonu bir sonraki menü turunu da bitirir
            if (input == null)
            {
                endOfInput = true;
            }
        }
    }
}
