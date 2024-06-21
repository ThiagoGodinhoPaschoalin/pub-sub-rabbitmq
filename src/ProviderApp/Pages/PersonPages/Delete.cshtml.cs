using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProviderApp.Notifications;
using SharedDomain;
using SharedDomain.Entities;

namespace ProviderApp.Pages.PersonPages
{
    public class DeleteModel(AppDbContext context, IMediator mediator) : PageModel
    {
        private readonly AppDbContext _context = context;
        private readonly IMediator mediator = mediator;

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
                await mediator.Publish(new PersonInactivated(personentity));
            }

            return RedirectToPage("./Index");
        }
    }
}
