// Bu dosya, menüdeki yemek ürününü temsil eden Food sınıfını içerir.

namespace RestaurantOrders.Models
{
    // MenuItem soyut sınıfından türeyen yemek ürünü
    public class Food : MenuItem
    {
        // Yemeğin vejetaryen olup olmadığını tutar
        public bool IsVegetarian { get; private set; }

        // Yeni yemek oluşturur; ortak doğrulama MenuItem constructor'ında yapılır
        public Food(string id, string name, decimal price, bool isVegetarian)
            : base(id, name, price)
        {
            // Vejetaryen bilgisi alt sınıfa özeldir
            IsVegetarian = isVegetarian;
        }

        // Yemek için kurgusal vergi oranı yüzde 10'dur
        public override decimal GetTaxRate()
        {
            // Polimorfik vergi hesabı için 0.10 döndürülür
            return 0.10m;
        }

        // Bu ürünün kategorisi yemektir
        public override string GetCategory()
        {
            // Liste ve fişte "Food" yazılır
            return "Food";
        }

        // Üst sınıf metnine, vejetaryense ek bilgi ekler
        public override string GetDisplayText()
        {
            // Ortak kimlik, kategori, ad ve fiyat metni alınır
            string text = base.GetDisplayText();

            // Vejetaryen yemeklerin sonunda bunu belirtiyoruz
            if (IsVegetarian)
            {
                // Kullanıcı listede vejetaryen olduğunu görsün
                text = text + " (Vegetarian)";
            }

            // Tamamlanmış satır metni döndürülür
            return text;
        }
    }
}
