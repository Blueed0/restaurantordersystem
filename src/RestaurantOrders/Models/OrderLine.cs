// Bu dosya, siparişin içindeki tek bir ürün satırını temsil eder.

using System;

namespace RestaurantOrders.Models
{
    // Bir sipariş satırı: hangi üründen kaç adet olduğu
    public class OrderLine
    {
        // Satırdaki menü ürünü referansı
        public MenuItem Item { get; private set; }

        // Üründen kaç adet istendiği
        public int Quantity { get; private set; }

        // Yeni sipariş satırı oluşturur; miktar 1 ile 20 arasında olmalıdır
        public OrderLine(MenuItem item, int quantity)
        {
            // Satırın bir ürüne bağlanması zorunludur
            if (item == null)
            {
                throw new ArgumentException("Sipariş satırı için ürün gereklidir.");
            }

            // Miktar 1 ile 20 arasında değilse satır oluşturulamaz
            if (quantity < 1 || quantity > 20)
            {
                throw new ArgumentException("Miktar 1 ile 20 arasında olmalıdır.");
            }

            // Ürün referansı saklanır
            Item = item;

            // Geçerli miktar saklanır
            Quantity = quantity;
        }

        // Vergili satır tutarını hesaplar; vergi ürünün kendi oranından gelir
        public decimal GetLineTotal()
        {
            // Food yüzde 10, Drink yüzde 20 ile birim fiyat çarpılır, sonra miktar uygulanır
            return Item.GetPriceWithTax() * Quantity;
        }

        // Satır miktarını değiştirir; yeni miktar 1 ile 20 arasında olmalıdır
        public void ChangeQuantity(int newQuantity)
        {
            // Aralık dışındaki miktarı kabul etmiyoruz
            if (newQuantity < 1 || newQuantity > 20)
            {
                throw new ArgumentException("Miktar 1 ile 20 arasında olmalıdır.");
            }

            // Geçerli yeni miktar yazılır
            Quantity = newQuantity;
        }
    }
}
