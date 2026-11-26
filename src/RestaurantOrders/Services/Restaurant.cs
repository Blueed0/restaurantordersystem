// Bu dosya, menü, müşteri ve sipariş listelerini yöneten Restaurant sınıfını içerir.

using System;
using System.Collections.Generic;
using RestaurantOrders.Models;

namespace RestaurantOrders.Services
{
    // Restoranın menüsünü, müşterilerini ve siparişlerini bellekte tutar
    public class Restaurant
    {
        // Menüdeki ürünler
        private List<MenuItem> menu;

        // Kayıtlı müşteriler
        private List<Customer> customers;

        // Açılmış siparişler
        private List<Order> orders;

        // Bir sonraki sipariş numarası; O001 formatı için kullanılır
        private int nextOrderNumber;

        // Boş listelerle yeni bir restoran oluşturur
        public Restaurant()
        {
            // Menü listesi boş başlar
            menu = new List<MenuItem>();

            // Müşteri listesi boş başlar
            customers = new List<Customer>();

            // Sipariş listesi boş başlar
            orders = new List<Order>();

            // İlk sipariş numarası O001 olacaktır
            nextOrderNumber = 1;
        }

        // Menüye ürün ekler; kimlik daha önce kullanıldıysa eklemez
        public bool AddMenuItem(MenuItem item, out string message)
        {
            // Boş ürün referansında menü değişmez
            if (item == null)
            {
                message = "Eklenecek ürün boş olamaz.";
                return false;
            }

            // Aynı kimlik varsa yeni ürün eklenmez
            if (FindMenuItem(item.Id) != null)
            {
                message = "Bu kimlikli ürün zaten var.";
                return false;
            }

            // Ürün menü listesine eklenir
            menu.Add(item);

            // Başarı mesajı yazılır
            message = "Ürün menüye eklendi.";

            // İşlem başarılıdır
            return true;
        }

        // Menüdeki bir ürünün fiyatını günceller
        public bool UpdateMenuItemPrice(string id, decimal newPrice, out string message)
        {
            // Kimliğe göre ürün aranır
            MenuItem? item = FindMenuItem(id);

            // Olmayan ürünün fiyatı değişmez
            if (item == null)
            {
                message = "Menü ürünü bulunamadı.";
                return false;
            }

            // Geçersiz fiyatta eski fiyat korunur
            if (newPrice <= 0)
            {
                message = "Fiyat 0'dan büyük olmalıdır.";
                return false;
            }

            // Yeni fiyat ürüne yazılır
            item.ChangePrice(newPrice);

            // Başarı mesajı yazılır
            message = "Ürün fiyatı güncellendi.";

            // İşlem başarılıdır
            return true;
        }

        // Menüden ürün siler
        public bool RemoveMenuItem(string id, out string message)
        {
            // Kimliğe göre ürün aranır
            MenuItem? item = FindMenuItem(id);

            // Olmayan ürün silinmez, liste aynı kalır
            if (item == null)
            {
                message = "Menü ürünü bulunamadı.";
                return false;
            }

            // Ürün menü listesinden çıkarılır
            menu.Remove(item);

            // Başarı mesajı yazılır
            message = "Ürün menüden silindi.";

            // İşlem başarılıdır
            return true;
        }

        // Yeni müşteri ekler; kimlik benzersiz olmalıdır
        public bool AddCustomer(Customer customer, out string message)
        {
            // Boş müşteri referansında liste değişmez
            if (customer == null)
            {
                message = "Eklenecek müşteri boş olamaz.";
                return false;
            }

            // Aynı kimlik varsa ikinci müşteri eklenmez
            if (FindCustomer(customer.Id) != null)
            {
                message = "Bu kimlikli müşteri zaten var.";
                return false;
            }

            // Müşteri listeye eklenir
            customers.Add(customer);

            // Başarı mesajı yazılır
            message = "Müşteri eklendi.";

            // İşlem başarılıdır
            return true;
        }

