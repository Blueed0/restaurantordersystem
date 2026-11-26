// Bu dosya, bir siparişin geçebileceği durumları tanımlayan OrderStatus enum türünü içerir.

namespace RestaurantOrders.Models
{
    // Siparişin yaşam döngüsündeki durumlar
    public enum OrderStatus
    {
        // Sipariş yeni açıldı ve henüz mutfağa gönderilmedi
        Created,

        // Mutfak siparişi hazırlıyor
        Preparing,

        // Sipariş müşteriye servis edildi
        Served,

        // Hesap ödendi
        Paid,

        // Sipariş iptal edildi
        Cancelled
    }
}
