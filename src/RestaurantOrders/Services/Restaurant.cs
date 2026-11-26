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
            menu = new List<MenuItem>();
            customers = new List<Customer>();
            orders = new List<Order>();
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

            menu.Add(item);
            message = "Ürün menüye eklendi.";
            return true;
        }

        // Menüdeki bir ürünün fiyatını günceller
        public bool UpdateMenuItemPrice(string id, decimal newPrice, out string message)
        {
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

            item.ChangePrice(newPrice);
            message = "Ürün fiyatı güncellendi.";
            return true;
        }

        // Menüden ürün siler
        public bool RemoveMenuItem(string id, out string message)
        {
            MenuItem? item = FindMenuItem(id);

            // Olmayan ürün silinmez, liste aynı kalır
            if (item == null)
            {
                message = "Menü ürünü bulunamadı.";
                return false;
            }

            menu.Remove(item);
            message = "Ürün menüden silindi.";
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

            customers.Add(customer);
            message = "Müşteri eklendi.";
            return true;
        }

        // Kayıtlı bir müşteri için yeni sipariş açar
        public bool CreateOrder(string customerId, out Order? order, out string message)
        {
            order = null;
            Customer? customer = FindCustomer(customerId);

            // Bilinmeyen müşteri için sipariş açılmaz
            if (customer == null)
            {
                message = "Müşteri bulunamadı.";
                return false;
            }

            string newId = "O" + nextOrderNumber.ToString("000");
            Order created = new Order(newId, customer);
            orders.Add(created);
            nextOrderNumber = nextOrderNumber + 1;
            order = created;
            message = "Sipariş oluşturuldu.";
            return true;
        }

        // Var olan bir siparişe menüdeki üründen ekler
        public bool AddItemToOrder(string orderId, string itemId, int quantity, out string message)
        {
            Order? order = FindOrder(orderId);

            // Olmayan siparişe ürün eklenmez
            if (order == null)
            {
                message = "Sipariş bulunamadı.";
                return false;
            }

            MenuItem? item = FindMenuItem(itemId);

            // Menüde olmayan ürün siparişe yazılmaz
            if (item == null)
            {
                message = "Menü ürünü bulunamadı.";
                return false;
            }

            return order.AddItem(item, quantity, out message);
        }

        // Siparişten bir ürünü çıkarır
        public bool RemoveItemFromOrder(string orderId, string itemId, out string message)
        {
            Order? order = FindOrder(orderId);

            // Olmayan sipariş değiştirilmez
            if (order == null)
            {
                message = "Sipariş bulunamadı.";
                return false;
            }

            return order.RemoveItem(itemId, out message);
        }

        // Sipariş satırının miktarını günceller
        public bool UpdateOrderItemQuantity(string orderId, string itemId, int newQuantity, out string message)
        {
            Order? order = FindOrder(orderId);

            // Olmayan siparişin miktarı değişmez
            if (order == null)
            {
                message = "Sipariş bulunamadı.";
                return false;
            }

            return order.UpdateQuantity(itemId, newQuantity, out message);
        }

        // Siparişe indirim uygular
        public bool ApplyDiscountToOrder(string orderId, decimal percent, out string message)
        {
            Order? order = FindOrder(orderId);

            // Olmayan siparişe indirim yazılmaz
            if (order == null)
            {
                message = "Sipariş bulunamadı.";
                return false;
            }

            return order.ApplyDiscount(percent, out message);
        }

        // Siparişin durumunu değiştirmeyi dener
        public bool ChangeOrderStatus(string orderId, OrderStatus newStatus, out string message)
        {
            Order? order = FindOrder(orderId);

            // Olmayan siparişin durumu değişmez
            if (order == null)
            {
                message = "Sipariş bulunamadı.";
                return false;
            }

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
