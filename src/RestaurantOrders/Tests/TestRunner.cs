// Bu dosya, iş kurallarını harici bir test aracı olmadan çalıştıran TestRunner sınıfını içerir.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using RestaurantOrders.Models;
using RestaurantOrders.Services;

namespace RestaurantOrders.Tests
{
    // Her testi temiz bir Restaurant nesnesiyle çalıştırır ve sonucu tablo olarak yazar
    public class TestRunner
    {
        // Geçen test sayısı
        private int passedCount;

        // Çalıştırılan test sayısı
        private int totalCount;

        // Dosyaya yazılacak sonuç satırları
        private List<string> reportLines;

        // Sayaçları ve rapor listesini sıfırlar
        public TestRunner()
        {
            passedCount = 0;
            totalCount = 0;
            reportLines = new List<string>();
        }

        // Tüm testleri sırayla çalıştırır, özeti yazar ve dosyaya kaydeder
        public void RunAll()
        {
            Console.WriteLine("===== OTOMATİK TESTLER =====");
            Console.WriteLine();

            RunT1_AddValidFood();
            RunT2_TaxTotal();
            RunT3_NegativePrice();
            RunT4_ZeroQuantity();
            RunT5_DuplicateId();
            RunT6_UnknownCustomer();
            RunT7_QuantityBoundary();
            RunT8_DiscountBoundary();
            RunT9_InvalidStatusJump();
            RunT10_EmptyOrderPreparing();
            RunT11_AddItemWhilePreparing();
            RunT12_FullStatusFlow();

            string summary = passedCount + " / " + totalCount + " test geçti";

            // Tümü geçtiyse özet yeşil, aksi halde kırmızı yazılır
            if (passedCount == totalCount)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Console.WriteLine(summary);
            Console.ResetColor();
            SaveReport(summary);
        }

        // Tek bir testin altı alanını konsola ve rapor listesine yazar
        private void WriteResult(string testNo, string description, string input, string expected, string actual, bool passed)
        {
            totalCount = totalCount + 1;
            string resultText = "FAIL";

            // Yalnızca başarılı test sayacı artırılır
            if (passed)
            {
                passedCount = passedCount + 1;
                resultText = "PASS";
            }

            string block = "Test No            : " + testNo + Environment.NewLine
                + "Açıklama           : " + description + Environment.NewLine
                + "Girdi/Ön koşul     : " + input + Environment.NewLine
                + "Beklenen Sonuç     : " + expected + Environment.NewLine
                + "Gerçek Sonuç       : " + actual + Environment.NewLine
                + "PASS/FAIL          : " + resultText + Environment.NewLine
                + "------------------------------------------------------------";

            reportLines.Add(block);

            int resultIndex = block.LastIndexOf("PASS/FAIL", StringComparison.Ordinal);
            Console.WriteLine(block.Substring(0, resultIndex).TrimEnd());

            // Sonuç satırı başarıda yeşil, hatada kırmızı gösterilir
            if (passed)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Console.WriteLine("PASS/FAIL          : " + resultText);
            Console.ResetColor();
            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine();
        }

        // Rapor metnini çalışma klasöründeki test-results.txt dosyasına kaydeder
        private void SaveReport(string summary)
        {
            string fileText = "RESTAURANT ORDERS - TEST SONUÇLARI" + Environment.NewLine + Environment.NewLine;

            // Her test bloğunu dosya metnine ekliyoruz
            foreach (string line in reportLines)
            {
                fileText = fileText + line + Environment.NewLine;
            }

            fileText = fileText + Environment.NewLine + summary + Environment.NewLine;
            File.WriteAllText("test-results.txt", fileText, Encoding.UTF8);
            Console.WriteLine("Sonuçlar kaydedildi: " + Path.GetFullPath("test-results.txt"));
        }

