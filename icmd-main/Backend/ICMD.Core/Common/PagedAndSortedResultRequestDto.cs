using System.ComponentModel.DataAnnotations.Schema;

using ICMD.Core.Constants;

namespace ICMD.Core.Common
{
    public class PagedAndSortedResultRequestDto
    {
        public string Sorting { get; set; }
        public bool SortAcending { get; set; } = true;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Search { get; set; } = string.Empty;
        public Guid? ProjectId { get; set; }
        public List<CustomFieldSearch> CustomSearchList { get; set; }

        [NotMapped]
        List<string> fields = new List<string> { "type", "isExport" };

        [NotMapped]
        public List<CustomFieldSearch> CustomSearchs => CustomSearchList.Where(x => !x.IsColumnFilter).ToList();

        [NotMapped]
        public string SearchFieldQuery =>
            CustomSearchs.Count() != 0 ? (CustomSearchs.Where(s => !fields.Contains(s.FieldName)).Count() != 0 ? string.Join(" and ", CustomSearchs.Select(GetSearchCondition)) : "true") : "true";

        [NotMapped]
        public List<CustomFieldSearch> CustomColumnSearch => CustomSearchList.Where(x => x.IsColumnFilter).ToList();

        [NotMapped]
        public string SearchColumnFilterQuery =>
            CustomColumnSearch.Count() != 0 ? string.Join(" and ", CustomColumnSearch.Select(GetColumnFilterSearchCondition)) : "true";

        //[NotMapped]
        //public string[] IntFieldsArray = { "area" };

        private string GetSearchCondition(CustomFieldSearch search)
        {
            return GetConditions(search);
        }

        private string GetColumnFilterSearchCondition(CustomFieldSearch search)
        {
            return GetConditions(search, true);
        }

        private string GetConditions(CustomFieldSearch search, bool isColumnFilter = false)
        {
            if (!fields.Any(a => a.ToLower() == search.FieldName?.ToLower()) || isColumnFilter)
            {
                SearchType sourceEnum;
                if (Enum.TryParse(search.SearchType.Replace(" ", string.Empty), out sourceEnum))
                {
                    switch (sourceEnum)
                    {
                        case SearchType.StartsWith:
                            return @$"Convert.ToString({search.FieldName}).ToLower().StartsWith(""{search.FieldValue.ToLower()}"")";
                        case SearchType.EndsWith:
                            return @$"Convert.ToString({search.FieldName}).ToLower().EndsWith(""{search.FieldValue.ToLower()}"")";
                        case SearchType.Equals:
                            return @$"Convert.ToString({search.FieldName}).ToLower().Equals(""{search.FieldValue.ToLower()}"")";
                        case SearchType.DoesNotContains:
                            return @$"!Convert.ToString({search.FieldName}).ToLower().Contains(""{search.FieldValue.ToLower()}"")";
                        case SearchType.DoesNotEquals:
                            return @$"!Convert.ToString({search.FieldName}).ToLower().Equals(""{search.FieldValue.ToLower()}"")";
                        default:
                            return @$"Convert.ToString({search.FieldName}).ToLower().Contains(""{search.FieldValue.ToLower()}"")";
                    }
                }
                else
                    return @$"Convert.ToString({search.FieldName}).ToLower().Contains(""{search.FieldValue.ToLower()}"")";

            }
            else
                return "true";
        }
    }


    public class CustomFieldSearch
    {
        public string FieldValue { get; set; }
        public string FieldName { get; set; }
        public string SearchType { get; set; }
        public bool IsColumnFilter { get; set; } = false;
    }


}
