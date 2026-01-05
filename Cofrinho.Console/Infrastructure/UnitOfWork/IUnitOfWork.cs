using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofrinho.Console.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}