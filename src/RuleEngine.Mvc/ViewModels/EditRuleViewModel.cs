using System.ComponentModel.DataAnnotations;
using RuleEngine.Core.Models;

namespace RuleEngine.Mvc.ViewModels
{
    public class EditRuleViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(200, ErrorMessage = "Name cannot be longer than 200 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot be longer than 1000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Tags (comma-separated)")]
        public string? Tags { get; set; }

        public RuleStatus Status { get; set; }

        [Required]
        [Display(Name = "Predicate Expression")]
        [StringLength(2000, ErrorMessage = "Predicate expression cannot be longer than 2000 characters.")]
        public string PredicateExpression { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Result Expression")]
        [StringLength(2000, ErrorMessage = "Result expression cannot be longer than 2000 characters.")]
        public string ResultExpression { get; set; } = string.Empty;
    }
}
