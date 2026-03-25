using System.Data;
using DPM.Core.Interfaces;
using OfficeOpenXml;

namespace DPM.Infrastructure.Readers
{
    public class ExcelReader : IDataSourceReader
    {
        private readonly string _filePath;

        public ExcelReader(string filePath)
        {
            _filePath = filePath;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<DataTable> ReadPreviewAsync(int maxRows = 50)
        {
            return await Task.Run(() => ReadExcel(maxRows));
        }

        public async Task<DataTable> ReadAllAsync()
        {
            return await Task.Run(() => ReadExcel(int.MaxValue));
        }

        private DataTable ReadExcel(int maxRows)
        {
            var dt = new DataTable();
            using var package = new ExcelPackage(new FileInfo(_filePath));
            var sheet = package.Workbook.Worksheets[0];

            int colCount = sheet.Dimension.Columns;
            int rowCount = sheet.Dimension.Rows;

            for (int col = 1; col <= colCount; col++)
                dt.Columns.Add(sheet.Cells[1, col].Text);

            int limit = Math.Min(rowCount, maxRows + 1);
            for (int row = 2; row <= limit; row++)
            {
                var dataRow = dt.NewRow();
                for (int col = 1; col <= colCount; col++)
                    dataRow[col - 1] = sheet.Cells[row, col].Text;
                dt.Rows.Add(dataRow);
            }

            return dt;
        }
    }
}