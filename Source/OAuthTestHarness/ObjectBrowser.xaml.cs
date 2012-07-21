using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace OAuthTestHarness
{
    public partial class ObjectBrowser : UserControl
    {

        public ObjectBrowser()
        {
            InitializeComponent();
        }

        private Stack<object> _backStack = new Stack<object>();

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                _backStack.Push(this.DataContext);
                CanBack = true;
            }
            PropertyBinder p = ((HyperlinkButton)sender).DataContext as PropertyBinder;
            if (p != null)
                this.DataContext = p.Value;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_backStack.Count > 0)
            {
                DataContext = _backStack.Pop();
                CanBack = _backStack.Count > 0;
            }
        }

        /// <summary>
        /// The <see cref="CanBack" /> dependency property's name.
        /// </summary>
        public const string CanBackPropertyName = "CanBack";

        /// <summary>
        /// Gets or sets the value of the <see cref="CanBack" />
        /// property. This is a dependency property.
        /// </summary>
        public bool CanBack
        {
            get
            {
                return (bool)GetValue(CanBackProperty);
            }
            set
            {
                SetValue(CanBackProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="CanBack" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanBackProperty = DependencyProperty.Register(
            CanBackPropertyName,
            typeof(bool),
            typeof(ObjectBrowser),
            new PropertyMetadata(false));
    }
}
