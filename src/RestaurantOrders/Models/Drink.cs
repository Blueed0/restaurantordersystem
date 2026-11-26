// Bu dosya, menüdeki içecek ürününü temsil eden Drink sınıfını içerir.

using System;

namespace RestaurantOrders.Models
{
    // MenuItem soyut sınıfından türeyen içecek ürünü
    public class Drink : MenuItem
    {
        // İçeceğin mililitre cinsinden hacmini tutar
        public int VolumeMl { get; private set; }

        // Yeni içecek oluşturur; hacim 0'dan büyük olmalıdır
        public Drink(string id, string name, decimal price, int volumeMl)
            : base(id, name, price)
        {
            // Hacim sıfır veya negatifse içecek oluşturulamaz
            if (volumeMl <= 0)
            {
                throw new ArgumentException("Hacim 0'dan büyük olmalıdır.");
            }

            // Geçerli hacim alana yazılır
            VolumeMl = volumeMl;
        }

        // İçecek için kurgusal vergi oranı yüzde 20'dir
        public override decimal GetTaxRate()
        {
            // Polimorfik vergi hesabı için 0.20 döndürülür
            return 0.20m;
        }

        // Bu ürünün kategorisi içecektir
        public override string GetCategory()
        {
            // Liste ve fişte "Drink" yazılır
            return "Drink";
        }

        // Üst sınıf metninin sonuna hacmi ekler
        public override string GetDisplayText()
        {
            // Ortak metne mililitre bilgisi eklenerek döndürülür
            return base.GetDisplayText() + " | " + VolumeMl + " ml";
        }
    }
}
