﻿using Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public interface IReservationRepository
    {
        IQueryable<Reservation> Items { get; }
        Task<int> Add(Reservation reservation);
        Task<int> Update(Reservation reservation);
        Task<int> AddOrUpdate(Reservation reservation);
        Task<int> Delete(Reservation reservation);
    }
}
