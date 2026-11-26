# Restaurant Orders

## 1. Title page info

Course: Object-Oriented Programming

Professor: Besart Prebreza

Topic: 14 – Restaurant Orders

Date: 26 November 2026

## 2. Task 1 – Requirements Analysis

A small restaurant needs a console program that remembers the menu, the customers, and the orders while the program is running. The work is shared by four people, even though the program has one menu. The customer is stored as an id, a name, and a phone number, and later sees the receipt. The waiter opens the order and edits its lines before the kitchen starts. The cashier applies the discount, prints the receipt, and marks the order paid. The manager adds a dish or a drink, changes a price, or removes a product. All data is fictional and stays in memory.

The program must list the menu, add a food or a drink, update a price, and delete a product. It must add and list customers, create an order for a known customer, add a product, change a quantity, remove a line, apply a discount, change the status, print a receipt, and list every order. It must also run the built-in tests. Menu ids, customer ids, and order ids are unique inside their own lists. Order ids are generated as O001, O002, and so on, so the user cannot type a duplicate.

Seven rules decide whether an operation is accepted. A price must be greater than zero. A line quantity must be from 1 to 20, so 0 and 21 are both rejected. A discount must be from 0 to 50 percent. Lines can change only while the order is Created. The allowed status path is Created to Preparing when the order has at least one line, Created to Cancelled, Preparing to Served, and Served to Paid. Any other jump is rejected. Food is taxed at 10 percent and drink at 20 percent. Both rates are fictional.

A successful operation updates the object and returns a short message, which the console prints in green. A rejected operation returns false and a message, which the console prints in red. The lists are not half-updated. The method checks the status, the id, and the numbers first, and only then adds a line, stores a discount, or changes a status. A negative price never becomes an object: the constructor throws ArgumentException, so the item cannot be inserted. Service methods do not use exceptions for these rule failures. They return bool and an out message, and the menu keeps running.

## 3. Task 2 – UML Class Diagram

The class diagram is in docs/UML.md. It uses the same type and member names as the C# files, with `+` for public, `-` for private, and `#` for protected. It shows IPrintable, MenuItem, Food, Drink, Customer, OrderStatus, OrderLine, Order, Restaurant, Program, and TestRunner.

MenuItem is the parent of Food and Drink because both have an id, a name, and a price, while the tax rate and the extra fields differ. IPrintable has only GetDisplayText. MenuItem and Order both implement it. Order is not a kind of menu item, so the print contract is an interface rather than a shared parent. The console can print a mixed menu, and later a list of orders, without asking for the concrete class.

An order is composed of order lines. The lines live in a private list and are shown to callers as a read-only view. The multiplicity is 0..*, not 1..*. A new order is empty until the waiter adds a product, and Preparing is refused until a line exists. Test T10 depends on that empty state. Each order has exactly one customer, and a customer may have zero or many orders. Each line refers to exactly one menu item, while one item may appear on many lines. Restaurant aggregates the menu, the customers, and the orders: it owns the lists, but a line also keeps its own reference to a product, so the product is not an exclusive part of one order. Program uses Restaurant and starts TestRunner.

## 4. Task 3 – C# Implementation

The project is a .NET 8 console application with no NuGet packages and no dependency injection. Fields are private. Public properties use a private setter, so another class cannot assign Price or Status directly.

MenuItem shows the encapsulation. The price can be read, but a new price must pass through ChangePrice.

```csharp
public decimal Price { get; private set; }

public void ChangePrice(decimal newPrice)
{
    if (newPrice <= 0)
    {
        throw new ArgumentException("Fiyat 0'dan büyük olmalıdır.");
    }
    Price = newPrice;
}
```

Constructors check the values that define the object. MenuItem rejects an empty id, an empty name, and a price that is not positive. Drink also rejects a volume that is not positive. Customer rejects an empty id, name, or phone. OrderLine rejects a missing product and a quantity outside 1 to 20. The console catches ArgumentException, so a bad keystroke does not close the program.

Restaurant is the collection and the create-read-update-delete surface. Three lists hold menu items, customers, and orders. AddMenuItem and AddCustomer refuse a duplicate id before the list grows. CreateOrder refuses an unknown customer before it consumes an order number. AddItemToOrder refuses an unknown order or an unknown product before it touches the lines. Find methods return null when the id is missing. GetMenu, GetCustomers, and GetOrders return read-only lists, so the caller cannot insert a row behind the checks.

Food and Drink inherit the shared fields. Food adds IsVegetarian, returns the category Food, and uses a tax rate of 0.10. Drink adds VolumeMl, returns Drink, and uses 0.20. The price formula stays in the parent and calls the abstract rate.

```csharp
public decimal GetPriceWithTax()
{
    return Price * (1 + GetTaxRate());
}
```

The call is polymorphic. A MenuItem variable uses 0.10 when the object is food and 0.20 when it is a drink. OrderLine does not branch on the category. It asks the item for the tax-inclusive price.

```csharp
public decimal GetLineTotal()
{
    return Item.GetPriceWithTax() * Quantity;
}
```

GetDisplayText is the other override. The parent prints id, category, name, and price. Food appends "(Vegetarian)" when the flag is true. Drink appends the volume. The menu loop assigns the item to an IPrintable variable and prints that text, so soup and coffee render differently in the same loop. Order uses the same interface for its short summary.

