using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleRentalSystem.Domain.Entities;
using VehicleRentalSystem.Domain.Interfaces;

namespace VehicleRentalSystem.Infrastructure.Repositories
{
    public class CustomerApiRepository : ICustomerRepository
    {
        public Task<int> AddAsync(Customer customer)
        {
            //voeg toe via api
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            //delete via api
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Customer>> GetAllAsync()
        {
            //get via api
            throw new NotImplementedException();
        }

        public Task<Customer?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Customer customer)
        {
            throw new NotImplementedException();
        }
    }
}
