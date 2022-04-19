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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyWpfCustomControlLibrary
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:MyWpfCustomControlLibrary"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:MyWpfCustomControlLibrary;assembly=MyWpfCustomControlLibrary"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    ///
    [TemplateVisualState(GroupName = "ActiveStates", Name = "Active")]
    [TemplateVisualState(GroupName = "ActiveStates", Name = "Inactive")]
    public class LightControl : Control
    {
        static LightControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LightControl), new FrameworkPropertyMetadata(typeof(LightControl)));
        }

       
   
      
        public TextBlock Icon
        {
            get { return (TextBlock)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(TextBlock), typeof(LightControl), new PropertyMetadata(null));
        public Brush FromColor
        {
            get { return (Brush)GetValue(FromColorProperty); }
            set { SetValue(FromColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FromColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FromColorProperty =
            DependencyProperty.Register("FromColor", typeof(Brush), typeof(LightControl),
                new PropertyMetadata(Brushes.Red));


        public Brush ToColor
        {
            get { return (Brush)GetValue(ToColorProperty); }
            set { SetValue(ToColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToColorProperty =
            DependencyProperty.Register("ToColor", typeof(Brush), typeof(LightControl),
                new PropertyMetadata(Brushes.WhiteSmoke));

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(LightControl), new PropertyMetadata(false, OnIsActivePropertyChangedCallBack));

        private static void OnIsActivePropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is LightControl customLight))
            {
                return;
            }
            var isActive = (bool)e.NewValue;
            customLight.UpdateState(isActive);
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _hasAppliedTemplate = true;
            UpdateState(IsActive);
        }
        private bool _hasAppliedTemplate = false;
        private void UpdateState(bool isActive)
        {
            if (_hasAppliedTemplate)
            {
                var state = isActive ? "Active" : "Inactive";
                VisualStateManager.GoToState(this, state, true);
            }
        }

    }
}
