using System.Net.Http.Json;

class Program
{
    static async Task Main(string[] args)
    {
        var baseUrl = "http://localhost:5243/api/product";

        using var httpClient = new HttpClient();

        while (true)
        {
            Console.WriteLine("\nВыберите действие:");
            Console.WriteLine("1. Просмотреть все продукты");
            Console.WriteLine("2. Добавить новый продукт");
            Console.WriteLine("3. Обновить существующий продукт");
            Console.WriteLine("4. Удалить продукт");
            Console.WriteLine("5. Выход");
            Console.Write("Введите номер действия: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await GetProducts(httpClient, baseUrl);
                    break;

                case "2":
                    await CreateProduct(httpClient, baseUrl);
                    break;

                case "3":
                    await UpdateProduct(httpClient, baseUrl);
                    break;

                case "4":
                    await DeleteProduct(httpClient, baseUrl);
                    break;

                case "5":
                    return;

                default:
                    Console.WriteLine("Неверный выбор. Пожалуйста, попробуйте снова.");
                    break;
            }
        }
    }

    //read
    static async Task GetProducts(HttpClient httpClient, string baseUrl)
    {
        Console.WriteLine("\nЗагрузка продуктов...");
        var products = await httpClient.GetFromJsonAsync<List<Product>>(baseUrl);
        if (products != null && products.Any())
        {
            foreach (var product in products)
            {
                Console.WriteLine($"{product.ProductId}: {product.ProductName} - {product.UnitPrice:C}");
            }
        }
        else
        {
            Console.WriteLine("Продукты не найдены.");
        }
    }

    //create
    static async Task CreateProduct(HttpClient httpClient, string baseUrl)
    {
        Console.WriteLine("\nДобавление нового продукта...");
        var product = new Product();

        Console.Write("Введите название продукта: ");
        product.ProductName = Console.ReadLine();

        Console.Write("Введите ID поставщика: ");
        product.SupplierId = int.Parse(Console.ReadLine());

        Console.Write("Введите ID категории: ");
        product.CategoryId = int.Parse(Console.ReadLine());

        Console.Write("Введите количество в упаковке: ");
        product.QuantityPerUnit = Console.ReadLine();

        Console.Write("Введите цену за единицу: ");
        product.UnitPrice = decimal.Parse(Console.ReadLine());

        Console.Write("Введите количество в наличии: ");
        product.UnitsInStock = short.Parse(Console.ReadLine());

        Console.Write("Введите количество на заказ: ");
        product.UnitsOnOrder = short.Parse(Console.ReadLine());

        Console.Write("Введите уровень запаса для повторного заказа: ");
        product.ReorderLevel = short.Parse(Console.ReadLine());

        Console.Write("Продукт снят с производства (true/false): ");
        product.Discontinued = bool.Parse(Console.ReadLine());

        var response = await httpClient.PostAsJsonAsync(baseUrl, product);
        Console.WriteLine($"Статус create запроса: {response.StatusCode}");
    }

    //update
    static async Task UpdateProduct(HttpClient httpClient, string baseUrl)
    {
        Console.Write("\nВведите ID продукта для обновления: ");
        var id = int.Parse(Console.ReadLine());

        var product = await httpClient.GetFromJsonAsync<Product>($"{baseUrl}/{id}");
        if (product == null)
        {
            Console.WriteLine("Продукт не найден.");
            return;
        }

        Console.WriteLine($"Текущее название: {product.ProductName}");
        Console.Write("Введите новое название (или оставьте пустым, чтобы оставить текущее): ");
        var newName = Console.ReadLine();
        if (!string.IsNullOrEmpty(newName))
        {
            product.ProductName = newName;
        }

        Console.WriteLine($"Текущая цена: {product.UnitPrice}");
        Console.Write("Введите новую цену (или оставьте пустым, чтобы оставить текущую): ");
        var newPriceInput = Console.ReadLine();
        if (!string.IsNullOrEmpty(newPriceInput))
        {
            product.UnitPrice = decimal.Parse(newPriceInput);
        }

        var response = await httpClient.PutAsJsonAsync($"{baseUrl}/{id}", product);
        Console.WriteLine($"Статус update запроса: {response.StatusCode}");
    }

    //delete
    static async Task DeleteProduct(HttpClient httpClient, string baseUrl)
    {
        Console.Write("\nВведите ID продукта для удаления: ");
        var id = int.Parse(Console.ReadLine());

        var response = await httpClient.DeleteAsync($"{baseUrl}/{id}");
        Console.WriteLine($"Статус DELETE запроса: {response.StatusCode}");
    }
}

public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int SupplierId { get; set; }
    public int CategoryId { get; set; }
    public string QuantityPerUnit { get; set; }
    public decimal UnitPrice { get; set; }
    public short UnitsInStock { get; set; }
    public short UnitsOnOrder { get; set; }
    public short ReorderLevel { get; set; }
    public bool Discontinued { get; set; }
}
