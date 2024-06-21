using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SharedDomain;
using SharedDomain.Entities;

namespace PartnerApp.Pages.PersonPages
{
    public class DeleteModel : PageModel
    {
        private readonly SharedDomain.AppDbContext _context;

        public DeleteModel(SharedDomain.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PersonEntity PersonEntity { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personentity = await _context.People.FirstOrDefaultAsync(m => m.Id == id);

            if (personentity == null)
            {
                return NotFound();
            }
            else
            {
                PersonEntity = personentity;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personentity = await _context.People.FindAsync(id);
            if (personentity != null)
            {
                PersonEntity = personentity;
                _context.People.Remove(PersonEntity);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
