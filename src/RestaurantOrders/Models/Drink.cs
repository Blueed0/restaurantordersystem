// Bu dosya, menüdeki içecek ürününü temsil eden Drink sınıfını içerir.

using System;

namespace RestaurantOrders.Models
{
    // MenuItem sınıfından türeyen içecek ürünü
    public class Drink : MenuItem
    {
        // İçeceğin mililitre cinsinden hacmi
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

            VolumeMl = volumeMl;
        }

        // İçecek için kurgusal vergi oranı %20'dir
        public override decimal GetTaxRate()
        {
            return 0.20m;
        }

        // Bu ürünün kategorisi içecektir
        public override string GetCategory()
        {
            return "Drink";
        }

        // Üst sınıf metninin sonuna hacmi ekler
        public override string GetDisplayText()
        {
            return base.GetDisplayText() + " | " + VolumeMl + " ml";
        }
    }
}
