// Bu dosya, sipariş veren müşteriyi temsil eden Customer sınıfını içerir.

using System;

namespace RestaurantOrders.Models
{
    // Restoranda sipariş açabilen müşteri
    public class Customer
    {
        // Müşterinin benzersiz kimliği (örnek: C01)
        public string Id { get; private set; }

        // Müşterinin adı ve soyadı
        public string FullName { get; private set; }

        // Müşterinin telefon numarası
        public string Phone { get; private set; }

        // Yeni müşteri oluşturur; kimlik ve ad boş olamaz
        public Customer(string id, string fullName, string phone)
        {
            // Kimlik boşsa müşteri oluşturulamaz
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Müşteri kimliği boş olamaz.");
            }

            // Ad soyad boşsa müşteri oluşturulamaz
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentException("Müşteri adı boş olamaz.");
            }

            // Telefon boşsa müşteri oluşturulamaz
            if (string.IsNullOrWhiteSpace(phone))
            {
                throw new ArgumentException("Telefon boş olamaz.");
            }

            Id = id.Trim();
            FullName = fullName.Trim();
            Phone = phone.Trim();
        }
    }
}