        // Kayıtlı bir müşteri için yeni sipariş açar
        public bool CreateOrder(string customerId, out Order? order, out string message)
        {
            // Başarısız durumda dışarı null döner
            order = null;

            // Kimliğe göre müşteri aranır
            Customer? customer = FindCustomer(customerId);

            // Bilinmeyen müşteri için sipariş açılmaz
            if (customer == null)
            {
                message = "Müşteri bulunamadı.";
                return false;
            }

            // Sipariş kimliği O001, O002 biçiminde üretilir
            string newId = "O" + nextOrderNumber.ToString("000");

            // Yeni sipariş nesnesi Created durumunda açılır
            Order created = new Order(newId, customer);

            // Sipariş listeye eklenir
            orders.Add(created);

            // Bir sonraki sipariş için numara artırılır
            nextOrderNumber = nextOrderNumber + 1;

            // Çağırana yeni sipariş verilir
            order = created;

            // Başarı mesajı yazılır
            message = "Sipariş oluşturuldu.";

            // İşlem başarılıdır
            return true;
        }

        // Var olan bir siparişe menüdeki üründen ekler
        public bool AddItemToOrder(string orderId, string itemId, int quantity, out string message)
        {
            // Sipariş kimliğiyle kayıt aranır
            Order? order = FindOrder(orderId);

            // Olmayan siparişe ürün eklenmez
            if (order == null)
            {
                message = "Sipariş bulunamadı.";
                return false;
            }

            // Menü kimliğiyle ürün aranır
            MenuItem? item = FindMenuItem(itemId);

            // Menüde olmayan ürün siparişe yazılmaz
            if (item == null)
            {
                message = "Menü ürünü bulunamadı.";
                return false;
            }

            // Satır ekleme kuralı Order sınıfındadır
            return order.AddItem(item, quantity, out message);
        }

        // Siparişten bir ürünü çıkarır
        public bool RemoveItemFromOrder(string orderId, string itemId, out string message)
        {
            // Sipariş kimliğiyle kayıt aranır
            Order? order = FindOrder(orderId);

            // Olmayan sipariş değiştirilmez
            if (order == null)
            {
                message = "Sipariş bulunamadı.";
                return false;
            }

            // Satır silme kuralı Order sınıfındadır
            return order.RemoveItem(itemId, out message);
        }

        // Sipariş satırının miktarını günceller
        public bool UpdateOrderItemQuantity(string orderId, string itemId, int newQuantity, out string message)
        {
            // Sipariş kimliğiyle kayıt aranır
            Order? order = FindOrder(orderId);

            // Olmayan siparişin miktarı değişmez
            if (order == null)
            {
                message = "Sipariş bulunamadı.";
                return false;
            }

            // Miktar kuralı Order sınıfındadır
            return order.UpdateQuantity(itemId, newQuantity, out message);
        }

        // Siparişe indirim uygular
        public bool ApplyDiscountToOrder(string orderId, decimal percent, out string message)
        {
            // Sipariş kimliğiyle kayıt aranır
            Order? order = FindOrder(orderId);

            // Olmayan siparişe indirim yazılmaz
            if (order == null)
            {
                message = "Sipariş bulunamadı.";
                return false;
            }

            // İndirim kuralı Order sınıfındadır
            return order.ApplyDiscount(percent, out message);
        }

        // Siparişin durumunu değiştirmeyi dener
        public bool ChangeOrderStatus(string orderId, OrderStatus newStatus, out string message)
        {
            // Sipariş kimliğiyle kayıt aranır
            Order? order = FindOrder(orderId);

            // Olmayan siparişin durumu değişmez
            if (order == null)
            {
                message = "Sipariş bulunamadı.";
                return false;
            }

            // Durum geçiş kuralı Order sınıfındadır
            return order.ChangeStatus(newStatus, out message);
        }

        // Kimliğe göre menü ürününü bulur; yoksa null döner
        public MenuItem? FindMenuItem(string id)
        {
            // Boş kimlikte ürün aranmaz
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            // Menü listesinde birebir kimlik eşleşmesi aranır
            foreach (MenuItem item in menu)
            {
                // Aranan kimlik bulunduysa bu ürünü döndürüyoruz
                if (item.Id == id)
                {
                    return item;
                }
            }

            // Bulunamazsa null döner
            return null;
        }

