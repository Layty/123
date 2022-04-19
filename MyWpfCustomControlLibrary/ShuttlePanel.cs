using System;
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

namespace MyWpfCustomControlLibrary
{

    public class ShuttlePanel : StackPanel
    {
        Border translateborder = new Border();
        static ShuttlePanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ShuttlePanel), new FrameworkPropertyMetadata(typeof(ShuttlePanel)));
        }

        public ShuttlePanel()
        {
            Children.Add(translateborder);
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }


        public Brush ActiveBrush
        {
            get { return (Brush)GetValue(ActiveBrushProperty); }
            set { SetValue(ActiveBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActiveBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActiveBrushProperty =
            DependencyProperty.Register("ActiveBrush", typeof(Brush), typeof(ShuttlePanel), new PropertyMetadata(Brushes.Aqua));


        protected override Size MeasureOverride(Size constraint)
        {
            if (Children.Count == 1)
            {
                return constraint;
            }

            var cloumWidth = constraint.Width / (Children.Count - 1);
            Size childrenSize = new Size();
            foreach (UIElement child in Children)
            {

                if (child is Border)
                {
                    translateborder.BorderThickness = new Thickness(0, 0, 0, 2);
                   
                    translateborder.Width = cloumWidth;
                    translateborder.Height = constraint.Height;
                    translateborder.BorderBrush = ActiveBrush;
                    continue;
                }
                child.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                childrenSize.Width += child.DesiredSize.Width;
                childrenSize.Height += child.DesiredSize.Height;
            }

            return childrenSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 1)
            {
                return finalSize;
            }
            var cloumWidth = finalSize.Width / (Children.Count - 1);
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    Point childPoint = new Point(0, 0);
                    foreach (UIElement child in InternalChildren)
                    {
                        if (child is Border)
                        {
                            var rect1 = new Rect(childPoint, new Size(cloumWidth, finalSize.Height));
                            child.Arrange(rect1);
                            continue;
                        }
                        var rect = new Rect(childPoint, new Size(cloumWidth, finalSize.Height));

                        child.Arrange(rect);
                        childPoint.X += child.RenderSize.Width;
                    }

                    break;
            }

            return finalSize;
        }
    }
}
