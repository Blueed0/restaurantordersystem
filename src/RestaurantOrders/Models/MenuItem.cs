// Bu dosya, menüdeki ortak ürün özelliklerini tanımlayan soyut MenuItem sınıfını içerir.

using System;
using System.Globalization;

namespace RestaurantOrders.Models
{
    // Yemek ve içeceğin ortak özelliklerini tutan soyut sınıf
    public abstract class MenuItem : IPrintable
    {
        // Ürünün benzersiz kimliği (örnek: F01)
        public string Id { get; private set; }

        // Ürünün adı
        public string Name { get; private set; }

        // Vergisiz birim fiyat; dışarıdan yalnızca okunur
        public decimal Price { get; private set; }

        // Yeni menü ürünü oluşturur ve temel alanları doğrular
        protected MenuItem(string id, string name, decimal price)
        {
            // Kimlik boşsa ürün oluşturulamaz
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Ürün kimliği boş olamaz.");
            }

            // Ad boşsa ürün oluşturulamaz
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Ürün adı boş olamaz.");
            }

            // Fiyat sıfır veya negatifse ürün oluşturulamaz
            if (price <= 0)
            {
                throw new ArgumentException("Fiyat 0'dan büyük olmalıdır.");
            }

            // Kimlikteki gereksiz boşluklar temizlenir
            Id = id.Trim();

            // Ad temizlenerek saklanır
            Name = name.Trim();

            // Geçerli fiyat saklanır
            Price = price;
        }

        // Alt sınıf kendi vergi oranını verir
        public abstract decimal GetTaxRate();

        // Alt sınıf kendi kategorisini verir: Food veya Drink
        public abstract string GetCategory();

        // Vergili birim fiyatı hesaplar; vergi oranı alt sınıftan gelir
        public decimal GetPriceWithTax()
        {
            // Fiyat * (1 + vergi) formülü polimorfik GetTaxRate ile çalışır
            return Price * (1 + GetTaxRate());
        }

        // Kimlik, kategori, ad ve vergisiz fiyatı yazar
        public virtual string GetDisplayText()
        {
            // Alt sınıflar bu metni genişletebilir
            return Id + " | " + GetCategory() + " | " + Name + " | " + FormatMoney(Price);
        }

        // Ürün fiyatını günceller; yeni fiyat 0'dan büyük olmalıdır
        public void ChangePrice(decimal newPrice)
        {
            // Geçersiz fiyatta eski fiyat korunur ve hata fırlatılır
            if (newPrice <= 0)
            {
                throw new ArgumentException("Fiyat 0'dan büyük olmalıdır.");
            }

            // Geçerli yeni fiyat yazılır
            Price = newPrice;
        }

        // Parayı iki basamaklı ve noktalı yazar
        private string FormatMoney(decimal amount)
        {
            // Liste metninde para birimi TL olarak gösterilir
            return amount.ToString("0.00", CultureInfo.InvariantCulture) + " TL";
        }
    }
}
