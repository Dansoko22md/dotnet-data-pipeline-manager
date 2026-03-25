using System.Data;
using System.Globalization;
using CsvHelper;
using DPM.Core.Interfaces;

namespace DPM.Infrastructure.Readers
{
    public class CsvDataReader : IDataSourceReader
    {
        private readonly string _filePath;

        public CsvDataReader(string filePath)
        {
            _filePath = filePath;
        }

        public async Task<DataTable> ReadPreviewAsync(int maxRows = 50)
        {
            return await Task.Run(() => ReadCsv(maxRows));
        }

        public async Task<DataTable> ReadAllAsync()
        {
            return await Task.Run(() => ReadCsv(int.MaxValue));
        }

        private DataTable ReadCsv(int maxRows)
        {
            var dt = new DataTable();
            using var reader = new StreamReader(_filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            csv.Read();
            csv.ReadHeader();

            foreach (var header in csv.HeaderRecord!)
                dt.Columns.Add(header);

            int count = 0;
            while (csv.Read() && count < maxRows)
            {
                var row = dt.NewRow();
                foreach (DataColumn col in dt.Columns)
                    row[col.ColumnName] = csv.GetField(col.ColumnName) ?? "";
                dt.Rows.Add(row);
                count++;
            }

            return dt;
        }
    }
}