using Microsoft.Extensions.Configuration;
using NETCoreBase.Common.Interfaces;
using NETCoreBase.Common.Model;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NETCoreBase.Common.Services
{
    /// <summary>
    /// Excel服務
    /// </summary>
    public class ExcelService : IExcelService
    {
        public ExcelService()
        {
            ExcelPackage.License.SetNonCommercialPersonal("NETCoreBase");
        }

        static object ProcessValue(PropertyInfo property, object value)
        {
            switch (value)
            {
                case DateTime dt:
                    var dataType = property.GetCustomAttribute<DataTypeAttribute>(false);
                    if (dataType?.DataType == DataType.Date)
                        value = dt.ToString("yyyy-MM-dd");
                    else
                        if (dt.ToString("HH:mm:ss") == "00:00:00")
                    {
                        value = dt.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        value = dt.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    break;
                case TimeSpan time:
                    value = time.ToString("c", CultureInfo.InvariantCulture);
                    break;
                case Enum @enum:
                    value = GetDisplayValue(@enum);
                    break;
                case string[] strArr:
                    value = string.Join(',', strArr);
                    break;
                case bool @bool:
                    value = @bool ? "是" : "否";
                    break;
                default:
                    break;
            }
            return value;
        }

        #region 列表Excel
        /// <summary>
        /// 取得資料Excel
        /// </summary>
        public void GetDataListFromPageResult<T>(PageResult<T> pageResult, ref string sFileName, Stream output)
            where T : class
        {
            this.GetDataList<T>((IEnumerable<T>)pageResult.Items, ref sFileName, output);
        }

        /// <summary>
        /// 取得資料Excel
        /// </summary>
        public void GetDataList<T>(IEnumerable<T> listData, ref string sFileName, Stream output)
            where T : class
        {

            sFileName = $"{sFileName}_{DateTime.Now.ToString("yyyyMMddhhmmss")}.xlsx";

            using (var excel = new ExcelPackage())
            {
                //建立頁籤
                excel.Workbook.Worksheets.Add("Worksheets1");
                ExcelWorksheet sheet1 = excel.Workbook.Worksheets[0];
                Dictionary<string, (int ColumnNum, dynamic Total)> sum = new Dictionary<string, (int, dynamic)>();

                Type type = typeof(T);
                PropertyInfo[] listProperties = type.GetProperties();
                int iListNum = 0;
                foreach (T data in listData)
                {

                    int iColumnNum = 1;
                    foreach (PropertyInfo property in listProperties)
                    {
                        string sCloumName = GetDisplayName(property);

                        //若無欄位名稱略過此參數
                        if (string.IsNullOrWhiteSpace(sCloumName))
                            continue;

                        var excelAttribute = property.GetCustomAttribute<ExcelAttribute>(true);
                        if (iListNum == 0)
                        {
                            sheet1.Cells[1, iColumnNum].Value = sCloumName;

                            if (excelAttribute?.Width > 0)
                                sheet1.Column(iColumnNum).Width = excelAttribute.Width;
                            if (!string.IsNullOrWhiteSpace(excelAttribute?.NumberFormate))
                                sheet1.Column(iColumnNum).Style.Numberformat.Format = excelAttribute.NumberFormate;
                        }
                        var value = property.GetValue(data, null);
                        value = ProcessValue(property, value);
                        sheet1.Cells[iListNum + 2, iColumnNum].Value = value;
                        if (excelAttribute?.HasSum == true)
                        {
                            if (sum.TryGetValue(property.Name, out (int ColumnNum, dynamic Total) dicValue))
                                sum[property.Name] = (iColumnNum, dicValue.Total + (dynamic)value);
                            else
                                sum[property.Name] = (iColumnNum, value);
                        }
                        iColumnNum++;
                    }
                    iListNum++;
                }
                foreach (var item in sum)
                {
                    var property = listProperties.First(p => p.Name == item.Key);
                    object value = item.Value.Total;
                    value = ProcessValue(property, value);
                    sheet1.Cells[iListNum + 2, item.Value.ColumnNum].Value = value;

                    var excelAttribute = property.GetCustomAttribute<ExcelAttribute>(true);
                }

                excel.SaveAs(output);
            }
        }

        /// <summary>
        /// 取得DisplayName
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        private static string GetDisplayName(PropertyInfo propertyInfo)
        {
            string sResult = string.Empty;
            var attr = propertyInfo.GetCustomAttribute<DisplayNameAttribute>(true);
            if (attr != null)
                sResult = attr.DisplayName;
            return sResult;
        }

        private static string GetDisplayValue(Enum value)
        {
            var valueStr = value.ToString();
            var fieldInfo = value.GetType().GetField(valueStr);

            var descriptionAttribute = fieldInfo?.GetCustomAttribute<DisplayAttribute>(false);

            if (descriptionAttribute == null)
                return valueStr;
            return descriptionAttribute.Name;
        }
        #endregion

        /// <summary>
        /// Json轉成Excel
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public async Task<MemoryStream> JsonToExcel(string jsonStr)
        {
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(jsonStr, (typeof(DataTable)));

            #region 產生報表Excel
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add($"sheet1");
                int colCnt = 0;
                int rowCnt = 1;

                #region 表頭
                colCnt++;
                foreach (DataColumn colName in dt.Columns)
                {
                    worksheet.Cells[rowCnt, colCnt++].Value = colName.ColumnName;
                }
                #endregion

                #region 內容
                rowCnt = 2;
                foreach (DataRow row in dt.Rows) //行
                {
                    colCnt = 1;

                    foreach (DataColumn col in dt.Columns) //列
                    {
                        worksheet.Cells[rowCnt, colCnt++].Value = row[col];
                    }

                    rowCnt++;
                }
                #endregion

                return new MemoryStream(await package.GetAsByteArrayAsync());
            }
            #endregion
        }
    }
}