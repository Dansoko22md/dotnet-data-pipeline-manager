using System.Data;
using System.Text.Json;
using DPM.Core.Interfaces;
using DPM.Core.Models;

namespace DPM.Application.Services
{
    public class TransformService : ITransformService
    {
        public DataTable Apply(DataTable data, IEnumerable<TransformStep> steps)
        {
            var result = data.Copy();
            foreach (var step in steps.OrderBy(s => s.Order))
            {
                var p = JsonSerializer.Deserialize<Dictionary<string, string>>(step.Parameters)
                        ?? new Dictionary<string, string>();
                result = step.Type switch
                {
                    TransformType.Filter =>
                        Filter(result, p["column"], p["op"], p["value"]),
                    TransformType.SelectColumns =>
                        SelectColumns(result, p["columns"].Split(',')),
                    TransformType.OrderBy =>
                        OrderBy(result, p["column"], false),
                    TransformType.OrderByDescending =>
                        OrderBy(result, p["column"], true),
                    TransformType.GroupBy =>
                        GroupBy(result, p["groupColumn"], p["aggColumn"], p["aggFunc"]),
                    TransformType.RemoveNulls =>
                        RemoveNulls(result),
                    TransformType.TrimSpaces =>
                        TrimSpaces(result),
                    TransformType.ReplaceValue =>
                        ReplaceValue(result, p["column"], p["oldVal"], p["newVal"]),
                    TransformType.ChangeType =>
                        ChangeType(result, p["column"], p["targetType"]),
                    _ => result
                };
            }
            return result;
        }

        public DataTable Filter(DataTable data, string column, string op, string value)
        {
            var result = data.Clone();
            foreach (DataRow row in data.Rows)
            {
                var cell = row[column]?.ToString() ?? "";
                bool match = op switch
                {
                    "==" => cell == value,
                    "!=" => cell != value,
                    "contains" => cell.Contains(value, StringComparison.OrdinalIgnoreCase),
                    ">" => double.TryParse(cell, out var a) && double.TryParse(value, out var b) && a > b,
                    "<" => double.TryParse(cell, out var a2) && double.TryParse(value, out var b2) && a2 < b2,
                    ">=" => double.TryParse(cell, out var a3) && double.TryParse(value, out var b3) && a3 >= b3,
                    "<=" => double.TryParse(cell, out var a4) && double.TryParse(value, out var b4) && a4 <= b4,
                    _ => true
                };
                if (match) result.ImportRow(row);
            }
            return result;
        }

        public DataTable SelectColumns(DataTable data, IEnumerable<string> columns)
        {
            var cols = columns.Select(c => c.Trim()).ToList();
            var result = new DataTable();
            foreach (var col in cols)
                if (data.Columns.Contains(col))
                    result.Columns.Add(col, data.Columns[col]!.DataType);

            foreach (DataRow row in data.Rows)
            {
                var newRow = result.NewRow();
                foreach (var col in cols)
                    if (data.Columns.Contains(col))
                        newRow[col] = row[col];
                result.Rows.Add(newRow);
            }
            return result;
        }

        public DataTable OrderBy(DataTable data, string column, bool descending)
        {
            var view = data.DefaultView;
            view.Sort = descending ? $"{column} DESC" : $"{column} ASC";
            return view.ToTable();
        }

        public DataTable GroupBy(DataTable data, string groupColumn, string aggColumn, string aggFunc)
        {
            var result = new DataTable();
            result.Columns.Add(groupColumn);
            result.Columns.Add($"{aggFunc}({aggColumn})", typeof(double));

            var groups = data.AsEnumerable()
                .GroupBy(r => r[groupColumn]?.ToString() ?? "");

            foreach (var group in groups)
            {
                var values = group
                    .Select(r => double.TryParse(r[aggColumn]?.ToString(), out var v) ? v : 0)
                    .ToList();

                double agg = aggFunc.ToUpper() switch
                {
                    "SUM" => values.Sum(),
                    "AVG" => values.Average(),
                    "COUNT" => values.Count,
                    "MIN" => values.Min(),
                    "MAX" => values.Max(),
                    _ => values.Sum()
                };

                result.Rows.Add(group.Key, Math.Round(agg, 2));
            }
            return result;
        }

        public DataTable RemoveNulls(DataTable data)
        {
            var result = data.Clone();
            foreach (DataRow row in data.Rows)
            {
                bool hasNull = row.ItemArray.Any(f =>
                    f == null || f == DBNull.Value || string.IsNullOrWhiteSpace(f.ToString()));
                if (!hasNull) result.ImportRow(row);
            }
            return result;
        }

        public DataTable TrimSpaces(DataTable data)
        {
            var result = data.Copy();
            foreach (DataRow row in result.Rows)
                for (int i = 0; i < result.Columns.Count; i++)
                    if (row[i] is string s)
                        row[i] = s.Trim();
            return result;
        }

        public DataTable ReplaceValue(DataTable data, string column, string oldVal, string newVal)
        {
            var result = data.Copy();
            foreach (DataRow row in result.Rows)
                if (row[column]?.ToString() == oldVal)
                    row[column] = newVal;
            return result;
        }

        public DataTable ChangeType(DataTable data, string column, string targetType)
        {
            var result = new DataTable();
            foreach (DataColumn col in data.Columns)
            {
                if (col.ColumnName == column)
                {
                    Type t = targetType.ToLower() switch
                    {
                        "int" => typeof(int),
                        "double" => typeof(double),
                        "decimal" => typeof(decimal),
                        "datetime" => typeof(DateTime),
                        _ => typeof(string)
                    };
                    result.Columns.Add(column, t);
                }
                else
                {
                    result.Columns.Add(col.ColumnName, col.DataType);
                }
            }

            foreach (DataRow row in data.Rows)
            {
                var newRow = result.NewRow();
                foreach (DataColumn col in data.Columns)
                {
                    if (col.ColumnName == column)
                    {
                        try
                        {
                            newRow[column] = Convert.ChangeType(row[column], result.Columns[column]!.DataType);
                        }
                        catch { newRow[column] = DBNull.Value; }
                    }
                    else newRow[col.ColumnName] = row[col.ColumnName];
                }
                result.Rows.Add(newRow);
            }
            return result;
        }
    }
}