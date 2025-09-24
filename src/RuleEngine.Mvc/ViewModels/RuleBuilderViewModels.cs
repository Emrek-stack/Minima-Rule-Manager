using System.ComponentModel.DataAnnotations;

namespace RuleEngine.Mvc.ViewModels
{
    public class RuleBuilderViewModel
    {
        public RuleItemViewModel RuleItem { get; set; } = new RuleItemViewModel();
        public ResultItemViewModel ResultItem { get; set; } = new ResultItemViewModel();
    }

    public class RuleItemViewModel
    {
        public string Name { get; set; } = "";
        public List<object> Children { get; set; } = new List<object>();
        public List<MetadataViewModel> Metadatas { get; set; } = new List<MetadataViewModel>();
    }

    public class ResultItemViewModel
    {
        public List<MetadataViewModel> Metadatas { get; set; } = new List<MetadataViewModel>();
    }

    public class MetadataViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public List<string> ParameterTypes { get; set; } = new List<string>();
        public List<ParameterDefinitionViewModel> ParameterDefinations { get; set; } = new List<ParameterDefinitionViewModel>();
    }

    public class ParameterDefinitionViewModel
    {
        public string Type { get; set; } = "";
        public string Title { get; set; } = "";
        public string Selected { get; set; } = "";
    }

    public class RuleBuilderSaveRequest
    {
        [Required]
        public string Name { get; set; } = "";
        
        [Required]
        public string PredicateExpression { get; set; } = "";
        
        [Required]
        public string ResultExpression { get; set; } = "";
        
        public string Language { get; set; } = "json";
    }
}
