using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NETCoreBase.Common.Interfaces
{

    public interface IExcelService
    {
        /// <summary>
        /// 資料列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageResult">Dto</param>
        /// <param name="sFileName"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        void GetDataListFromPageResult<T>(PageResult<T> pageResult, ref string sFileName, Stream output)
            where T : class;

        /// <summary>
        /// 資料列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">Dto</param>
        /// <param name="sFileName"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        void GetDataList<T>(IEnumerable<T> data, ref string sFileName, Stream output)
            where T : class;

        /// <summary>
        /// Json轉成Excel
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        Task<MemoryStream> JsonToExcel(string jsonStr);
    }
}
