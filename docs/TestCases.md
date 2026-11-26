# Test Cases

The tests below were executed on 26 November 2026 by starting the program and choosing menu option 13. Each test created its own `Restaurant` object. The console summary was `12 / 12 test geçti`. The Actual Result column is copied from that run, not written by hand in advance.

| Test No | Type | Input / Preconditions | Expected Result | Actual Result | PASS/FAIL |
| --- | --- | --- | --- | --- | --- |
| T1 | Normal | Empty restaurant. Food F01, Mercimek Corbasi, price 25.00, vegetarian. | The add succeeds and the menu contains 1 item. | Ekleme kabul edildi mi: evet. Menüdeki ürün sayısı: 1. Mesaj: Ürün menüye eklendi. | PASS |
| T2 | Normal | Corba 10.00, Tavuk 20.00, Ayran 5.00; quantity 1 each. | Tax-inclusive total is 39.00 (10*1.10 + 20*1.10 + 5*1.20). | Ekleme sonuçları: evet, evet, evet. Vergi oranları: 0.10, 0.10, 0.20. Vergili birim fiyatlar: 11.00, 22.00, 6.00. Sipariş toplamı: 39.00. Satır sayısı: 3 | PASS |
| T3 | Invalid input | Food id F99, name Hatali Urun, price -5. | ArgumentException is thrown and the item is not added. | ArgumentException: Fiyat 0'dan büyük olmalıdır. Menüdeki ürün sayısı: 0. | PASS |
| T4 | Invalid input | Empty order in Created, item F01, quantity 0. | The operation is rejected and the line count stays 0. | Ekleme kabul edildi mi: hayır. Satır sayısı önce: 0, sonra: 0. Mesaj: Miktar 1 ile 20 arasında olmalıdır. | PASS |
| T5 | Duplicate | F01 is added, then F01 is added again with another name. | The second add is rejected and the menu count does not change. | İlk ekleme: evet. İkinci ekleme: hayır. Menü sayısı ilk eklemeden sonra: 1, ikinci denemeden sonra: 1. Mesaj: Bu kimlikli ürün zaten var. | PASS |
| T6 | Unknown id | Customer list is empty. Customer id is C99. | The operation is rejected and the order count stays 0. | Sipariş oluştu mu: hayır. Dönen sipariş null mu: evet. Sipariş sayısı önce: 0, sonra: 0. Mesaj: Müşteri bulunamadı. | PASS |
| T7 | Boundary | One order receives quantity 20. Another order receives quantity 21. | 20 is accepted. 21 is rejected and that order keeps 0 lines. | 20 adet kabul edildi mi: evet (Ürün siparişe eklendi.). Birinci sipariş satır: 1, miktar: 20. 21 adet kabul edildi mi: hayır. İkinci sipariş satır: 0. Mesaj: Miktar 1 ile 20 arasında olmalıdır. | PASS |
| T8 | Boundary | A Created order receives 50 percent, then 51 percent. | 50 is accepted, 51 is rejected, and the discount stays 50. | %50 kabul edildi mi: evet (İndirim uygulandı.). Bu adımdan sonra indirim: 50.00. %51 kabul edildi mi: hayır. Son indirim: 50.00. Mesaj: İndirim oranı 0 ile 50 arasında olmalıdır. | PASS |
| T9 | Invalid transition | New order. Requested change is Created to Paid. | The change is rejected and the status stays Created. | Geçiş kabul edildi mi: hayır. Durum önce: Created, sonra: Created. Satır sayısı: 0 -> 0. Mesaj: Bu durum geçişine izin verilmiyor. | PASS |
| T10 | Rule | Created order with no lines. Requested status is Preparing. | The change is rejected and the status stays Created. | Geçiş kabul edildi mi: hayır. Durum: Created. Satır sayısı önce: 0, sonra: 0. Mesaj: Boş sipariş hazırlanamaz. | PASS |
| T11 | Rule | F01 quantity 2 is added, status becomes Preparing, then D01 is added. | The add is rejected. Line count and quantity stay the same. | İlk ürün eklendi mi: evet. Preparing oldu mu: evet. Son ekleme kabul edildi mi: hayır. Durum: Preparing. Satır sayısı önce: 1, sonra: 1. Miktar önce: 2, sonra: 2. Mesaj: Yalnızca Created durumundaki siparişe ürün eklenebilir. | PASS |
| T12 | Normal | An order with at least one line follows the three allowed status changes. | All three changes succeed and the final status is Paid. | Ürün eklendi mi: evet. Preparing: evet. Served: evet. Paid: evet. Son durum: Paid | PASS |

Summary: 12 / 12 tests passed.