        // Testlerde ortak kullanılan küçük menü ve müşteriyi kurar
        private Restaurant CreateRestaurantWithMenuAndCustomer()
        {
            Restaurant restaurant = new Restaurant();
            string message;
            bool added;

            added = restaurant.AddMenuItem(new Food("F01", "Corba", 10.00m, true), out message);
            // Kurulum kaydı eklenemezse testi durduruyoruz
            if (added == false)
            {
                throw new InvalidOperationException(message);
            }

            added = restaurant.AddMenuItem(new Food("F02", "Tavuk", 20.00m, false), out message);
            // Kurulum kaydı eklenemezse testi durduruyoruz
            if (added == false)
            {
                throw new InvalidOperationException(message);
            }

            added = restaurant.AddMenuItem(new Drink("D01", "Ayran", 5.00m, 200), out message);
            // Kurulum kaydı eklenemezse testi durduruyoruz
            if (added == false)
            {
                throw new InvalidOperationException(message);
            }

            added = restaurant.AddCustomer(new Customer("C01", "Ayse Demir", "05551112233"), out message);
            // Kurulum kaydı eklenemezse testi durduruyoruz
            if (added == false)
            {
                throw new InvalidOperationException(message);
            }

            return restaurant;
        }

        // Bool değerini Türkçe yazar
        private string YesNo(bool value)
        {
            // Doğruysa Türkçe evet yazılır
            if (value)
            {
                return "evet";
            }

            return "hayır";
        }

        // Ondalık sayıyı test metninde sabit biçimde yazar
        private string Money(decimal amount)
        {
            return amount.ToString("0.00", CultureInfo.InvariantCulture);
        }

        // T1: Geçerli bir yemek menüye eklenir
        private void RunT1_AddValidFood()
        {
            string description = "Geçerli bir Food ürünü eklenir.";
            string input = "Boş restoran. Food F01, Mercimek Corbasi, fiyat 25.00, vejetaryen.";
            string expected = "Ekleme başarılı olur ve menüde 1 ürün bulunur.";

            Restaurant restaurant = new Restaurant();
            Food food = new Food("F01", "Mercimek Corbasi", 25.00m, true);
            string message;
            bool added = restaurant.AddMenuItem(food, out message);
            int count = restaurant.GetMenu().Count;
            string actual = "Ekleme kabul edildi mi: " + YesNo(added) + ". Menüdeki ürün sayısı: " + count + ". Mesaj: " + message;
            bool passed = added && count == 1;
            WriteResult("T1", description, input, expected, actual, passed);
        }

        // T2: İki yemek ve bir içeceğin vergili toplamı polimorfizm ile hesaplanır
        private void RunT2_TaxTotal()
        {
            string description = "Siparişe 2 Food ve 1 Drink eklenir, vergili toplam hesaplanır.";
            string input = "Corba 10.00 TL, Tavuk 20.00 TL, Ayran 5.00 TL; her birinden 1 adet.";
            string expected = "Vergili toplam 39.00 olur (10*1.10 + 20*1.10 + 5*1.20).";

            Restaurant restaurant = CreateRestaurantWithMenuAndCustomer();
            Order? created;
            string message;
            bool orderCreated = restaurant.CreateOrder("C01", out created, out message);

            // Kurulum olmazsa sipariş toplamı diye bir sonuç yoktur
            if (orderCreated == false || created == null)
            {
                WriteResult("T2", description, input, expected, "Sipariş oluşturulamadı: " + message, false);
                return;
            }

            Order order = created;
            bool add1 = restaurant.AddItemToOrder(order.Id, "F01", 1, out message);
            bool add2 = restaurant.AddItemToOrder(order.Id, "F02", 1, out message);
            bool add3 = restaurant.AddItemToOrder(order.Id, "D01", 1, out message);
            MenuItem? food1 = restaurant.FindMenuItem("F01");
            MenuItem? food2 = restaurant.FindMenuItem("F02");
            MenuItem? drink = restaurant.FindMenuItem("D01");

            // Ürün referansı yoksa vergi hesabı yapılamaz
            if (food1 == null || food2 == null || drink == null)
            {
                WriteResult("T2", description, input, expected, "Menü ürünleri bulunamadı.", false);
                return;
            }

            decimal food1Price = food1.GetPriceWithTax();
            decimal food2Price = food2.GetPriceWithTax();
            decimal drinkPrice = drink.GetPriceWithTax();
            decimal total = order.GetTotal();
            string actual = "Ekleme sonuçları: " + YesNo(add1) + ", " + YesNo(add2) + ", " + YesNo(add3)
                + ". Vergi oranları: " + food1.GetTaxRate().ToString("0.00", CultureInfo.InvariantCulture)
                + ", " + food2.GetTaxRate().ToString("0.00", CultureInfo.InvariantCulture)
                + ", " + drink.GetTaxRate().ToString("0.00", CultureInfo.InvariantCulture)
                + ". Vergili birim fiyatlar: " + Money(food1Price) + ", " + Money(food2Price) + ", " + Money(drinkPrice)
                + ". Sipariş toplamı: " + Money(total) + ". Satır sayısı: " + order.Lines.Count;
            bool passed = add1 && add2 && add3
                && food1.GetTaxRate() == 0.10m
                && food2.GetTaxRate() == 0.10m
                && drink.GetTaxRate() == 0.20m
                && food1Price == 11.00m
                && food2Price == 22.00m
                && drinkPrice == 6.00m
                && total == 39.00m
                && order.Lines.Count == 3;
            WriteResult("T2", description, input, expected, actual, passed);
        }

