using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KTPM.Views.Home
{
    /// <summary>
    /// Interaction logic for TrangChuLayout.xaml
    /// </summary>
    public partial class TrangChuLayout : UserControl
    {
        public TrangChuLayout()
        {
            InitializeComponent();

            this.DataContextChanged += (s, e) => {
                MainDataGrid.ItemsSource = (IList)DataContext;
            };
        }
    }

    class Index : BaseView<TrangChuLayout, object> 
    {
        protected override void RenderCore()
        {
            base.RenderCore();
        }
    }
}
