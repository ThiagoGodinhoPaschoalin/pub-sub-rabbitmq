using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SharedDomain;
using SharedDomain.Entities;

namespace PartnerApp.Pages.PersonPages
{
    public class EditModel : PageModel
    {
        private readonly SharedDomain.AppDbContext _context;

        public EditModel(SharedDomain.AppDbContext context)
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

            var personentity =  await _context.People.FirstOrDefaultAsync(m => m.Id == id);
            if (personentity == null)
            {
                return NotFound();
            }
            PersonEntity = personentity;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(PersonEntity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonEntityExists(PersonEntity.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool PersonEntityExists(Guid id)
        {
            return _context.People.Any(e => e.Id == id);
        }
    }
}
