using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Transmision.Net.Basico
{
    public class Ticket
    {
        private ArrayList headerLines = new ArrayList();
        private ArrayList subHeaderLines = new ArrayList();
        private ArrayList items = new ArrayList();
        private ArrayList totales = new ArrayList();
        private ArrayList footerLines = new ArrayList();
        private ArrayList footerLines0 = new ArrayList();
        private int maxChar = 35;
        private int maxCharDescription = 20;
        private float topMargin = 3f;
        private string fontName = "Lucida Console";
        private double fontSize = 9;
        private SolidBrush myBrush = new SolidBrush(Color.Black);
        private Image headerImage;
        private int count;
        private int imageHeight;
        public float leftMargin ;
        private Font printFont;
        private Graphics gfx;
        private string line;

        public Image HeaderImage
        {
            get
            {
                return this.headerImage;
            }
            set
            {
                if (this.headerImage == value)
                    return;
                this.headerImage = value;
            }
        }

        public int MaxChar
        {
            get
            {
                return this.maxChar;
            }
            set
            {
                if (value == this.maxChar)
                    return;
                this.maxChar = value;
            }
        }

        public int MaxCharDescription
        {
            get
            {
                return this.maxCharDescription;
            }
            set
            {
                if (value == this.maxCharDescription)
                    return;
                this.maxCharDescription = value;
            }
        }

        public double FontSize
        {
            get
            {
                return this.fontSize;
            }
            set
            {
                if (value == this.fontSize)
                    return;
                this.fontSize = value;
            }
        }

        public string FontName
        {
            get
            {
                return this.fontName;
            }
            set
            {
                if (!(value != this.fontName))
                    return;
                this.fontName = value;
            }
        }

        public void AddHeaderLine(string line)
        {
            this.headerLines.Add((object)line);
        }

        public void AddSubHeaderLine(string line)
        {
            this.subHeaderLines.Add((object)line);
        }

        public void AddItem(string cantidad, string item, string price)
        {
            //this.items.Add((object)new OrderItem('?').GenerateItem(cantidad, item, price));
        }

        public void AddTotal(string name, string price)
        {
            //this.totales.Add((object)new OrderTotal('?').GenerateTotal(name, price));
        }

        public void AddFooterLine(string line)
        {
            this.footerLines.Add((object)line);
        }

        public void AddFooterLine0(string line)
        {
            this.footerLines0.Add((object)line);
        }

        private string AlignRightText(int lenght)
        {
            string str = "";
            int num = this.maxChar - lenght;
            for (int index = 0; index < num; ++index)
                str += " ";
            return str;
        }

        private string CenterText(string text , ref string sobrante)
        {
            Console.WriteLine(text);
            text = text.TrimStart();
            sobrante = "";
            int len = text.Length;
            int lastSpacePos = text.LastIndexOf(" ");
            if (lastSpacePos > 0 && lastSpacePos != len-1)
            {
                sobrante = text.Substring(lastSpacePos + 1);
                return text.Substring(0, lastSpacePos + 1);
            }else
            {
                return text;
            }
                
        }


        private string DottedLine()
        {
            string str = "";
            for (int index = 0; index < this.maxChar; ++index)
                str += "=";
            return str;
        }

        public bool PrinterExists(string impresora)
        {
            foreach (string installedPrinter in PrinterSettings.InstalledPrinters)
            {
                if (impresora == installedPrinter)
                    return true;
            }
            return false;
        }

        public void PrintTicket(string impresora)
        {
            this.printFont = new Font(this.fontName, (float)this.fontSize, FontStyle.Regular);
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrinterSettings.PrinterName = impresora;
            printDocument.PrintPage += new PrintPageEventHandler(this.pr_PrintPage);
            printDocument.Print();
        }

        private void pr_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.PageUnit = GraphicsUnit.Millimeter;
            this.gfx = e.Graphics;

            this.DrawHeader();
            //this.DrawSubHeader();


            //this.DrawItems();
            //this.DrawTotales();
            this.DrawFooter();
            this.DrawImage();
            this.DrawFooter0();
            
            
            if (this.headerImage == null)
                return;
            this.HeaderImage.Dispose();
            this.headerImage.Dispose();
        }

        private float YPosition()
        {
            return this.topMargin + ((float)this.count * this.printFont.GetHeight(this.gfx) + (float)this.imageHeight);
        }

        private void DrawImage()
        {
            if (this.headerImage == null)
                return;
            try
            {
                this.gfx.DrawImage(this.headerImage, new Point((int)this.leftMargin + 4, (int)this.YPosition()));/*posicion para el codigo de barra*/


                //this.gfx.DrawImage(this.headerImage, new Point((int)this.leftMargin + 20, (int)this.YPosition()));
                this.imageHeight = 5;// (int)Math.Round((double)this.headerImage.Height / 58.0 * 15.0) + 3;
                this.DrawEspacio();
            }
            catch (Exception ex)
            {
            }
        }

        //private void DrawHeader()
        //{
        //    Font printFont = new Font(this.fontName, (float)(this.fontSize - 0.3), FontStyle.Bold);
        //    foreach (string headerLine in this.headerLines)
        //    {
        //        if (headerLine.Length > this.maxChar)
        //        {
        //            int startIndex = 0;
        //            int length = headerLine.Length;
        //            while (length > this.maxChar)
        //            {
        //                this.line = headerLine.Substring(startIndex, this.maxChar);
        //                this.gfx.DrawString(this.line, this.printFont, (Brush)this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
        //                ++this.count;
        //                startIndex += this.maxChar;
        //                length -= this.maxChar;
        //            }
        //            this.line = headerLine;
        //            this.gfx.DrawString(this.line.Substring(startIndex, this.line.Length - startIndex), this.printFont, (Brush)this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
        //            ++this.count;
        //        }
        //        else
        //        {
        //            this.line = headerLine;
        //            this.gfx.DrawString(this.line, this.printFont, (Brush)this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
        //            ++this.count;
        //        }
        //    }
        //    //this.DrawEspacio();
        //}
        private void DrawHeader()
        {
            Font printFont = new Font(this.fontName, (float)(this.fontSize - 0.3), FontStyle.Bold);


            foreach (string header in this.headerLines)
            {
                if (header.Length > this.maxChar)
                {
                    int currentChar = 0;
                    for (int headerLenght = header.Length; headerLenght > this.maxChar; headerLenght -= this.maxChar)
                    {
                        this.line = AlignCenterText(header.Substring(currentChar, this.maxChar).Length) + header.Substring(currentChar, this.maxChar);
                        this.gfx.DrawString(this.line, printFont, this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
                        this.count++;
                        currentChar += this.maxChar;
                    }
                    this.line = AlignCenterText(header.Length) + header;
                    this.gfx.DrawString(this.line.Substring(currentChar, this.line.Length - currentChar), printFont, this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
                    this.count++;
                }
                else
                {
                    this.line = AlignCenterText(header.Length) + header;
                    this.gfx.DrawString(this.line, printFont, this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
                    this.count++;
                }
            }
            // this.DrawEspacio();
        }
        private void DrawFooter0()
        {
            Font printFont = new Font(this.fontName, (float)(this.fontSize - 0.3), FontStyle.Bold);

            this.maxChar -= 7;

            foreach (string header in this.footerLines0)
            {
                if (header.Length > this.maxChar)
                {
                    int currentChar = 0;
                    for (int headerLenght = header.Length; headerLenght > this.maxChar; headerLenght -= this.maxChar)
                    {
                        this.line = AlignCenterText(header.Substring(currentChar, this.maxChar).Length) + header.Substring(currentChar, this.maxChar);
                        this.gfx.DrawString(this.line, printFont, this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
                        this.count++;
                        currentChar += this.maxChar;
                    }
                    this.line = AlignCenterText(header.Length) + header;
                    this.gfx.DrawString(this.line.Substring(currentChar, this.line.Length - currentChar), printFont, this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
                    this.count++;
                }
                else
                {
                    this.line = AlignCenterText(header.Length) + header;
                    this.gfx.DrawString(this.line, printFont, this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
                    this.count++;
                }
            }
            this.DrawEspacio();
        }
        private string AlignCenterText(int lenght)
        {
            string espacios = "";
            int spaces = (this.maxChar - lenght) / 2;
            for (int x = 0; x < spaces; x++)
            {
                espacios += " ";
            }
            return espacios;
        }
        private void DrawSubHeader()
        {
            foreach (string subHeaderLine in this.subHeaderLines)
            {
                if (subHeaderLine.Length > this.maxChar)
                {
                    int startIndex = 0;
                    int length = subHeaderLine.Length;
                    while (length > this.maxChar)
                    {
                        this.line = subHeaderLine;
                        this.gfx.DrawString(this.line.Substring(startIndex, this.maxChar), this.printFont, (Brush)this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
                        ++this.count;
                        startIndex += this.maxChar;
                        length -= this.maxChar;
                    }
                    this.line = subHeaderLine;
                    this.gfx.DrawString(this.line.Substring(startIndex, this.line.Length - startIndex), this.printFont, (Brush)this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
                    ++this.count;
                }
                else
                {
                    this.line = subHeaderLine;
                    this.gfx.DrawString(this.line, this.printFont, (Brush)this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
                    ++this.count;
                    this.line = this.DottedLine();
                    this.gfx.DrawString(this.line, this.printFont, (Brush)this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
                    ++this.count;
                }
            }
            this.DrawEspacio();
        }

        private void DrawItems()
        {
            //OrderItem orderItem1 = new OrderItem('?');
            //this.gfx.DrawString("CANT  DESCRIPCION           IMPORTE", this.printFont, (Brush)this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
            //++this.count;
            //this.DrawEspacio();
            //foreach (string orderItem2 in this.items)
            //{
            //    this.line = orderItem1.GetItemCantidad(orderItem2);
            //    this.gfx.DrawString(this.line, this.printFont, (Brush)this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
            //    this.line = orderItem1.GetItemPrice(orderItem2);
            //    this.line = this.AlignRightText(this.line.Length) + this.line;
            //    this.gfx.DrawString(this.line, this.printFont, (Brush)this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
            //    string itemName = orderItem1.GetItemName(orderItem2);
            //    this.leftMargin = 0.0f;
            //    if (itemName.Length > this.maxCharDescription)
            //    {
            //        int startIndex = 0;
            //        int length = itemName.Length;
            //        while (length > this.maxCharDescription)
            //        {
            //            this.line = orderItem1.GetItemName(orderItem2);
            //            this.gfx.DrawString("      " + this.line.Substring(startIndex, this.maxCharDescription), this.printFont, (Brush)this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
            //            ++this.count;
            //            startIndex += this.maxCharDescription;
            //            length -= this.maxCharDescription;
            //        }
            //        this.line = orderItem1.GetItemName(orderItem2);
            //        this.gfx.DrawString("      " + this.line.Substring(startIndex, this.line.Length - startIndex), this.printFont, (Brush)this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
            //        ++this.count;
            //    }
            //    else
            //    {
            //        this.gfx.DrawString("      " + orderItem1.GetItemName(orderItem2), this.printFont, (Brush)this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
            //        ++this.count;
            //    }
            //}
            //this.leftMargin = 0.0f;
            //this.DrawEspacio();
            //this.line = this.DottedLine();
            //this.gfx.DrawString(this.line, this.printFont, (Brush)this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
            //++this.count;
            //this.DrawEspacio();
        }

        private void DrawTotales()
        {
            //OrderTotal orderTotal = new OrderTotal('?');
            //foreach (string totale in this.totales)
            //{
            //    this.line = orderTotal.GetTotalCantidad(totale);
            //    this.line = this.AlignRightText(this.line.Length) + this.line;
            //    this.gfx.DrawString(this.line, this.printFont, (Brush)this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
            //    this.leftMargin = 0.0f;
            //    this.line = "      " + orderTotal.GetTotalName(totale);
            //    this.gfx.DrawString(this.line, this.printFont, (Brush)this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
            //    ++this.count;
            //}
            //this.leftMargin = 0.0f;
            //this.DrawEspacio();
            //this.DrawEspacio();
        }

        private void DrawFooter()
        {
            this.printFont = new Font(this.fontName, (float)(this.fontSize - 1.5), FontStyle.Regular);
            this.maxChar += 7;
            string sobrante = "";
            foreach (string footerLine in this.footerLines)
            {
                //string footerLine = sobrante + " " + footerLine1;
                if (footerLine.Length > this.maxChar)
                {
                    int startIndex = 0;
                    int length = footerLine.Length;
                    while (length > this.maxChar)
                    {
                        sobrante = "";
                        this.line = footerLine;
                        this.gfx.DrawString(CenterText(this.line.Substring(startIndex, this.maxChar), ref sobrante), this.printFont, (Brush)this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
                        ++this.count;
                        startIndex += this.maxChar - (sobrante.Length);
                        length -= this.maxChar;
                        length += (sobrante.Length);
                        //this.line = sobrante + " " + this.line;
                    }
                    this.line = footerLine;
                    this.gfx.DrawString(this.line.Substring(startIndex, this.line.Length - startIndex), this.printFont, (Brush)this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
                    ++this.count;
                }
                else
                {
                    this.line = footerLine;
                    this.gfx.DrawString(this.line, this.printFont, (Brush)this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
                    ++this.count;
                }
            }
            this.leftMargin = 0.0f;
            this.DrawEspacio();
        }

        private void DrawEspacio()
        {
            this.line = "";
            this.gfx.DrawString(this.line, this.printFont, (Brush)this.myBrush, this.leftMargin, this.YPosition(), new StringFormat());
            ++this.count;
        }
    }
    //Clase para mandara a imprimir texto plano a la impresora
    public class RawPrinterHelper
    {
        // Structure and API declarions:
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        // SendBytesToPrinter()
        // When the function is given a printer name and an unmanaged array
        // of bytes, the function sends those bytes to the print queue.
        // Returns true on success, false on failure.
        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false; // Assume failure unless you specifically succeed.

            di.pDocName = "Ticket de Venta";//Este es el nombre con el que guarda el archivo en caso de no imprimir a la impresora fisica.
            di.pDataType = "RAW";//de tipo texto plano

            // Open the printer.
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Start a document.
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    // Start a page.
                    if (StartPagePrinter(hPrinter))
                    {
                        // Write your bytes.
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information
            // about why not.
            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }

        public static bool SendStringToPrinter(string szPrinterName, string szString)
        {
            IntPtr pBytes;
            Int32 dwCount;
            // How many characters are in the string?
            dwCount = szString.Length;
            // Assume that the printer is expecting ANSI text, and then convert
            // the string to ANSI text.
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            // Send the converted ANSI string to the printer.
            Boolean _valida = SendBytesToPrinter(szPrinterName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return _valida;
        }
    }
}
