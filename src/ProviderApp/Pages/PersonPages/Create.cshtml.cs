using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProviderApp.Notifications;
using SharedDomain;
using SharedDomain.Entities;

namespace ProviderApp.Pages.PersonPages
{
    public class CreateModel(AppDbContext context, IMediator mediator) : PageModel
    {
        private readonly AppDbContext _context = context;
        private readonly IMediator mediator = mediator;

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

            await mediator.Publish(new PersonCreated(PersonEntity));

            return RedirectToPage("./Index");
        }
    }
}