        // Kimliğe göre müşteriyi bulur; yoksa null döner
        public Customer? FindCustomer(string id)
        {
            // Boş kimlikte müşteri aranmaz
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            // Müşteri listesinde birebir kimlik eşleşmesi aranır
            foreach (Customer customer in customers)
            {
                // Aranan kimlik bulunduysa bu müşteriyi döndürüyoruz
                if (customer.Id == id)
                {
                    return customer;
                }
            }

            // Bulunamazsa null döner
            return null;
        }

        // Kimliğe göre siparişi bulur; yoksa null döner
        public Order? FindOrder(string id)
        {
            // Boş kimlikte sipariş aranmaz
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            // Sipariş listesinde birebir kimlik eşleşmesi aranır
            foreach (Order order in orders)
            {
                // Aranan kimlik bulunduysa bu siparişi döndürüyoruz
                if (order.Id == id)
                {
                    return order;
                }
            }

            // Bulunamazsa null döner
            return null;
        }

        // Menünün salt okunur kopyasını döndürür
        public IReadOnlyList<MenuItem> GetMenu()
        {
            return menu.AsReadOnly();
        }

        // Müşteri listesinin salt okunur kopyasını döndürür
        public IReadOnlyList<Customer> GetCustomers()
        {
            return customers.AsReadOnly();
        }

        // Sipariş listesinin salt okunur kopyasını döndürür
        public IReadOnlyList<Order> GetOrders()
        {
            return orders.AsReadOnly();
        }

        // Program açılışında kullanılacak kurgusal menü ve müşterileri yükler
        public void LoadSampleData()
        {
            string message;
            bool added;

            // En az üç yemek örneği eklenir
            added = AddMenuItem(new Food("F01", "Mercimek Çorbası", 45.00m, true), out message);
            // Örnek kayıt eklenemezse kurulum burada durur
            if (added == false)
            {
                throw new InvalidOperationException(message);
            }

            added = AddMenuItem(new Food("F02", "Izgara Tavuk", 120.00m, false), out message);
            // Örnek kayıt eklenemezse kurulum burada durur
            if (added == false)
            {
                throw new InvalidOperationException(message);
            }

            added = AddMenuItem(new Food("F03", "Mevsim Salata", 70.00m, true), out message);
            // Örnek kayıt eklenemezse kurulum burada durur
            if (added == false)
            {
                throw new InvalidOperationException(message);
            }

            // En az üç içecek örneği eklenir
            added = AddMenuItem(new Drink("D01", "Ayran", 25.00m, 300), out message);
            // Örnek kayıt eklenemezse kurulum burada durur
            if (added == false)
            {
                throw new InvalidOperationException(message);
            }

            added = AddMenuItem(new Drink("D02", "Portakal Suyu", 35.00m, 330), out message);
            // Örnek kayıt eklenemezse kurulum burada durur
            if (added == false)
            {
                throw new InvalidOperationException(message);
            }

            added = AddMenuItem(new Drink("D03", "Türk Kahvesi", 40.00m, 70), out message);
            // Örnek kayıt eklenemezse kurulum burada durur
            if (added == false)
            {
                throw new InvalidOperationException(message);
            }

            // En az üç müşteri örneği eklenir
            added = AddCustomer(new Customer("C01", "Ayşe Demir", "05321110001"), out message);
            // Örnek kayıt eklenemezse kurulum burada durur
            if (added == false)
            {
                throw new InvalidOperationException(message);
            }

            added = AddCustomer(new Customer("C02", "Mehmet Kaya", "05321110002"), out message);
            // Örnek kayıt eklenemezse kurulum burada durur
            if (added == false)
            {
                throw new InvalidOperationException(message);
            }

            added = AddCustomer(new Customer("C03", "Elif Yılmaz", "05321110003"), out message);
            // Örnek kayıt eklenemezse kurulum burada durur
            if (added == false)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}