Order is the composition root. AddItem either appends a line or, if that product is already present, increases the quantity without passing 20. RemoveItem and UpdateQuantity run only in the Created status. ApplyDiscount accepts 0 to 50 percent and refuses Paid and Cancelled. ChangeStatus allows only the four transitions from the rules. GetSubtotal sums the tax-inclusive line totals, the discount is that sum times the percent divided by 100, and GetTotal subtracts the discount. The receipt prints the restaurant name, order id, customer, date, every line, the subtotal, the discount, and the grand total.

The console reads integers with int.TryParse and decimals with decimal.TryParse, and it asks again when the text is empty. Startup loads three foods, three drinks, and three customers. The tests build their own rows instead of using that sample.

## 5. Task 4 – Testing

TestRunner is an ordinary class, not a test framework. RunAll calls twelve methods. Each method creates a new Restaurant, runs one scenario, and prints the test number, the description, the input, the expected result, the actual result, and PASS or FAIL. The actual text is built from the returned bool, the counts, the rates, the total, and the status. The same text is saved with File.WriteAllText to test-results.txt.

T1 adds one valid food and expects a menu count of 1. T2 adds two foods and one drink and expects 10 × 1.10 + 20 × 1.10 + 5 × 1.20 = 39.00. T3 constructs a food with price -5 and expects ArgumentException, with the menu count left at 0. T4 uses quantity 0 and expects the line count to stay 0. T5 repeats a product id and expects the count to stay 1. T6 asks for customer C99 and expects the order count to stay 0. T7 accepts quantity 20 and rejects 21. T8 accepts 50 percent and rejects 51 percent, leaving the discount at 50. T9 rejects Created to Paid. T10 rejects Preparing on an empty order. T11 refuses a new line after Preparing and checks that the old quantity is unchanged. T12 walks Created, Preparing, Served, and Paid.

The failing cases store a count, a quantity, a status, or a discount before the call and compare it afterwards. That comparison is how the suite shows that a rejected operation does not change the system. On 26 November 2026, menu option 13 printed `12 / 12 test geçti`. The full actual strings are in docs/TestCases.md.

| Test | Type | Result |
| --- | --- | --- |
| T1 | Normal add | PASS |
| T2 | Normal tax total | PASS |
| T3 | Invalid price | PASS |
| T4 | Invalid quantity | PASS |
| T5 | Duplicate id | PASS |
| T6 | Unknown id | PASS |
| T7 | Quantity boundary | PASS |
| T8 | Discount boundary | PASS |
| T9 | Invalid transition | PASS |
| T10 | Empty order rule | PASS |
| T11 | Locked order rule | PASS |
| T12 | Full status flow | PASS |

## 6. Task 5 – Design Justification

Each class has one job. MenuItem holds the shared product rules. Food and Drink add only what is special to their category. Customer stores who may order. OrderLine stores one product and one quantity. Order owns the lines, the discount, and the status rules. Restaurant owns the three lists and refuses unknown ids before it calls the order. Program reads input and prints output. TestRunner only checks the rules.

An abstract class is the right parent for the products because the parent has real fields and a real method, GetPriceWithTax, that the children should inherit. An interface cannot hold Id, Name, and Price by itself. The interface is still the right tool for printing, because printing is a capability rather than a family. Order and MenuItem can both be printed without pretending that an order is a menu item.

A procedural version would keep parallel arrays of names, prices, tax flags, and order rows. Deleting one product would mean removing the same index from every array. A short script could print a receipt quickly by editing text, but the status rules would be copied into every script that changes an order. Here those rules sit only in ChangeStatus. A new transition is a change to that method and its test.

One limitation is that the lists disappear when the program exits. The sample menu is loaded again on the next start, and orders from the previous run are gone. One realistic improvement is to write the menu, the customers, and the orders to a text file on exit and to read that file at startup. The class rules would stay the same. The project stops before that step because the assignment keeps the data in lists.

## 7. Task 6 – Documentation

The submitted files are listed below.

- RestaurantOrders.sln and src/RestaurantOrders/RestaurantOrders.csproj define the .NET 8 console project.
- Program.cs is the numbered menu.
- Models/IPrintable.cs, MenuItem.cs, Food.cs, Drink.cs, Customer.cs, OrderStatus.cs, OrderLine.cs, and Order.cs are the domain types.
- Services/Restaurant.cs is the in-memory service.
- Tests/TestRunner.cs runs the twelve tests and writes test-results.txt.
- README.md explains how to build, run, and test, and maps each object-oriented idea to a file.
- docs/UML.md is the class diagram.
- docs/TestCases.md is the test table filled from the real run.
- docs/Report.md is this report.

Source comments are in Turkish. Type names, method names, and variable names are in English.

## 8. Member Contributions

The contribution table is left blank for the project members to complete before submission.

| Member | Contribution |
| --- | --- |
| [TO BE FILLED BY STUDENTS] | [TO BE FILLED BY STUDENTS] |
| [TO BE FILLED BY STUDENTS] | [TO BE FILLED BY STUDENTS] |

## 9. AI Use Statement

Cursor and an AI assistant were used to draft the code skeleton, the Turkish comments, the class diagram, the test runner, and the wording of this report. The draft follows the assignment: a .NET 8 console application, no external packages, data stored in lists, and the business rules from the brief. [REVIEW] Before submission, both members must read every class, explain the status path and the tax calculation aloud, run menu option 13, and check that the diagram matches the code. A sentence that does not sound like the members should be rewritten during that review. The submitted project should be code they can defend.

Approximate word count (excluding code blocks, tables, and UML): 1951
