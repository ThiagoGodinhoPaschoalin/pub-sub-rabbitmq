using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SharedDomain;
using SharedDomain.Entities;

namespace ProviderApp.Pages
{
    public class IndexModel(ILogger<IndexModel> logger, AppDbContext context) : PageModel
    {
        private readonly ILogger<IndexModel> _logger = logger;
        private readonly AppDbContext _context = context;

        public IList<PersonEntity> PersonEntity { get; set; } = default!;

        public async Task OnGet()
        {
            PersonEntity = await _context.People.ToListAsync();
        }
    }
}
