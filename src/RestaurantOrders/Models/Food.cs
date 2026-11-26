// Bu dosya, menüdeki yemek ürününü temsil eden Food sınıfını içerir.

namespace RestaurantOrders.Models
{
    // MenuItem sınıfından türeyen yemek ürünü
    public class Food : MenuItem
    {
        // Yemeğin vejetaryen olup olmadığı
        public bool IsVegetarian { get; private set; }

        // Yeni yemek oluşturur; ortak doğrulama üst sınıfta yapılır
        public Food(string id, string name, decimal price, bool isVegetarian)
            : base(id, name, price)
        {
            IsVegetarian = isVegetarian;
        }

        // Yemek için kurgusal vergi oranı %10'dur
        public override decimal GetTaxRate()
        {
            return 0.10m;
        }

        // Bu ürünün kategorisi yemektir
        public override string GetCategory()
        {
            return "Food";
        }

        // Üst sınıf metnine, vejetaryense "(Vegetarian)" ekler
        public override string GetDisplayText()
        {
            string text = base.GetDisplayText();

            // Vejetaryen yemeklerin sonunda bunu belirtiyoruz
            if (IsVegetarian)
            {
                text = text + " (Vegetarian)";
            }

            return text;
        }
    }
}
