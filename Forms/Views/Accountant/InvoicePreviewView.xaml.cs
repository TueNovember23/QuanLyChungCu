using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Forms.Views.Accountant
{
    public partial class InvoicePreviewView : Window
    {
        private readonly FlowDocument _document;

        public InvoicePreviewView(FlowDocument document)
        {
            InitializeComponent();
            _document = document;
            PreviewViewer.Document = document;
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                _document.PageHeight = printDialog.PrintableAreaHeight;
                _document.PageWidth = printDialog.PrintableAreaWidth;
                _document.ColumnWidth = printDialog.PrintableAreaWidth;

                IDocumentPaginatorSource paginatorSource = _document;
                printDialog.PrintDocument(paginatorSource.DocumentPaginator, "Invoice Print");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}