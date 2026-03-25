using System.Data;
using DPM.Core.Models;

namespace DPM.Core.Interfaces
{
    public interface ITransformService
    {
        DataTable Apply(DataTable data, IEnumerable<TransformStep> steps);
        DataTable Filter(DataTable data, string column, string op, string value);
        DataTable SelectColumns(DataTable data, IEnumerable<string> columns);
        DataTable OrderBy(DataTable data, string column, bool descending);
        DataTable GroupBy(DataTable data, string groupColumn, string aggColumn, string aggFunc);
        DataTable RemoveNulls(DataTable data);
        DataTable TrimSpaces(DataTable data);
        DataTable ReplaceValue(DataTable data, string column, string oldVal, string newVal);
        DataTable ChangeType(DataTable data, string column, string targetType);
    }
}