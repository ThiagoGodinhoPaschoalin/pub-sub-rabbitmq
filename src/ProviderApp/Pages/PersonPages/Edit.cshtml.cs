using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProviderApp.Notifications;
using SharedDomain;
using SharedDomain.Entities;

namespace ProviderApp.Pages.PersonPages
{
    public class EditModel(AppDbContext context, IMediator mediator) : PageModel
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

            var OldPerson = await _context.People.AsNoTracking().FirstAsync(x => x.Id == PersonEntity.Id);

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

            await mediator.Publish(new PersonUpdated(OldPerson, PersonEntity));

            return RedirectToPage("./Index");
        }

        private bool PersonEntityExists(Guid id)
        {
            return _context.People.Any(e => e.Id == id);
        }
    }
}
