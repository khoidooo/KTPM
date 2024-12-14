using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Input;

namespace Vst.Controls
{
    public class TableColumn
    {
        string _header;
        public string Header 
        {
            get => _header ?? Name; 
            set => _header = value; 
        }
        public double Width { get; set; }
        public string Name { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public Brush Background { get; set; }
        public Brush Foreground { get; set; } = Brushes.Black;
    }
    public class TableColumnCollection : List<TableColumn>
    {
        public TableColumn this[string name] => Find(x => x.Name == name);
    }

    public class TableCell : TextBase
    {
        public int ColumnIndex => (int)GetValue(System.Windows.Controls.Grid.ColumnProperty);
        public TableCell()
        {
            VerticalTextAlignment = VerticalAlignment.Center;
        }
    }
    public class TableRow : Grid
    {
        protected virtual TableCell CreateCell(TableColumn column)
        {
            return new TableCell {
                HorizontalTextAlignment = column.HorizontalAlignment,
                Background = column.Background,
                Foreground = column.Foreground,
            };
        }
        public virtual void SetColumns(TableColumn[] cols)
        {
            Columns.Clear();
            Children.Clear();

            foreach (var col in cols)
            {   
                Children.Add(CreateCell(col));
            }
            Split(0, cols.Length);
            for (int i = 0; i < cols.Length; i++)
            {
                var w = cols[i].Width;
                if (w != 0)
                {
                    Columns[i].Width = new GridLength(w);
                }
            }
        }
        public int RowIndex { get; set; }
    }
    public class TableHeaderCell : TableCell
    {

    }
    public class TableHeader : TableRow
    {
        public TableHeader()
        {
            Background = Brushes.White;
        }
        protected override TableCell CreateCell(TableColumn column) {
            return new TableHeaderCell { 
                Text = column.Header,
                HorizontalTextAlignment = column.HorizontalAlignment
            };
        }
    }

    public class TableView : Border
    {
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.RegisterAttached(nameof(Columns),
            typeof(TableColumnCollection),
            typeof(TableView),
            null);
        public TableColumnCollection Columns
        {
            get { return (TableColumnCollection)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        TableHeader header = new TableHeader();
        StackPanel body = new StackPanel { 
            Background = Brushes.White
        };
        ScrollBar vscroll;


        int visibleRows;
        
        object[] items;
        protected virtual void RaiseOpenOne(int index)
        {
            if (index < items.Length)
            {
                OpenItem?.Invoke(items[index]);
            }
        }
        public event Action<object> OpenItem;
        public System.Collections.IEnumerable ItemsSource
        {
            get => items;
            set
            {
                items = null;
                if (value is Array)
                {
                    items = (object[])value;
                }
                else
                {
                    var lst = value as System.Collections.ICollection;
                    if (lst != null)
                    {
                        items = new object[lst.Count];
                        int i = 0;
                        foreach (var e in value) items[i++] = e;
                    }
                }
                
                vscroll.Maximum = 0;
                if (items != null) vscroll.Maximum = items.Length - 1;

                InvalidateVisual();
            }
        }

        double iHeight = 30;
        public double ItemHeight
        {
            get => iHeight;
            set
            {
                iHeight = value;
                InvalidateVisual();
            }
        }

        public int FirstRow
        {
            get => (int)vscroll.Value;
            set => vscroll.Value = value < 0 ? 0 : (value >= vscroll.Maximum ? (int)vscroll.Maximum - 1 : value);
        }
        void Measure(double height)
        {
            visibleRows = (int)(height / iHeight);
            body.Children.Clear();

            var cols = Columns.ToArray();
            for (int i = 0; i < visibleRows; i++)
            {
                var iv = new TableRow {
                    Height = iHeight,
                };
                iv.SetColumns(cols);
                body.Children.Add(iv);
            }

            header.SetColumns(cols);

            if (items != null)
            {
                vscroll.Maximum = items.Length - visibleRows;
            }
            else
            {
                vscroll.Maximum = 0;
            }
        }
        void Render()
        {
            if (items == null) return;
            var k = FirstRow;
            var cols = Columns.ToArray();

            foreach (TableRow r in body.Children)
            {
                if (k < items.Length)
                {
                    var doc = Document.FromObject(items[k]);
                    r.RowIndex = k++;
                    foreach (TableCell e in r.Children)
                    {
                        var name = cols[e.ColumnIndex].Name;
                        e.Text = name == null ? null : doc.GetString(name);
                    }
                }
                else
                {
                    foreach (TableCell e in r.Children)
                        e.Text = null;
                }
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            Measure(body.ActualHeight);
            Render();
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }
        public TableView()
        {
            Columns = new TableColumnCollection();

            var grid = new Grid { 
                Children = { header, body },
            };
            grid.Split(2, 1);
            grid.Rows[0].Height = new GridLength(iHeight);

            vscroll = new ScrollBar { 
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            vscroll.ValueChanged += (s, e) => Render();
            grid.Add(vscroll, 1, 0);

            Child = grid;
            Focusable = true;

            PreviewMouseDown += (s, e) => {
                Focus();
            };

            body.PreviewMouseLeftButtonDown += (s, e) => {
                int index = FirstRow + (int)(e.GetPosition(body).Y / iHeight);
                if (e.ClickCount >= 2)
                {
                    RaiseOpenOne(index);
                }
            };
            body.MouseWheel += (s, e) => {
                var d = e.Delta;
                FirstRow -= (int)(d / iHeight);
            };
            Application.Current.MainWindow.PreviewKeyDown += (s, e) => {
                if (!IsFocused) return;

                switch (e.Key)
                {
                    case Key.Home: FirstRow = 0; return;
                    case Key.End: FirstRow = (int)vscroll.Maximum; return;
                    case Key.PageDown: FirstRow += visibleRows; return;
                    case Key.PageUp: FirstRow -= visibleRows; return;
                    case Key.Down: ++FirstRow; return;
                    case Key.Up: --FirstRow; return;
                }
            };
        }
    }
}
