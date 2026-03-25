

using System.Data;

namespace DPM.Core.Interfaces
{
    public interface IDataSourceReader
    {
        Task<DataTable> ReadPreviewAsync(int maxRows = 50);
        Task<DataTable> ReadAllAsync();
    }
}
