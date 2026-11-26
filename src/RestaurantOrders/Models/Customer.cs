// Bu dosya, sipariş veren müşteriyi temsil eden Customer sınıfını içerir.

using System;

namespace RestaurantOrders.Models
{
    // Restoranda sipariş açabilen müşteri kaydı
    public class Customer
    {
        // Müşterinin benzersiz kimliği (örnek: C01)
        public string Id { get; private set; }

        // Müşterinin adı ve soyadı
        public string FullName { get; private set; }

        // Müşterinin telefon numarası
        public string Phone { get; private set; }

        // Yeni müşteri oluşturur; kimlik, ad ve telefon boş olamaz
        public Customer(string id, string fullName, string phone)
        {
            // Kimlik boşsa müşteri nesnesi kurulamaz
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Müşteri kimliği boş olamaz.");
            }

            // Ad soyad boşsa müşteri nesnesi kurulamaz
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentException("Müşteri adı boş olamaz.");
            }

            // Telefon boşsa müşteri nesnesi kurulamaz
            if (string.IsNullOrWhiteSpace(phone))
            {
                throw new ArgumentException("Telefon boş olamaz.");
            }

            // Baştaki ve sondaki boşluklar kimlikte tutulmaz
            Id = id.Trim();

            // Ad soyad da temizlenerek saklanır
            FullName = fullName.Trim();

            // Telefon metni de temizlenerek saklanır
            Phone = phone.Trim();
        }
    }
}
