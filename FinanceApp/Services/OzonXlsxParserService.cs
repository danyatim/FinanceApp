using FinanceApp.Models;
using OfficeOpenXml;
using System.Diagnostics;
using System.Globalization;
using System.Security.Principal;

namespace FinanceApp.Services
{
    public class OzonXlsxParserService
    {
        public static List<OzonSellerXlsxPrice> ParsePrices(string filePath)
        {
            // Получаем информацию о текущем пользователе Windows
            WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();

            // Извлекаем имя пользователя из полученной информации
            string userName = currentIdentity.Name.Split("\\")[0];

            // Включение лицензии c именем пользователя
            new EPPlusLicense().SetNonCommercialPersonal(userName);

            var result = new List<OzonSellerXlsxPrice>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets["Товары и цены"];
                if (worksheet == null)
                {
                    Debug.WriteLine("Лист 'Товары и цены' не найден");
                    return result;
                }
                var dimension = worksheet.Dimension;
                if (dimension == null)
                {
                    Debug.WriteLine("Лист пустой");
                    return result;
                }

                int startRow = dimension.Start.Row;
                int endRow = dimension.End.Row;
                int startCol = dimension.Start.Column;
                int endCol = dimension.End.Column;

                // Проверяем наличие необходимых колонок
                string[] requiredHeaders = {
                "Артикул", "Эквайринг", "Вознаграждение Ozon, FBS, %",
                "Обработка отправления, максимум FBS", "Логистика Ozon, максимум, FBS",
                "Доставка до места выдачи, FBS"};

                // Находим нужные колонки по заголовкам
                var headers = new Dictionary<string, int>();
                for (int col = startCol; col <= endCol; col++)
                {
                    var headerText = worksheet.Cells[startRow+1, col].Text.Trim();
                    if (requiredHeaders.Contains(headerText))
                    {
                        headers[headerText] = col;
                    }
                }

                foreach (var h in requiredHeaders)
                {
                    if (!headers.ContainsKey(h))
                    {
                        Debug.WriteLine($"Отсутствует необходимая колонка: {h}");
                        return result;
                    }
                }

                int articleCol = headers["Артикул"];
                int acquiringCol = headers["Эквайринг"];
                int rewardCol = headers["Вознаграждение Ozon, FBS, %"];
                int shipmentCol = headers["Обработка отправления, максимум FBS"];
                int logisticsCol = headers["Логистика Ozon, максимум, FBS"];
                int deliveryCol = headers["Доставка до места выдачи, FBS"];

                // Читаем строки
                for (int row = startRow + 4; row <= endRow; row++)
                {
                    var article = worksheet.Cells[row, articleCol].Text;
                    // Обработка числовых значений
                    decimal acquiring = ParseDecimal(worksheet.Cells[row, acquiringCol].Text);
                    decimal reward = ParseDecimal(worksheet.Cells[row, rewardCol].Text);
                    decimal shipment = ParseDecimal(worksheet.Cells[row, shipmentCol].Text);
                    decimal logistics = ParseDecimal(worksheet.Cells[row, logisticsCol].Text);
                    decimal delivery = ParseDecimal(worksheet.Cells[row, deliveryCol].Text);

                    var priceObj = new OzonSellerXlsxPrice
                    {
                        Article = string.IsNullOrWhiteSpace(article) ? null : article,
                        Acquiring = acquiring,
                        OzonRewardFBS = reward,
                        ShipmentProcessingFBS = shipment,
                        OzonLogisticsFBS = logistics,
                        DeliveryPickupPointFBS = delivery
                    };

                    result.Add(priceObj);
                }
            }

            return result;
        }

        private static decimal ParseDecimal(string text)
        {
            if (decimal.TryParse(text, out var val))
                return val;
            return 0m; // Или можно вернуть null, если сделать тип decimal?
        }
    }
}
