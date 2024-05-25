using System;
using System.Collections.Generic;

namespace Otus.Teaching.PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Preference
        :BaseEntity
    {
        public string Name { get; set; }

        public virtual ICollection<PromoCode> PromoCodes { get; set; }

        public virtual ICollection<CustomerPreference> Customers { get; set; }
    }
}