using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using website_performance.Data;

namespace website_performance.Controllers
{
    public abstract class InitController : Controller
    {
        private ApplicationDbContext _dbContext;
        public ApplicationDbContext DbContext
        {
            get
            {
                if (_dbContext == null)
                {
                    _dbContext = HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
                }

                return _dbContext;
            }
        }
    }
}
