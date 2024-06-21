using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SharedDomain.Entities;

namespace PartnerApp.Pages
{
    public class IndexModel(ILogger<IndexModel> logger, SharedDomain.AppDbContext context) 
        : PageModel
    {
        private readonly ILogger<IndexModel> _logger = logger;
        private readonly SharedDomain.AppDbContext _context = context;

        public IList<PersonEntity> PersonEntity { get; set; } = default!;

        public async Task OnGetAsync()
        {
            PersonEntity = await _context.People.ToListAsync();
        }
    }
}
