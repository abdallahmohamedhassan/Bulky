using Bulky.DataAccess.Migrations;
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
    public class ApplicationUserRepository : Repository<applicatiouser>, IApplicationUserRepository
    {
        private ApplicationDbContext _context;
        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
     
     

    }
}
