using ICMD.Core.Constants;
using ICMD.Core.Dtos.Project;

using System.Text;
using OfficeOpenXml;

namespace ICMD.API.Helpers
{
    public class CSVImport
    {
        private static FileType ValidateFile(IEnumerable<string> inputHeadings)
        {
            List<string> headings = new List<string>();
            if (inputHeadings.First() == FileHeadingConstants.IdHeading)
            {
                // Remove Id Header
                headings.AddRange(inputHeadings.Skip(1));
            }
            else
            {
                headings.AddRange(inputHeadings);
            }

            if (headings.SequenceEqual(FileHeadingConstants.OMItemsHeadings))
                return FileType.OMItems;
            else if (headings.SequenceEqual(FileHeadingConstants.OMServiceDescriptionHeadings))
                return FileType.OMServiceDescriptions;
            else if (headings.SequenceEqual(FileHeadingConstants.EquipmentListHeadings))
                return FileType.EquipmentList;
            else if (headings.SequenceEqual(FileHeadingConstants.InstrumentListHeadings))
                return FileType.InstrumentList;
            else if (headings.SequenceEqual(FileHeadingConstants.ValveListHeadings))
                return FileType.ValveList;
            else if (headings.SequenceEqual(FileHeadingConstants.CCMDHeadings))
                return FileType.CCMD;

            else if (headings.SequenceEqual(FileHeadingConstants.BankListHeadings))
                return FileType.Bank;
            else if (headings.SequenceEqual(FileHeadingConstants.WorkAreaPackHeadings))
                return FileType.WorkAreaPack;
            else if (headings.SequenceEqual(FileHeadingConstants.TrainHeadings))
                return FileType.Train;
            else if (headings.SequenceEqual(FileHeadingConstants.ZoneHeadings))
                return FileType.Zone;
            else if (headings.SequenceEqual(FileHeadingConstants.SystemHeadings))
                return FileType.System;
            else if (headings.SequenceEqual(FileHeadingConstants.SubSystemHeadings) ||
                headings.SequenceEqual(FileHeadingConstants.SubSystemExportHeadings))
                return FileType.SubSystem;
            else if (headings.SequenceEqual(FileHeadingConstants.TagField1Headings))
                return FileType.TagField1;
            else if (headings.SequenceEqual(FileHeadingConstants.TagField2Headings))
                return FileType.TagField2;
            else if (headings.SequenceEqual(FileHeadingConstants.TagField3Headings))
                return FileType.TagField3;
            else if (headings.SequenceEqual(FileHeadingConstants.ReferenceDocumentHeadings) ||
                     headings.SequenceEqual(FileHeadingConstants.ReferenceDocumentExportHeadings))
                return FileType.ReferenceDocument;
            else if (headings.SequenceEqual(FileHeadingConstants.TagHeadings))
                return FileType.Tags;
            else if (headings.SequenceEqual(FileHeadingConstants.JunctionBoxHeadings) ||
                     headings.SequenceEqual(FileHeadingConstants.JunctionBoxExportHeadings))
                return FileType.JunctionBox;
            else if (headings.SequenceEqual(FileHeadingConstants.PanelHeadings) ||
                     headings.SequenceEqual(FileHeadingConstants.PanelExportHeadings))
                return FileType.Panel;
            else if (headings.SequenceEqual(FileHeadingConstants.SkidHeadings) ||
                     headings.SequenceEqual(FileHeadingConstants.SkidExportHeadings))
                return FileType.Skid;
            else if (headings.SequenceEqual(FileHeadingConstants.StandHeadings) ||
                     headings.SequenceEqual(FileHeadingConstants.StandExportHeadings))
                return FileType.Stand;
            else if (headings.SequenceEqual(FileHeadingConstants.ReferenceDocumentTypeHeadings))
                return FileType.ReferenceDocumentType;
            else if (headings.SequenceEqual(FileHeadingConstants.EquipmentCodeHeadings))
                return FileType.EquipmentCode;
            else if (headings.SequenceEqual(FileHeadingConstants.FailStateHeadings))
                return FileType.FailState;
            else if (headings.SequenceEqual(FileHeadingConstants.TagTypeHeadings))
                return FileType.TagType;
            else if (headings.SequenceEqual(FileHeadingConstants.TagDescriptorHeadings))
                return FileType.TagDescriptor;
            else if (headings.SequenceEqual(FileHeadingConstants.ManufacturerHeadings))
                return FileType.Manufacturer;
            else if (headings.SequenceEqual(FileHeadingConstants.DeviceModelHeadings))
                return FileType.DeviceModel;
            else if (headings.SequenceEqual(FileHeadingConstants.DeviceTypeHeadings))
                return FileType.DeviceType;
            else if (headings.SequenceEqual(FileHeadingConstants.NatureOfSignalTypeHeadings))
                return FileType.NatureOfSignals;

            else
                return FileType.Invalid;
        }

