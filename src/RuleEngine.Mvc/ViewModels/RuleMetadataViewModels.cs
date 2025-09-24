using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace RuleEngine.Mvc.ViewModels
{
    public class RuleMetadataIndexViewModel
    {
        public List<SelectListItem> CategoryItems { get; set; } = new List<SelectListItem>();
    }

    public class RuleMetadataDetailViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Lütfen metadata adını giriniz")]
        public string Name { get; set; } = "";
        
        public string Title { get; set; } = "";
        
        public string Description { get; set; } = "";
        
        [Required(ErrorMessage = "Lütfen expression giriniz")]
        public string ExpressionString { get; set; } = "";
        
        public bool IsPredicate { get; set; } = true;
        
        public List<string> Categories { get; set; } = new List<string>();
        
        public List<SelectListItem> CategoryItems { get; set; } = new List<SelectListItem>();
        
        public List<SelectListItem> ParameterTypes { get; set; } = new List<SelectListItem>();
        
        public List<ParameterDefinitionModel> Parameters { get; set; } = new List<ParameterDefinitionModel>();
    }

    public class RuleMetadataListRequest
    {
        public string Name { get; set; } = "";
        public string Title { get; set; } = "";
        public string ExpressionString { get; set; } = "";
        public bool? IsPredicate { get; set; }
        public List<int> CategoryItems { get; set; } = new List<int>();
    }

    public class RuleMetadataListResponse
    {
        public List<RuleMetadataDto> Data { get; set; } = new List<RuleMetadataDto>();
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
    }

    public class RuleMetadataDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Title { get; set; } = "";
        public string ExpressionString { get; set; } = "";
        public bool IsPredicate { get; set; }
        public List<string> Categories { get; set; } = new List<string>();
        public string Description { get; set; } = "";
    }

    public class ParameterDefinitionModel
    {
        public string TypeName { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string DisplayFormat { get; set; } = "{0}";
        public string ArrayType { get; set; } = "";
        public string DefinitionType { get; set; } = "";
        public string ReadonlyValue { get; set; } = "";
        public List<string> DestinationTypes { get; set; } = new List<string>();
        public string Filter { get; set; } = "{}";
        public List<string> Operators { get; set; } = new List<string>();
        public string Symbol { get; set; } = "";
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
        public List<ListParameterItemModel> Items { get; set; } = new List<ListParameterItemModel>();
    }

    public class ListParameterItemModel
    {
        public string Value { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string ExpressionFormat { get; set; } = "{0}";
    }
}
