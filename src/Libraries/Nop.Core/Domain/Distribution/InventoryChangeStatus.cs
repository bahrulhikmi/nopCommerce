using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Distribution
{
    public enum InventoryChangeStatus
    {
        None = 0,
        /// <summary>
        /// Pending
        /// </summary>
        Pending = 10,

        /// <summary>
        /// Processing
        /// </summary>
        Processing = 20,

        /// <summary>
        /// Complete
        /// </summary>
        Complete = 30,

        /// <summary>
        /// Cancelled
        /// </summary>
        Cancelled = 40

    }
}