        // T3: Negatif fiyatlı ürün constructor tarafından reddedilir
        private void RunT3_NegativePrice()
        {
            string description = "Fiyatı -5 olan ürün oluşturulamaz.";
            string input = "Food kimliği F99, ad Hatali Urun, fiyat -5.";
            string expected = "ArgumentException fırlatılır ve menüye ürün eklenmez.";

            Restaurant restaurant = new Restaurant();
            bool rejected = false;
            string actual;

            try
            {
                Food invalid = new Food("F99", "Hatali Urun", -5m, false);
                actual = "İstisna fırlatılmadı. Oluşan ürün: " + invalid.Name;
            }
            // Negatif fiyat ArgumentException fırlatmalıdır
            catch (ArgumentException ex)
            {
                rejected = true;
                actual = "ArgumentException: " + ex.Message;
            }

            int count = restaurant.GetMenu().Count;
            actual = actual + " Menüdeki ürün sayısı: " + count + ".";
            bool passed = rejected && count == 0;
            WriteResult("T3", description, input, expected, actual, passed);
        }

        // T4: Sıfır miktar reddedilir ve satır sayısı değişmez
        private void RunT4_ZeroQuantity()
        {
            string description = "Siparişe 0 adet ürün eklenemez.";
            string input = "Created durumundaki boş sipariş, ürün F01, miktar 0.";
            string expected = "İşlem reddedilir, satır sayısı 0 kalır.";

            Restaurant restaurant = CreateRestaurantWithMenuAndCustomer();
            Order? created;
            string message;
            bool orderCreated = restaurant.CreateOrder("C01", out created, out message);

            // Sipariş kurulamazsa bu testi başarısız yazıp çıkıyoruz
            if (orderCreated == false || created == null)
            {
                WriteResult("T4", description, input, expected, "Sipariş oluşturulamadı: " + message, false);
                return;
            }

            Order order = created;
            int before = order.Lines.Count;
            bool added = restaurant.AddItemToOrder(order.Id, "F01", 0, out message);
            int after = order.Lines.Count;
            string actual = "Ekleme kabul edildi mi: " + YesNo(added) + ". Satır sayısı önce: " + before + ", sonra: " + after + ". Mesaj: " + message;
            bool passed = added == false && before == 0 && after == 0;
            WriteResult("T4", description, input, expected, actual, passed);
        }

