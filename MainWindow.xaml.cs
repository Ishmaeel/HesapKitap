using System;
using System.Windows;

namespace HesapKitap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private StatementParser _LastParser = null;
        private bool ignoreChange = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            if (_LastParser == null)
                return;

            _LastParser.Save();

            buttonSave.IsEnabled = false;
        }

        private void textBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (ignoreChange)
                return;

            try
            {
                ignoreChange = true;

                StatementParser parser = new StatementParser();
                parser.Process(textBox.Text);

                if (parser.Output.Length > 0)
                {
                    textBox.Text = parser.Output;
                    _LastParser = parser;
                    buttonSave.IsEnabled = true;
                }
            }
            catch (Exception)
            {
                //Do naught.
            }
            finally
            {
                ignoreChange = false;
            }
        }
    }
}
