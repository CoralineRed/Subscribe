using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectInformatics.Entities
{
    /// <summary>
    /// Подписка в сервисе (например Яндекс) внутри нашего приложения
    /// </summary>
    public class Subscription
    {
        public int ServiceId { get; set; }
        public int Id { get; set; }
        /// <summary>
        /// Период подписки в днях
        /// </summary>
        public int Period { get; set; }
        [Column(TypeName = "money")]
        public int Price { get; set; }
    }
}
