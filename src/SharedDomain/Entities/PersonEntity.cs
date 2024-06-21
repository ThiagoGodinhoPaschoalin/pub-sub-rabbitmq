using System.ComponentModel.DataAnnotations;

namespace SharedDomain.Entities
{
    public class PersonEntity
    {
        public PersonEntity()
        {
            Id = Id == Guid.Empty ? Guid.NewGuid() : Id;
        }

        public PersonEntity(string name, string email)
            : this()
        {
            Name = name;
            Email = email;
        }

        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public bool CanItBeShared { get; set; }

        public override string ToString()
        {
            return $"{Name} {Email} {CanItBeShared}";
        }
    }
}
