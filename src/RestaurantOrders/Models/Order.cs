// Bu dosya, müşteriye ait siparişi, satırlarını ve hesap kurallarını içeren Order sınıfını barındırır.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace RestaurantOrders.Models
{
    // Bir müşterinin siparişi; satırlar bu siparişin parçasıdır
    public class Order : IPrintable
    {
        // Sipariş satırları yalnızca bu sınıfın içinde değiştirilir
        private List<OrderLine> lines;

        // Siparişin benzersiz kimliği (örnek: O001)
        public string Id { get; private set; }

        // Siparişi veren müşteri
        public Customer Customer { get; private set; }

        // Siparişin güncel durumu
        public OrderStatus Status { get; private set; }

        // Yüzde olarak indirim; 0 ile 50 arasındadır
        public decimal DiscountPercent { get; private set; }

        // Satırların salt okunur görünümü
        public IReadOnlyList<OrderLine> Lines
        {
            get
            {
                return lines.AsReadOnly();
            }
        }

        // Yeni siparişi Created durumunda ve indirimsiz açar
        public Order(string id, Customer customer)
        {
            // Sipariş kimliği boş olamaz
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Sipariş kimliği boş olamaz.");
            }

            // Sipariş bir müşteriye bağlı olmalıdır
            if (customer == null)
            {
                throw new ArgumentException("Sipariş için müşteri gereklidir.");
            }

            // Kimlikteki boşluklar temizlenerek saklanır
            Id = id.Trim();

            // Sipariş bu müşteriye bağlanır
            Customer = customer;

            // Yeni sipariş her zaman Created ile başlar
            Status = OrderStatus.Created;

            // Başlangıçta indirim yoktur
            DiscountPercent = 0;

            // Satır listesi boş açılır
            lines = new List<OrderLine>();
        }

        // Siparişe ürün ekler; aynı ürün varsa miktarını artırır
        public bool AddItem(MenuItem item, int quantity, out string message)
        {
            // Ürün ekleme yalnızca Created durumunda serbesttir
            if (Status != OrderStatus.Created)
            {
                message = "Yalnızca Created durumundaki siparişe ürün eklenebilir.";
                return false;
            }

            // Boş ürün referansını kabul etmiyoruz
            if (item == null)
            {
                message = "Eklenecek ürün boş olamaz.";
                return false;
            }

            // Miktar 1 ile 20 arasında değilse satır eklemiyoruz
            if (quantity < 1 || quantity > 20)
            {
                message = "Miktar 1 ile 20 arasında olmalıdır.";
                return false;
            }

            OrderLine? existing = FindLine(item.Id);

            // Aynı ürün zaten varsa yeni satır açmak yerine miktarı artırıyoruz
            if (existing != null)
            {
                int totalQuantity = existing.Quantity + quantity;

                // Toplam 20'yi geçerse eski miktar aynen kalır
                if (totalQuantity > 20)
                {
                    message = "Aynı üründen toplam miktar 20'yi geçemez.";
                    return false;
                }

                // Eski satırın miktarı yeni toplama güncellenir
                existing.ChangeQuantity(totalQuantity);

                // Kullanıcıya miktarın arttığı bildirilir
                message = "Ürün miktarı artırıldı.";

                // İşlem başarılıdır
                return true;
            }

            // Ürün ilk kez ekleniyorsa yeni satır açılır
            lines.Add(new OrderLine(item, quantity));

            // Kullanıcıya ekleme bildirilir
            message = "Ürün siparişe eklendi.";

            // İşlem başarılıdır
            return true;
        }

        // Siparişten ürünü kimliğine göre çıkarır
        public bool RemoveItem(string itemId, out string message)
        {
            // Satır silme yalnızca Created durumunda serbesttir
            if (Status != OrderStatus.Created)
            {
                message = "Yalnızca Created durumundaki sipariş değiştirilebilir.";
                return false;
            }

            // Ürün kimliğiyle satır aranır
            OrderLine? existing = FindLine(itemId);

            // Ürün bu siparişte yoksa listeye dokunmuyoruz
            if (existing == null)
            {
                message = "Bu ürün siparişte yok.";
                return false;
            }

            // Bulunan satır listeden silinir
            lines.Remove(existing);

            // Kullanıcıya silme bildirilir
            message = "Ürün siparişten çıkarıldı.";

            // İşlem başarılıdır
            return true;
        }

        // Var olan satırın miktarını değiştirir
        public bool UpdateQuantity(string itemId, int newQuantity, out string message)
        {
            // Miktar değişikliği yalnızca Created durumunda serbesttir
            if (Status != OrderStatus.Created)
            {
                message = "Yalnızca Created durumundaki sipariş değiştirilebilir.";
                return false;
            }

            // Ürün kimliğiyle satır aranır
            OrderLine? existing = FindLine(itemId);

            // Olmayan satırın miktarı değiştirilemez
            if (existing == null)
            {
                message = "Bu ürün siparişte yok.";
                return false;
            }

            // Aralık dışı miktarda eski değer korunur
            if (newQuantity < 1 || newQuantity > 20)
            {
                message = "Miktar 1 ile 20 arasında olmalıdır.";
                return false;
            }

            // Yeni miktar satıra yazılır
            existing.ChangeQuantity(newQuantity);

            // Kullanıcıya güncelleme bildirilir
            message = "Ürün miktarı güncellendi.";

            // İşlem başarılıdır
            return true;
        }

        // Siparişe yüzde indirim uygular
        public bool ApplyDiscount(decimal percent, out string message)
        {
            // Ödenmiş veya iptal edilmiş siparişin indirimi değişmez
            if (Status == OrderStatus.Paid || Status == OrderStatus.Cancelled)
            {
                message = "Ödenmiş veya iptal edilmiş siparişe indirim uygulanamaz.";
                return false;
            }

            // İndirim yalnızca %0 ile %50 arasındadır
            if (percent < 0 || percent > 50)
            {
                message = "İndirim oranı 0 ile 50 arasında olmalıdır.";
                return false;
            }

            // Geçerli oran siparişe yazılır
            DiscountPercent = percent;

            // Kullanıcıya indirim bildirilir
            message = "İndirim uygulandı.";

            // İşlem başarılıdır
            return true;
        }

        // Sipariş durumunu yalnızca izin verilen geçişlerde değiştirir
        public bool ChangeStatus(OrderStatus newStatus, out string message)
        {
            // Created durumundan hazırlığa geçiş için en az bir satır gerekir
            if (Status == OrderStatus.Created && newStatus == OrderStatus.Preparing)
            {
                // Boş sipariş mutfağa gönderilemez
                if (lines.Count == 0)
                {
                    message = "Boş sipariş hazırlanamaz.";
                    return false;
                }

                // Durum hazırlığa alınır
                Status = OrderStatus.Preparing;

                // Başarı mesajı yazılır
                message = "Sipariş durumu Preparing oldu.";

                // Geçiş kabul edilir
                return true;
            }

            // Created durumundaki sipariş iptal edilebilir
            if (Status == OrderStatus.Created && newStatus == OrderStatus.Cancelled)
            {
                // Durum iptale alınır
                Status = OrderStatus.Cancelled;

                // Başarı mesajı yazılır
                message = "Sipariş durumu Cancelled oldu.";

                // Geçiş kabul edilir
                return true;
            }

            // Hazırlanan sipariş servis edilebilir
            if (Status == OrderStatus.Preparing && newStatus == OrderStatus.Served)
            {
                // Durum servise alınır
                Status = OrderStatus.Served;

                // Başarı mesajı yazılır
                message = "Sipariş durumu Served oldu.";

                // Geçiş kabul edilir
                return true;
            }

            // Servis edilen siparişin hesabı kapanabilir
            if (Status == OrderStatus.Served && newStatus == OrderStatus.Paid)
            {
                // Durum ödemeye alınır
                Status = OrderStatus.Paid;

                // Başarı mesajı yazılır
                message = "Sipariş durumu Paid oldu.";

                // Geçiş kabul edilir
                return true;
            }

            // Diğer tüm geçişler reddedilir
            message = "Bu durum geçişine izin verilmiyor.";

            // Sistem durumu değişmez
            return false;
        }

        // Vergi dahil satır tutarlarının toplamını döndürür
        public decimal GetSubtotal()
        {
            // Toplam sıfırdan başlar
            decimal sum = 0;

            // Her satırın vergi dahil tutarını topluyoruz
            foreach (OrderLine line in lines)
            {
                // Satır tutarı ara toplama eklenir
                sum = sum + line.GetLineTotal();
            }

            // Vergi dahil ara toplam döndürülür
            return sum;
        }

        // Ara toplam üzerinden indirim tutarını döndürür
        public decimal GetDiscountAmount()
        {
            // Yüzde oranı ara toplama uygulanır
            return GetSubtotal() * DiscountPercent / 100m;
        }

        // İndirim düşüldükten sonraki genel toplamı döndürür
        public decimal GetTotal()
        {
            // Müşterinin ödeyeceği tutar indirim sonrasıdır
            return GetSubtotal() - GetDiscountAmount();
        }

        // Siparişin kısa özetini döndürür
        public string GetDisplayText()
        {
            // Liste ekranında kimlik, müşteri, durum ve toplam yazılır
            return Id + " | " + Customer.FullName + " | " + Status + " | " + FormatMoney(GetTotal());
        }

        // Hizalı fiş metnini üretir
        public string GetReceipt()
        {
            // Ürün adı sütununun genişliği
            int nameWidth = 18;

            // Kategori sütununun genişliği
            int categoryWidth = 10;

            // Adet sütununun genişliği
            int quantityWidth = 6;

            // Birim fiyat sütununun genişliği
            int unitWidth = 12;

            // Fiş metni boş başlar
            string receipt = "";

            // Üst çerçeve çizilir
            receipt = receipt + "========================================\r\n";

            // Restoran adı yazılır
            receipt = receipt + "            LEZZET SOFRASI\r\n";

            // Alt çerçeve çizilir
            receipt = receipt + "========================================\r\n";

            // Sipariş numarası yazılır
            receipt = receipt + "Sipariş No : " + Id + "\r\n";

            // Müşteri adı ve kimliği yazılır
            receipt = receipt + "Müşteri    : " + Customer.FullName + " (" + Customer.Id + ")\r\n";

            // Telefon yazılır
            receipt = receipt + "Telefon    : " + Customer.Phone + "\r\n";

            // Yazdırma anındaki tarih yazılır
            receipt = receipt + "Tarih      : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) + "\r\n";

            // Güncel durum yazılır
            receipt = receipt + "Durum      : " + Status + "\r\n";

            // Başlık ayırıcı çizilir
            receipt = receipt + "----------------------------------------\r\n";

            // Sütun başlıkları hizalanır
            receipt = receipt + "Ürün".PadRight(nameWidth);
            receipt = receipt + "Kategori".PadRight(categoryWidth);
            receipt = receipt + "Adet".PadRight(quantityWidth);
            receipt = receipt + "Birim".PadRight(unitWidth);
            receipt = receipt + "Tutar\r\n";

            // Başlık altı çizilir
            receipt = receipt + "----------------------------------------\r\n";

            // Her satırı ürün, kategori, adet, birim fiyat ve vergi dahil tutar olarak yazıyoruz
            foreach (OrderLine line in lines)
            {
                // Satırdaki ürün adı alınır
                string name = line.Item.Name;

                // Uzun adlar sütun hizasını bozmasın diye kısaltıyoruz
                if (name.Length > nameWidth - 1)
                {
                    // Sütuna sığacak kadar karakter bırakılır
                    name = name.Substring(0, nameWidth - 1);
                }

                // Ürün adı sütuna yazılır
                receipt = receipt + name.PadRight(nameWidth);

                // Kategori polimorfik olarak alınır
                receipt = receipt + line.Item.GetCategory().PadRight(categoryWidth);

                // Adet yazılır
                receipt = receipt + line.Quantity.ToString().PadRight(quantityWidth);

                // Vergisiz birim fiyat yazılır
                receipt = receipt + FormatMoney(line.Item.Price).PadRight(unitWidth);

                // Vergi dahil satır tutarı yazılır
                receipt = receipt + FormatMoney(line.GetLineTotal()) + "\r\n";
            }

            // Toplam bölümü ayırıcısı çizilir
            receipt = receipt + "----------------------------------------\r\n";

            // Ara toplam yazılır
            receipt = receipt + "Ara Toplam        : " + FormatMoney(GetSubtotal()) + "\r\n";

            // İndirim oranı ve tutarı yazılır
            receipt = receipt + "İndirim (%" + DiscountPercent.ToString("0.##", CultureInfo.InvariantCulture) + ")     : " + FormatMoney(GetDiscountAmount()) + "\r\n";

            // Ödenecek genel toplam yazılır
            receipt = receipt + "GENEL TOPLAM      : " + FormatMoney(GetTotal()) + "\r\n";

            // Vergi notu yazılır
            receipt = receipt + "Not: Satır tutarları vergi dahildir (Food %10, Drink %20).\r\n";

            // Alt çerçeve çizilir
            receipt = receipt + "========================================\r\n";

            // Tamamlanmış fiş döndürülür
            return receipt;
        }

        // Kimliğe göre satırı arar; yoksa null döner
        private OrderLine? FindLine(string itemId)
        {
            // Boş kimlikte arama yapılmaz
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return null;
            }

            // Satırlar arasında aynı ürün kimliğini arıyoruz
            foreach (OrderLine line in lines)
            {
                // Aranan ürün bu satırdaysa satırı döndürüyoruz
                if (line.Item.Id == itemId)
                {
                    return line;
                }
            }

            // Bulunamazsa null döner
            return null;
        }

        // Parayı iki basamaklı ve noktalı yazar
        private string FormatMoney(decimal amount)
        {
            return amount.ToString("0.00", CultureInfo.InvariantCulture) + " TL";
        }
    }
}
