using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity
{
    class Customer : PersonBase
    {
        public bool IsAdult { get; set; }
    }
}
