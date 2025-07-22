using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private ApplicationDbContext _context;
        public OrderDetailRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
     
        public void Update(OrderDetail orderDetail)
        {
            _context.OrderDetails.Update(orderDetail);

        }

    }
}
