// Bu dosya, bir siparişin geçebileceği durumları tanımlayan enum türünü içerir.

namespace RestaurantOrders.Models
{
    // Siparişin yaşam döngüsündeki izinli durumlar
    public enum OrderStatus
    {
        // Sipariş yeni açıldı; ürün ekleme ve iptal buradayken yapılabilir
        Created,

        // Mutfak siparişi hazırlıyor; ürün eklenemez
        Preparing,

        // Sipariş müşteriye servis edildi
        Served,

        // Hesap ödendi; indirim artık uygulanamaz
        Paid,

        // Sipariş iptal edildi; indirim uygulanamaz
        Cancelled
    }
}