        public List<Dictionary<string, string>>? ReadFile(IFormFile file, out FileType fileType, bool skipHeaderCheck = false)
        {
            // Check that the file is a CSV file
            if (!file.FileName.EndsWith(".csv") && !file.FileName.EndsWith(".xlsx"))
            {
                fileType = FileType.Invalid;
                return null;
            }

            var strings = new List<Dictionary<string, string>>();

            if (file.FileName.EndsWith(".csv"))
            {
                strings = ReadCsvFile(file, out fileType, skipHeaderCheck);
            }
            else if (file.FileName.EndsWith(".xlsx"))
            {
                using var stream = new MemoryStream();
                file.CopyTo(stream);

                ExcelPackage.LicenseContext = LicenseContext.Commercial;
                using var package = new ExcelPackage(stream);

                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    fileType = FileType.Invalid;
                    return null;
                }
                var rowCount = worksheet.Dimension.Rows;
                var colCount = worksheet.Dimension.Columns;

                var header = worksheet.Cells[string.Format("{0}:{0}", 1)];
                var headings = header.Select(cell => cell.Text).ToList();

                if (!skipHeaderCheck)
                {
                    fileType = ValidateFile(headings);
                    if (fileType == FileType.Invalid)
                        return null;
                }
                else
                {
                    fileType = FileType.Invalid;
                }

                for (int row = 2; row <= rowCount; row++)
                {
                    var dictionary = new Dictionary<string, string>();
                    for (int col = 1; col <= colCount; col++)
                    {
                        var headerKey = (worksheet.Cells[1, col].Value.ToString() ?? string.Empty).Trim();
                        var value = (worksheet.Cells[row, col].Value?.ToString() ?? string.Empty).Trim();

                        if (string.IsNullOrEmpty(headerKey) && string.IsNullOrEmpty(value))
                            continue;

                        dictionary.Add(headerKey, value);
                    }

                    strings.Add(dictionary);
                }
            }
            else
            {
                fileType = FileType.Invalid;
                return null;
            }

            return strings;
        }

        public List<Dictionary<string, string>>? ReadCsvFile(IFormFile file, out FileType fileType, bool skipHeaderCheck = false)
        {
            var strings = new List<Dictionary<string, string>>();

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                // Validate the file headings
                var headings = reader.ReadLine()?.Split(',').Select(s => s.Trim('"'));
                fileType = ValidateFile(headings);

                if (!skipHeaderCheck)
                {
                    fileType = ValidateFile(headings);
                    if (fileType == FileType.Invalid)
                        return null;
                }
                else
                    fileType = FileType.Invalid;

                var inQuotes = false;
                var index = 0;
                var dictionary = new Dictionary<string, string>();
                var value = new StringBuilder();

                //while (reader.Peek() >= 0)
                //{
                //    var character = (char)reader.Read();

                //    // Toggle character flag if the current tag is in quotes
                //    if (character == '"')
                //        inQuotes = !inQuotes;
                //    else if (character == ',' && !inQuotes)
                //    {
                //        // Separate value out and add it to the dictionary
                //        dictionary.Add(headings.ElementAt(index), value.ToString().Trim());
                //        value = new StringBuilder();
                //        index++;
                //    }
                //    else if (character == '\r') // If character is carriage return
                //    {
                //        // Consume the following newline character if present
                //        if (reader.Peek() == '\n')
                //            reader.Read();

                //        // Add value to dictionary
                //        dictionary.Add(headings.ElementAt(index), value.ToString().Trim());
                //        strings.Add(dictionary);

                //        // Reinitialize values for new records
                //        value = new StringBuilder();
                //        dictionary = new Dictionary<string, string>();
                //        index = 0;
                //    }
                //    else
                //        value.Append(character); // Append character to the current value
                //}
                while (reader.Peek() >= 0)
                {
                    var character = (char)reader.Read();

                    // Toggle character flag if the current tag is in quotes
                    if (character == '"')
                        inQuotes = !inQuotes;
                    else if (character == ',' && !inQuotes)
                    {
                        // Separate value out and add it to the dictionary
                        dictionary.Add(headings.ElementAt(index), value.ToString().Trim());
                        value = new StringBuilder();
                        index++;
                    }
                    else if (character == '\r' || character == '\n') // If character is carriage return or newline
                    {
                        // Consume the following newline character if present
                        if (character == '\r' && reader.Peek() == '\n')
                            reader.Read();

                        // Add value to dictionary
                        dictionary.Add(headings.ElementAt(index), value.ToString().Trim());
                        strings.Add(dictionary);

                        // Reinitialize values for new records
                        value = new StringBuilder();
                        dictionary = new Dictionary<string, string>();
                        index = 0;
                    }
                    else
                        value.Append(character); // Append character to the current value
                }

                // Add the last value to the dictionary (outside the loop)
                dictionary.Add(headings.ElementAt(index), value.ToString().Trim());
                strings.Add(dictionary);
            }

