using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KTPM.Views
{
    internal class BaseView<TView, TModel> : System.Mvc.IView
        where TView : FrameworkElement, new()
    {
        public TView MainView { get; private set; } = new TView();
        public TModel Model => (TModel)MainView.DataContext;
        public object Content => MainView;

        public void Render(object model)
        {
            MainView.DataContext = model;
            RenderCore();
        }

        protected virtual void RenderCore() { }
    }
}
