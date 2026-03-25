namespace DPM.Core.Models
{
    public enum TransformType
    {
        Filter,
        SelectColumns,
        OrderBy,
        OrderByDescending,
        GroupBy,
        RemoveNulls,
        TrimSpaces,
        ReplaceValue,
        ChangeType
    }

    public class TransformStep
    {
        public int Id { get; set; }
        public TransformType Type { get; set; }
        public string Parameters { get; set; } = string.Empty;
        public int Order { get; set; }
        public int PipelineId { get; set; }
        public Pipeline? Pipeline { get; set; }
    }
}
