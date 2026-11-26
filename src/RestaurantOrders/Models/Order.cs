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

            Id = id.Trim();
            Customer = customer;
            Status = OrderStatus.Created;
            DiscountPercent = 0;
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

                existing.ChangeQuantity(totalQuantity);
                message = "Ürün miktarı artırıldı.";
                return true;
            }

            lines.Add(new OrderLine(item, quantity));
            message = "Ürün siparişe eklendi.";
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

            OrderLine? existing = FindLine(itemId);

            // Ürün bu siparişte yoksa listeye dokunmuyoruz
            if (existing == null)
            {
                message = "Bu ürün siparişte yok.";
                return false;
            }

            lines.Remove(existing);
            message = "Ürün siparişten çıkarıldı.";
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

            existing.ChangeQuantity(newQuantity);
            message = "Ürün miktarı güncellendi.";
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

            DiscountPercent = percent;
            message = "İndirim uygulandı.";
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

                Status = OrderStatus.Preparing;
                message = "Sipariş durumu Preparing oldu.";
                return true;
            }

            // Created durumundaki sipariş iptal edilebilir
            if (Status == OrderStatus.Created && newStatus == OrderStatus.Cancelled)
            {
                Status = OrderStatus.Cancelled;
                message = "Sipariş durumu Cancelled oldu.";
                return true;
            }

            // Hazırlanan sipariş servis edilebilir
            if (Status == OrderStatus.Preparing && newStatus == OrderStatus.Served)
            {
                Status = OrderStatus.Served;
                message = "Sipariş durumu Served oldu.";
                return true;
            }

            // Servis edilen siparişin hesabı kapanabilir
            if (Status == OrderStatus.Served && newStatus == OrderStatus.Paid)
            {
                Status = OrderStatus.Paid;
                message = "Sipariş durumu Paid oldu.";
                return true;
            }

            message = "Bu durum geçişine izin verilmiyor.";
            return false;
        }

        // Vergi dahil satır tutarlarının toplamını döndürür
        public decimal GetSubtotal()
        {
            decimal sum = 0;

            // Her satırın vergi dahil tutarını topluyoruz
            foreach (OrderLine line in lines)
            {
                sum = sum + line.GetLineTotal();
            }

            return sum;
        }

        // Ara toplam üzerinden indirim tutarını döndürür
        public decimal GetDiscountAmount()
        {
            return GetSubtotal() * DiscountPercent / 100m;
        }

        // İndirim düşüldükten sonraki genel toplamı döndürür
        public decimal GetTotal()
        {
            return GetSubtotal() - GetDiscountAmount();
        }

        // Siparişin kısa özetini döndürür
        public string GetDisplayText()
        {
            return Id + " | " + Customer.FullName + " | " + Status + " | " + FormatMoney(GetTotal());
        }

        // Hizalı fiş metnini üretir
        public string GetReceipt()
        {
            int nameWidth = 18;
            int categoryWidth = 10;
            int quantityWidth = 6;
            int unitWidth = 12;
            string receipt = "";

            receipt = receipt + "========================================\r\n";
            receipt = receipt + "            LEZZET SOFRASI\r\n";
            receipt = receipt + "========================================\r\n";
            receipt = receipt + "Sipariş No : " + Id + "\r\n";
            receipt = receipt + "Müşteri    : " + Customer.FullName + " (" + Customer.Id + ")\r\n";
            receipt = receipt + "Telefon    : " + Customer.Phone + "\r\n";
            receipt = receipt + "Tarih      : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) + "\r\n";
            receipt = receipt + "Durum      : " + Status + "\r\n";
            receipt = receipt + "----------------------------------------\r\n";
            receipt = receipt + "Ürün".PadRight(nameWidth);
            receipt = receipt + "Kategori".PadRight(categoryWidth);
            receipt = receipt + "Adet".PadRight(quantityWidth);
            receipt = receipt + "Birim".PadRight(unitWidth);
            receipt = receipt + "Tutar\r\n";
            receipt = receipt + "----------------------------------------\r\n";

            // Her satırı ürün, kategori, adet, birim fiyat ve vergi dahil tutar olarak yazıyoruz
            foreach (OrderLine line in lines)
            {
                string name = line.Item.Name;

                // Uzun adlar sütun hizasını bozmasın diye kısaltıyoruz
                if (name.Length > nameWidth - 1)
                {
                    name = name.Substring(0, nameWidth - 1);
                }

                receipt = receipt + name.PadRight(nameWidth);
                receipt = receipt + line.Item.GetCategory().PadRight(categoryWidth);
                receipt = receipt + line.Quantity.ToString().PadRight(quantityWidth);
                receipt = receipt + FormatMoney(line.Item.Price).PadRight(unitWidth);
                receipt = receipt + FormatMoney(line.GetLineTotal()) + "\r\n";
            }

            receipt = receipt + "----------------------------------------\r\n";
            receipt = receipt + "Ara Toplam        : " + FormatMoney(GetSubtotal()) + "\r\n";
            receipt = receipt + "İndirim (%" + DiscountPercent.ToString("0.##", CultureInfo.InvariantCulture) + ")     : " + FormatMoney(GetDiscountAmount()) + "\r\n";
            receipt = receipt + "GENEL TOPLAM      : " + FormatMoney(GetTotal()) + "\r\n";
            receipt = receipt + "Not: Satır tutarları vergi dahildir (Food %10, Drink %20).\r\n";
            receipt = receipt + "========================================\r\n";
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

            return null;
        }

        // Parayı iki basamaklı ve noktalı yazar
        private string FormatMoney(decimal amount)
        {
            return amount.ToString("0.00", CultureInfo.InvariantCulture) + " TL";
        }
    }
}
