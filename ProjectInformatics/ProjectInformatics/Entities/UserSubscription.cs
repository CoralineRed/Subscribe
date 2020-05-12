using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectInformatics.Entities
{
    public class UserSubscription
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SubscriptionId { get; set; }
        public bool IsPaid { get; set; }
        /// <summary>
        /// Подписка всегда на месяц, храним дату начала
        /// </summary>
        public DateTime LastPayment { get; set; }
        public bool IsAutomaticPayment { get; set; }
    }
}
