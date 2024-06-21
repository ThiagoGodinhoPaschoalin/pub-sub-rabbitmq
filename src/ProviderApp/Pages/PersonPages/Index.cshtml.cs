using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SharedDomain;
using SharedDomain.Entities;

namespace ProviderApp.Pages.PersonPages
{
    public class IndexModel : PageModel
    {
        private readonly SharedDomain.AppDbContext _context;

        public IndexModel(SharedDomain.AppDbContext context)
        {
            _context = context;
        }

        public IList<PersonEntity> PersonEntity { get;set; } = default!;

        public async Task OnGetAsync()
        {
            PersonEntity = await _context.People.ToListAsync();
        }
    }
}
