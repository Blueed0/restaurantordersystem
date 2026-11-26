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
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            restaurant.LoadSampleData();
            Console.WriteLine("Örnek menü ve müşteriler yüklendi.");

            bool running = true;

            // Kullanıcı 0 seçene kadar menü tekrar eder
            while (running)
            {
                // Dosya sonuna gelindiyse döngü biter
                if (endOfInput)
                {
                    break;
                }

                ShowMainMenu();
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

        // Menüdeki ürünleri polimorfik metinle listeler
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

            // Aynı listede Food ve Drink kendi GetDisplayText metnini üretir
            foreach (MenuItem item in menu)
            {
                IPrintable printable = item;
                Console.WriteLine(printable.GetDisplayText());
            }
        }

        // Kullanıcıdan yemek veya içecek bilgisi alıp menüye ekler
        private static void AddMenuItemFromUser()
        {
            Console.WriteLine("1. Food (Yemek)");
            Console.WriteLine("2. Drink (İçecek)");
            int type = ReadInt("Ürün türü: ");

            // Yalnızca iki ürün türü vardır
            if (type != 1 && type != 2)
            {
                WriteError("Ürün türü 1 veya 2 olmalıdır.");
                return;
            }

            string id = ReadText("Ürün kimliği (örnek F04 veya D04): ");
            string name = ReadText("Ürün adı: ");
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
                    bool isVegetarian = ReadYesNo("Vejetaryen mi?");
                    item = new Food(id, name, price, isVegetarian);
                }
                else
                {
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
            ShowMenuItems();
            string id = ReadText("Güncellenecek ürün kimliği: ");
            decimal price = ReadDecimal("Yeni fiyat: ");
            string message;
            bool updated = restaurant.UpdateMenuItemPrice(id, price, out message);
            ShowOperationResult(updated, message);
        }

        // Seçilen ürünü menüden siler
        private static void RemoveMenuItemFromUser()
        {
            ShowMenuItems();
            string id = ReadText("Silinecek ürün kimliği: ");
            string message;
            bool removed = restaurant.RemoveMenuItem(id, out message);
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

        // Kayıtlı müşterileri yazar
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

            // Her müşterinin kimliği, adı ve telefonu yazılır
            foreach (Customer customer in customers)
            {
                Console.WriteLine(customer.Id + " | " + customer.FullName + " | " + customer.Phone);
            }
        }

        // Yeni müşteri bilgilerini okuyup listeye ekler
        private static void AddCustomerFromUser()
        {
            string id = ReadText("Müşteri kimliği (örnek C04): ");
            string fullName = ReadText("Ad soyad: ");
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
            ShowCustomers();
            string customerId = ReadText("Müşteri kimliği: ");
            Order? order;
            string message;
            bool created = restaurant.CreateOrder(customerId, out order, out message);

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
            ShowOrders();
            ShowMenuItems();
            string orderId = ReadText("Sipariş kimliği: ");
            string itemId = ReadText("Ürün kimliği: ");
            int quantity = ReadInt("Miktar (1-20): ");
            string message;
            bool added = restaurant.AddItemToOrder(orderId, itemId, quantity, out message);
            ShowOperationResult(added, message);
        }

        // Sipariş satırının miktarını değiştirir veya satırı siler
        private static void EditOrderLineFromUser()
        {
            ShowOrders();
            Console.WriteLine("1. Miktarı değiştir");
            Console.WriteLine("2. Ürünü çıkar");
            int choice = ReadInt("Seçiminiz: ");

            // Bu alt menüde yalnızca iki işlem vardır
            if (choice != 1 && choice != 2)
            {
                WriteError("Seçim 1 veya 2 olmalıdır.");
                return;
            }

            string orderId = ReadText("Sipariş kimliği: ");
            string itemId = ReadText("Ürün kimliği: ");
            string message;
            bool ok;

            // 1 miktar günceller, 2 ürünü siparişten çıkarır
            if (choice == 1)
            {
                int quantity = ReadInt("Yeni miktar (1-20): ");
                ok = restaurant.UpdateOrderItemQuantity(orderId, itemId, quantity, out message);
            }
            else
            {
                ok = restaurant.RemoveItemFromOrder(orderId, itemId, out message);
            }

            ShowOperationResult(ok, message);
        }

        // Seçilen siparişe indirim oranı uygular
        private static void ApplyDiscountFromUser()
        {
            ShowOrders();
            string orderId = ReadText("Sipariş kimliği: ");
            decimal percent = ReadDecimal("İndirim oranı (0-50): ");
            string message;
            bool applied = restaurant.ApplyDiscountToOrder(orderId, percent, out message);
            ShowOperationResult(applied, message);
        }

        // Seçilen siparişin durumunu güncellemeyi dener
        private static void ChangeStatusFromUser()
        {
            ShowOrders();
            string orderId = ReadText("Sipariş kimliği: ");
            Console.WriteLine("1. Preparing");
            Console.WriteLine("2. Served");
            Console.WriteLine("3. Paid");
            Console.WriteLine("4. Cancelled");
            int choice = ReadInt("Yeni durum: ");
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
            bool changed = restaurant.ChangeOrderStatus(orderId, newStatus, out message);
            ShowOperationResult(changed, message);
        }

        // Seçilen siparişin fişini yazar
        private static void PrintReceiptFromUser()
        {
            ShowOrders();
            string orderId = ReadText("Sipariş kimliği: ");
            Order? order = restaurant.FindOrder(orderId);

            // Olmayan sipariş için fiş basılmaz
            if (order == null)
            {
                WriteError("Sipariş bulunamadı.");
                return;
            }

            Console.WriteLine(order.GetReceipt());
        }

        // Tüm siparişleri kısa özetle listeler
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

            // Her sipariş IPrintable üzerinden kendi özetini yazar
            foreach (Order order in orders)
            {
                IPrintable printable = order;
                Console.WriteLine(printable.GetDisplayText());
            }
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

        // Evet veya hayır cevabı okur
        private static bool ReadYesNo(string prompt)
        {
            // E veya H girilene kadar soru tekrarlanır
            while (true)
            {
                string answer = ReadText(prompt + " (E/H): ");

                // Girdi kapandıysa hayır kabul edilir
                if (endOfInput)
                {
                    return false;
                }

                // Büyük veya küçük E olumlu cevaptır
                if (answer == "E" || answer == "e")
                {
                    return true;
                }

                // Büyük veya küçük H olumsuz cevaptır
                if (answer == "H" || answer == "h")
                {
                    return false;
                }

                WriteError("Lütfen E veya H girin.");
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