            return strings;
        }

        public List<Tuple<string, string, Guid?>[]>? ReadTagFile(
            string tagNameKey,
            IFormFile file,
            List<ProjectTagFieldInfoDto> tagInfo,
            List<string> requiredHeaders,
            out FileType fileType)
        {
            if (file.FileName.EndsWith(".xlsx"))
            {
                using var stream = new MemoryStream();
                file.CopyTo(stream);

                ExcelPackage.LicenseContext = LicenseContext.Commercial;
                using var package = new ExcelPackage(stream);

                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    fileType = FileType.Invalid;
                    return null;
                }
                var rowCount = worksheet.Dimension.Rows;
                var colCount = worksheet.Dimension.Columns;

                var header = worksheet.Cells[string.Format("{0}:{0}", 1)];
                var headings = header.Select(cell => cell.Text).ToList();
                if (headings.FirstOrDefault() == FileHeadingConstants.IdHeading)
                {
                    fileType = headings.Where(x => x != FileHeadingConstants.IdHeading).SequenceEqual(requiredHeaders) ? FileType.Tags : FileType.Invalid;
                }
                else
                {
                    fileType = headings.SequenceEqual(requiredHeaders) ? FileType.Tags : FileType.Invalid;
                }

                if (fileType == FileType.Invalid)
                    return null;

                var tuplesList = new List<Tuple<string, string, Guid?>[]>();

                for (int row = 2; row <= rowCount; row++)
                {
                    List<Tuple<string, string, Guid?>> tupleArray = [];
                    for (int col = 1; col <= colCount; col++)
                    {
                        var headerKey = (worksheet.Cells[1, col].Value.ToString() ?? string.Empty).Trim();
                        var value = (worksheet.Cells[row, col].Value?.ToString() ?? string.Empty).Trim();

                        Guid? tagFieldInfoId = headerKey != tagNameKey ? tagInfo.FirstOrDefault(x => x.Name == headerKey)?.Id ?? null : null;

                        if (string.IsNullOrEmpty(headerKey) && string.IsNullOrEmpty(value))
                            continue;

                        tupleArray.Add(Tuple.Create(headerKey, value, tagFieldInfoId));
                    }

                    if (tupleArray.ToArray().Length != 0)
                        tuplesList.Add(tupleArray.ToArray());
                }

                return tuplesList;
            }
            else
            {
                if (!file.FileName.EndsWith(".csv"))
                {
                    fileType = FileType.Invalid;
                    return null;
                }

                var tuplesList = new List<Tuple<string, string, Guid?>[]>();

                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    // Validate the file headings
                    var headings = reader.ReadLine()?.Split(',').Select(s => s.Trim('"'));
                    fileType = headings.SequenceEqual(requiredHeaders) ? FileType.Tags : FileType.Invalid;

                    if (fileType == FileType.Invalid)
                        return null;

                    var inQuotes = false;
                    var index = 0;
                    List<Tuple<string, string, Guid?>> tupleArray = new();
                    var value = new StringBuilder();

                    while (reader.Peek() >= 0)
                    {
                        var character = (char)reader.Read();

                        Guid? tagFieldInfoId = headings.ElementAt(index) != tagNameKey ? tagInfo.ElementAt(index - 1)?.Id ?? null : null;
                        // Toggle character flag if the current tag is in quotes
                        if (character == '"')
                            inQuotes = !inQuotes;
                        else if (character == ',' && !inQuotes)
                        {
                            // Add value to tuple list
                            tupleArray.Add(Tuple.Create(headings.ElementAt(index), value.ToString().Trim(), tagFieldInfoId));
                            value = new StringBuilder();
                            index++;
                        }
                        else if (character == '\r' || character == '\n') // If character is carriage return or newline
                        {
                            // Consume the following newline character if present
                            if (character == '\r' && reader.Peek() == '\n')
                                reader.Read();

                            // Add value to tuple list
                            tupleArray.Add(Tuple.Create(headings.ElementAt(index), value.ToString().Trim(), tagFieldInfoId));
                            tuplesList.Add(tupleArray.ToArray());

                            // Reinitialize values for new records
                            value = new StringBuilder();
                            tupleArray = new List<Tuple<string, string, Guid?>>();
                            index = 0;
                        }
                        else
                            value.Append(character); // Append character to the current value
                    }

                    if (tupleArray.ToArray().Length != 0)
                        tuplesList.Add(tupleArray.ToArray());
                }
                return tuplesList;
            }
        }
    }
}
