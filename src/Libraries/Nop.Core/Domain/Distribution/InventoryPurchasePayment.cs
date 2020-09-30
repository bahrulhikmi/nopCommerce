using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Distribution
{
    public class InventoryPurchasePayment : BaseEntity
    {
        /// <summary>
        /// Gets or sets the note
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the attached file (download) identifier
        /// </summary>
        public int DownloadId { get; set; }

        /// <summary>
        /// Gets or sets the date and time payment creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the the customer id
        /// </summary>
        public int CustomerId { get; set; }

        public decimal Total { get; set; }

        public InventoryStatus Status { get => (InventoryStatus)StatusId; set => StatusId = (int)value; }

        public int StatusId { get; set; }
    }
}