        // T5: Yinelenen ürün kimliği reddedilir
        private void RunT5_DuplicateId()
        {
            string description = "Aynı kimlikle ikinci ürün eklenemez.";
            string input = "F01 eklendikten sonra farklı adla yine F01 eklenir.";
            string expected = "İkinci ekleme reddedilir, menü sayısı değişmez.";

            Restaurant restaurant = new Restaurant();
            string message;
            bool first = restaurant.AddMenuItem(new Food("F01", "Corba", 10.00m, true), out message);
            int countAfterFirst = restaurant.GetMenu().Count;
            bool second = restaurant.AddMenuItem(new Food("F01", "Baska Corba", 12.00m, false), out message);
            int countAfterSecond = restaurant.GetMenu().Count;
            string actual = "İlk ekleme: " + YesNo(first) + ". İkinci ekleme: " + YesNo(second)
                + ". Menü sayısı ilk eklemeden sonra: " + countAfterFirst + ", ikinci denemeden sonra: " + countAfterSecond
                + ". Mesaj: " + message;
            bool passed = first && second == false && countAfterFirst == 1 && countAfterSecond == 1;
            WriteResult("T5", description, input, expected, actual, passed);
        }

        // T6: Olmayan müşteri kimliği ile sipariş açılmaz
        private void RunT6_UnknownCustomer()
        {
            string description = "Olmayan müşteri ile sipariş oluşturulamaz.";
            string input = "Müşteri listesi boş, müşteri kimliği C99.";
            string expected = "İşlem reddedilir, sipariş sayısı 0 kalır.";

            Restaurant restaurant = new Restaurant();
            Order? order;
            string message;
            int before = restaurant.GetOrders().Count;
            bool created = restaurant.CreateOrder("C99", out order, out message);
            int after = restaurant.GetOrders().Count;
            string actual = "Sipariş oluştu mu: " + YesNo(created) + ". Dönen sipariş null mu: " + YesNo(order == null)
                + ". Sipariş sayısı önce: " + before + ", sonra: " + after + ". Mesaj: " + message;
            bool passed = created == false && order == null && before == 0 && after == 0;
            WriteResult("T6", description, input, expected, actual, passed);
        }

        // T7: Miktar 20 kabul edilir, 21 reddedilir
        private void RunT7_QuantityBoundary()
        {
            string description = "Miktar sınır değeri 20 kabul, 21 red.";
            string input = "Bir siparişe 20 adet F01, başka bir siparişe 21 adet F01.";
            string expected = "20 kabul edilir, 21 reddedilir ve ikinci siparişin satır sayısı 0 kalır.";

            Restaurant restaurant = CreateRestaurantWithMenuAndCustomer();
            Order? firstOrder;
            Order? secondOrder;
            string message;
            bool createdFirst = restaurant.CreateOrder("C01", out firstOrder, out message);
            bool createdSecond = restaurant.CreateOrder("C01", out secondOrder, out message);

            // İki sipariş de açılmazsa sınır testi yapılamaz
            if (createdFirst == false || createdSecond == false || firstOrder == null || secondOrder == null)
            {
                WriteResult("T7", description, input, expected, "Siparişler oluşturulamadı.", false);
                return;
            }

            bool accepted = restaurant.AddItemToOrder(firstOrder.Id, "F01", 20, out message);
            string acceptMessage = message;
            bool rejected = restaurant.AddItemToOrder(secondOrder.Id, "F01", 21, out message);
            int firstCount = firstOrder.Lines.Count;
            int firstQty = 0;

            // Kabul edilen siparişte miktarı okuyabilmek için satır var mı diye bakıyoruz
            if (firstCount > 0)
            {
                firstQty = firstOrder.Lines[0].Quantity;
            }

            int secondCount = secondOrder.Lines.Count;
            string actual = "20 adet kabul edildi mi: " + YesNo(accepted) + " (" + acceptMessage + "). Birinci sipariş satır: " + firstCount
                + ", miktar: " + firstQty + ". 21 adet kabul edildi mi: " + YesNo(rejected)
                + ". İkinci sipariş satır: " + secondCount + ". Mesaj: " + message;
            bool passed = accepted && rejected == false && firstCount == 1 && firstQty == 20 && secondCount == 0;
            WriteResult("T7", description, input, expected, actual, passed);
        }

