using System.Windows;
using System.Windows.Controls;

namespace MaloobaLipSync
{
    /// <summary>
    /// Interaction logic for IndicatorLabel.xaml
    /// </summary>
    public partial class IndicatorLabel : UserControl
    {
        public static readonly DependencyProperty IsLitProperty = DependencyProperty.Register("IsLit", typeof(bool), typeof(IndicatorLabel), new UIPropertyMetadata(true));

        public bool IsLit
        {
            get { return (bool)GetValue(IsLitProperty); }
            set { SetValue(IsLitProperty, value); }
        }

        public IndicatorLabel()
        {
            InitializeComponent();
        }
    }
}
