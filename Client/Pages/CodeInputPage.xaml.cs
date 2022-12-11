using Client.GameService;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Client.Pages {
    /// <summary>
    /// Interaction logic for CodeInputPage.xaml
    /// </summary>
    public partial class CodeInputPage : Page {
        private Action<string> Callback { get; set; }

        public CodeInputPage(string label, string message, Action<string> callback) {
            InitializeComponent();
            HideMessage();
            Callback = callback;
            CodeLabel.Content = label;
            Message.Text = message;
        }

        private void HideMessage() {
            CodeInvalidMessage.Visibility = Visibility.Hidden;
        }
        private bool ValidateCode() {
            bool isInputValid = true;
            if(CodeTextBox.Text.Length == 0) {
                CodeInvalidMessage.Visibility = Visibility.Visible;
                CodeInvalidMessage.Content = Properties.Resources.COMMON_REQUIRED_LABEL;
                isInputValid = false;
            }
            else if(CodeTextBox.Text.Length > 4) {
                CodeInvalidMessage.Visibility = Visibility.Visible;
                CodeInvalidMessage.Content = Properties.Resources.CODE_INPUT_MENU_CODE_TOO_LONG;
                isInputValid = false;
            }
            else if(CodeTextBox.Text.Length < 4) {
                CodeInvalidMessage.Visibility = Visibility.Visible;
                CodeInvalidMessage.Content = Properties.Resources.CODE_INPUT_MENU_CODE_TOO_SHORT;
                isInputValid = false;
            }
            return isInputValid;
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e) {
            if(!ValidateCode()) {
                return;
            }
            var code = CodeTextBox.Text;
            Callback(code);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            App.Current.CloseSecondaryPanel();
        }

        private void RectagleMouseLeft_ButtonUp(object sender, MouseButtonEventArgs e) {
            App.Current.CloseSecondaryPanel();
        }

        public void SetErrorMessage(string text) {
            CodeInvalidMessage.Content = text;
            CodeInvalidMessage.Visibility = Visibility.Visible;
        }

        public void SetMessage(string text) {
            Message.Text = text;
        }
    }
}