        // T8: İndirim %50 kabul, %51 red; oran 50 kalır
        private void RunT8_DiscountBoundary()
        {
            string description = "İndirim sınırı %50 kabul, %51 red.";
            string input = "Created durumundaki siparişe önce %50, sonra %51 indirim.";
            string expected = "%50 kabul edilir, %51 reddedilir, indirim %50 kalır.";

            Restaurant restaurant = CreateRestaurantWithMenuAndCustomer();
            Order? created;
            string message;
            bool orderCreated = restaurant.CreateOrder("C01", out created, out message);

            // Sipariş kurulamazsa bu testi başarısız yazıp çıkıyoruz
            if (orderCreated == false || created == null)
            {
                WriteResult("T8", description, input, expected, "Sipariş oluşturulamadı: " + message, false);
                return;
            }

            Order order = created;
            bool accept50 = restaurant.ApplyDiscountToOrder(order.Id, 50m, out message);
            string acceptMessage = message;
            decimal after50 = order.DiscountPercent;
            bool reject51 = restaurant.ApplyDiscountToOrder(order.Id, 51m, out message);
            decimal after51 = order.DiscountPercent;
            string actual = "%50 kabul edildi mi: " + YesNo(accept50) + " (" + acceptMessage + "). Bu adımdan sonra indirim: " + Money(after50)
                + ". %51 kabul edildi mi: " + YesNo(reject51) + ". Son indirim: " + Money(after51) + ". Mesaj: " + message;
            bool passed = accept50 && reject51 == false && after50 == 50m && after51 == 50m;
            WriteResult("T8", description, input, expected, actual, passed);
        }

        // T9: Created durumundan Paid durumuna atlanamaz
        private void RunT9_InvalidStatusJump()
        {
            string description = "Created durumundan doğrudan Paid olunamaz.";
            string input = "Yeni sipariş, istenen geçiş Created -> Paid.";
            string expected = "Geçiş reddedilir, durum Created kalır.";

            Restaurant restaurant = CreateRestaurantWithMenuAndCustomer();
            Order? created;
            string message;
            bool orderCreated = restaurant.CreateOrder("C01", out created, out message);

            // Sipariş kurulamazsa bu testi başarısız yazıp çıkıyoruz
            if (orderCreated == false || created == null)
            {
                WriteResult("T9", description, input, expected, "Sipariş oluşturulamadı: " + message, false);
                return;
            }

            Order order = created;
            OrderStatus before = order.Status;
            int linesBefore = order.Lines.Count;
            bool changed = restaurant.ChangeOrderStatus(order.Id, OrderStatus.Paid, out message);
            string actual = "Geçiş kabul edildi mi: " + YesNo(changed) + ". Durum önce: " + before + ", sonra: " + order.Status
                + ". Satır sayısı: " + linesBefore + " -> " + order.Lines.Count + ". Mesaj: " + message;
            bool passed = changed == false && before == OrderStatus.Created && order.Status == OrderStatus.Created && order.Lines.Count == linesBefore;
            WriteResult("T9", description, input, expected, actual, passed);
        }

        // T10: Boş sipariş Preparing durumuna geçemez
        private void RunT10_EmptyOrderPreparing()
        {
            string description = "Boş sipariş Preparing durumuna geçemez.";
            string input = "Satırı olmayan Created sipariş, istenen durum Preparing.";
            string expected = "Geçiş reddedilir, durum Created kalır.";

            Restaurant restaurant = CreateRestaurantWithMenuAndCustomer();
            Order? created;
            string message;
            bool orderCreated = restaurant.CreateOrder("C01", out created, out message);

            // Sipariş kurulamazsa bu testi başarısız yazıp çıkıyoruz
            if (orderCreated == false || created == null)
            {
                WriteResult("T10", description, input, expected, "Sipariş oluşturulamadı: " + message, false);
                return;
            }

            Order order = created;
            int linesBefore = order.Lines.Count;
            bool changed = restaurant.ChangeOrderStatus(order.Id, OrderStatus.Preparing, out message);
            string actual = "Geçiş kabul edildi mi: " + YesNo(changed) + ". Durum: " + order.Status
                + ". Satır sayısı önce: " + linesBefore + ", sonra: " + order.Lines.Count + ". Mesaj: " + message;
            bool passed = changed == false && order.Status == OrderStatus.Created && linesBefore == 0 && order.Lines.Count == 0;
            WriteResult("T10", description, input, expected, actual, passed);
        }

