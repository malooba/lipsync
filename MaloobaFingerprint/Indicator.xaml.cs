using System.Windows;
using System.Windows.Controls;

namespace MaloobaFingerprint
{
    /// <summary>
    /// Interaction logic for Indicator.xaml
    /// </summary>
    public partial class Indicator : UserControl
    {
        public static readonly DependencyProperty IsLitProperty = DependencyProperty.Register("IsLit", typeof(bool), typeof(Indicator), new UIPropertyMetadata(true));

        public bool IsLit
        {
            get { return (bool)GetValue(IsLitProperty); }
            set { SetValue(IsLitProperty, value);}
        }

        public Indicator()
        {
            InitializeComponent();
        }
    }
}
