using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SharedDomain;
using SharedDomain.Entities;

namespace PartnerApp.Pages.PersonPages
{
    public class CreateModel : PageModel
    {
        private readonly SharedDomain.AppDbContext _context;

        public CreateModel(SharedDomain.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public PersonEntity PersonEntity { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.People.Add(PersonEntity);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