        // T11: Preparing durumundaki siparişe ürün eklenemez
        private void RunT11_AddItemWhilePreparing()
        {
            string description = "Preparing durumundaki siparişe ürün eklenemez.";
            string input = "F01 adet 2 eklendikten sonra durum Preparing yapılır, ardından D01 eklenmek istenir.";
            string expected = "Ekleme reddedilir, satır sayısı ve miktar değişmez.";

            Restaurant restaurant = CreateRestaurantWithMenuAndCustomer();
            Order? created;
            string message;
            bool orderCreated = restaurant.CreateOrder("C01", out created, out message);

            // Sipariş kurulamazsa bu testi başarısız yazıp çıkıyoruz
            if (orderCreated == false || created == null)
            {
                WriteResult("T11", description, input, expected, "Sipariş oluşturulamadı: " + message, false);
                return;
            }

            Order order = created;
            bool seeded = restaurant.AddItemToOrder(order.Id, "F01", 2, out message);
            bool prepared = restaurant.ChangeOrderStatus(order.Id, OrderStatus.Preparing, out message);
            int countBefore = order.Lines.Count;
            int qtyBefore = 0;

            // Satır varsa eski miktarı kaydediyoruz
            if (countBefore > 0)
            {
                qtyBefore = order.Lines[0].Quantity;
            }

            bool added = restaurant.AddItemToOrder(order.Id, "D01", 1, out message);
            int qtyAfter = 0;

            // Satır hâlâ duruyorsa yeni miktarı okuyoruz
            if (order.Lines.Count > 0)
            {
                qtyAfter = order.Lines[0].Quantity;
            }

            string actual = "İlk ürün eklendi mi: " + YesNo(seeded) + ". Preparing oldu mu: " + YesNo(prepared)
                + ". Son ekleme kabul edildi mi: " + YesNo(added) + ". Durum: " + order.Status
                + ". Satır sayısı önce: " + countBefore + ", sonra: " + order.Lines.Count
                + ". Miktar önce: " + qtyBefore + ", sonra: " + qtyAfter + ". Mesaj: " + message;
            bool passed = seeded && prepared && added == false
                && order.Status == OrderStatus.Preparing
                && countBefore == 1
                && order.Lines.Count == 1
                && qtyBefore == 2
                && qtyAfter == 2;
            WriteResult("T11", description, input, expected, actual, passed);
        }

        // T12: Created -> Preparing -> Served -> Paid akışı tamamlanır
        private void RunT12_FullStatusFlow()
        {
            string description = "Created, Preparing, Served ve Paid akışı tamamlanır.";
            string input = "En az bir satırı olan siparişte üç ardışık durum geçişi.";
            string expected = "Üç geçiş de başarılı olur ve son durum Paid olur.";

            Restaurant restaurant = CreateRestaurantWithMenuAndCustomer();
            Order? created;
            string message;
            bool orderCreated = restaurant.CreateOrder("C01", out created, out message);

            // Sipariş kurulamazsa bu testi başarısız yazıp çıkıyoruz
            if (orderCreated == false || created == null)
            {
                WriteResult("T12", description, input, expected, "Sipariş oluşturulamadı: " + message, false);
                return;
            }

            Order order = created;
            bool seeded = restaurant.AddItemToOrder(order.Id, "F01", 1, out message);
            bool toPreparing = restaurant.ChangeOrderStatus(order.Id, OrderStatus.Preparing, out message);
            bool toServed = restaurant.ChangeOrderStatus(order.Id, OrderStatus.Served, out message);
            bool toPaid = restaurant.ChangeOrderStatus(order.Id, OrderStatus.Paid, out message);
            string actual = "Ürün eklendi mi: " + YesNo(seeded) + ". Preparing: " + YesNo(toPreparing)
                + ". Served: " + YesNo(toServed) + ". Paid: " + YesNo(toPaid) + ". Son durum: " + order.Status;
            bool passed = seeded && toPreparing && toServed && toPaid && order.Status == OrderStatus.Paid;
            WriteResult("T12", description, input, expected, actual, passed);
        }
    }
}
