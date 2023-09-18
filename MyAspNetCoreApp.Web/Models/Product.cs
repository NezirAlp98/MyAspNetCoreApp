﻿namespace MyAspNetCoreApp.Web.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Description { get; set; }
        public string? Color { get; set; } //null değer alabileceği için ? koydum
        public DateTime? PublishDate { get; set; }
        public bool IsPublish { get; set; }
        public int Expire { get; set; }
        public string ImagePath { get; set; }



    }
}
