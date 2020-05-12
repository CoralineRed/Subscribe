using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectInformatics.Entities
{
    /// <summary>
    /// Подписка на наш сервис
    /// </summary>
    public class Category
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int Price { get; set; }
    }
}
