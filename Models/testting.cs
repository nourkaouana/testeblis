
    public class PlaceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int? NumberOfTables { get; set; }
        public string? Font { get; set; }
        public string? Color { get; set; }

        public List<CategoryDto>? Categories { get; set; } // Include categories in the DTO
    }

    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Include Place information (you can simplify it if you don't want all fields)
        public PlaceDto Place { get; set; }
    }




    public class MenuItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string Image { get; set; }
        public bool IsAvailable { get; set; }
        public int Category{ get; set; }
    }

    public class OrderDto
    {
        public int Id { get; set; }
        public int PlaceId { get; set; }
        public string Table { get; set; }
        public string Detail { get; set; }
        public string PaymentIntent { get; set; }
        public int Amount { get; set; }
        public string Status { get; set; }
    }

