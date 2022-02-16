using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Globalization;

namespace Export.Helper
{
    public class OpenXMLHelper
    {
        public const uint FormattedDateTimeId = 165U;
        public const uint FormattedNormalId = 166U;
        public const uint FormattedBoldUnderlineId = 167U;

        /// <summary>
        /// This is the most important part of how to format cells of the spreadsheep
        /// </summary>
        /// <returns>Stylesheet</returns>
        public Stylesheet GenerateNewStylesheet()
        {
            Stylesheet newStyleSheet = new Stylesheet();

            // Create "fonts" node.
            Fonts fonts = new Fonts();
            fonts.Append(new Font());  // Default font (0)
            fonts.Append(new Font()    // Bold and underlined font
            {
                Underline = new Underline()
                {
                    Val = UnderlineValues.Single
                },
                Bold = new Bold()
                {
                    Val = true
                }
            });
            newStyleSheet.Append(fonts);

            // Create NumberingFormats node
            DateTimeFormatInfo dtfi = DateTimeFormatInfo.CurrentInfo;
            string sortableDateTimeFormat = dtfi.SortableDateTimePattern.Replace("'T'", " ");   // remove T
            string sortableDateTimeFormat2 = sortableDateTimeFormat.Replace("'", string.Empty); // remove all single quotes

            NumberingFormats numberingFormats = new NumberingFormats();
            numberingFormats.Append(new NumberingFormat()); // Default format
            numberingFormats.Append(new NumberingFormat()   // DateTime format
            {
                NumberFormatId = FormattedDateTimeId,
                FormatCode = sortableDateTimeFormat2
            });
            numberingFormats.Count = (uint)numberingFormats.ChildElements.Count;
            newStyleSheet.Append(numberingFormats);

            // Create CellFormats node
            CellFormats cellFormats = new CellFormats();
            cellFormats.Append(new CellFormat()        // normal - 0
            {
                FontId = 0,
            });
            cellFormats.Append(new CellFormat()        // underline + bold - 1
            {
                FontId = 1,
            });
            cellFormats.Append(new CellFormat()        // date format - 2
            {
                FontId = 0,
                NumberFormatId = FormattedDateTimeId,
                ApplyNumberFormat = true
            });

            cellFormats.Count = (uint)cellFormats.ChildElements.Count;
            newStyleSheet.Append(cellFormats);

            //// Date formats
            ////https://codingjump.com/posts/closedxml-openxml-setting-data-type-for-cell/
            ////https://polymathprogrammer.com/2009/11/09/how-to-create-stylesheet-in-excel-open-xml/
            ////https://stackoverflow.com/questions/6033376/openxml-and-date-format-in-excel-cell
            ////https://stackoverflow.com/questions/4730152/what-indicates-an-office-open-xml-cell-contains-a-date-time-value
            ////https://stackoverflow.com/questions/19034805/how-to-distinguish-inline-numbers-from-ole-automation-date-numbers-in-openxml-sp/19582685#19582685

            // Create CellStyleFormats node
            CellStyleFormats cellStyleFormats = new CellStyleFormats();
            cellStyleFormats.Append(new CellFormat()   // default - 0
            {
                FontId = 0,
                FormatId = FormattedNormalId
            });
            cellStyleFormats.Append(new CellFormat()   // underline + bold - 1
            {
                FontId = 1,
                FormatId = FormattedBoldUnderlineId
            });
            cellStyleFormats.Append(new CellFormat()   // date format - 2
            {
                FontId = 0,
                FormatId = FormattedDateTimeId
            });

            cellStyleFormats.Count = (uint)cellStyleFormats.ChildElements.Count;
            newStyleSheet.Append(cellStyleFormats);

            // Create "cellStyles" node.
            CellStyles cellStyles = new CellStyles();
            cellStyles.Append(new CellStyle()          // default - 0
            {
                Name = "Normal",
                FormatId = 0,
                BuiltinId = FormattedNormalId,
                Hidden = false
            });
            cellStyles.Append(new CellStyle()          // underline + bold - 1
            {
                Name = "Underline - Bold",
                FormatId = 1,
                BuiltinId = FormattedBoldUnderlineId,
                Hidden = false
            });
            cellStyles.Append(new CellStyle()          // date format - 2
            {
                Name = "Specialized DateTime formats",
                FormatId = 2,
                BuiltinId = FormattedDateTimeId,
                Hidden = false,
            });

            cellStyles.Count = (uint)cellStyles.ChildElements.Count;
            newStyleSheet.Append(cellStyles);

            return newStyleSheet;
        }

        /// <summary>
        /// Add cell - int
        /// </summary>
        /// <param name="value">value to add</param>
        /// <returns>Cell</returns>
        public Cell AddCell(int value)
        {
            return new Cell()
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(CellValues.Number),
            };
        }

        /// <summary>
        /// Add cell - string
        /// </summary>
        /// <param name="value">value to add</param>
        /// <param name="styleNo">style number</param>
        /// <returns>Cell</returns>
        public Cell AddCell(string value, uint? styleNo = null)
        {
            Cell newCell = new Cell()
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(CellValues.String),
            };

            if (styleNo != null)
            {
                newCell.StyleIndex = styleNo;
            }

            return newCell;
        }

        /// <summary>
        /// Add cell - DateTime
        /// </summary>
        /// <param name="value">value to add</param>
        /// <param name="styleNo">style number</param>
        /// <returns>Cell</returns>
        public Cell AddCell(DateTime value, uint? styleNo = null)
        {
            Cell newCell = new Cell()
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(CellValues.Date)
            };

            if (styleNo != null)
            {
                newCell.StyleIndex = styleNo;
            }

            return newCell;
        }

        /// <summary>
        /// Add cell - bool - Using CellValues.Boolean alone doesn't populate values
        /// </summary>
        /// <param name="value">value to add</param>
        /// <param name="styleNo">style number</param>
        /// <returns>Cell</returns>
        public Cell AddCell(bool value, uint? styleNo = null)
        {
            Cell newCell = new Cell()
            {
                CellValue = new CellValue(value ? 1 : 0),
                DataType = new EnumValue<CellValues>(CellValues.Boolean),
            };

            if (styleNo != null)
            {
                newCell.StyleIndex = styleNo;
            }

            return newCell;
        }
    }
}
