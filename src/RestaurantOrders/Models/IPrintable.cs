// Bu dosya, konsolda yazdırılabilen nesnelerin ortak sözleşmesini tanımlar.

namespace RestaurantOrders.Models
{
    // Food, Drink ve Order bu arayüzü uygulayarak kendi metnini üretir
    public interface IPrintable
    {
        // Nesnenin konsolda gösterilecek metnini döndürür
        string GetDisplayText();
    }
}
