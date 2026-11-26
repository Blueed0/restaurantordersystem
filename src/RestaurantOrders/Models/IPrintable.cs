// Bu dosya, konsolda yazdırılabilen nesnelerin ortak sözleşmesini tanımlayan IPrintable arayüzünü içerir.

namespace RestaurantOrders.Models
{
    // Konsolda gösterilecek metni üreten sınıfların uyması gereken arayüz
    public interface IPrintable
    {
        // Nesnenin konsolda gösterilecek metnini döndürür
        string GetDisplayText();
    }
}
