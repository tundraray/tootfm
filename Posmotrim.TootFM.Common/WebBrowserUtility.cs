using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace Posmotrim.TootFM.Common
{
    public class WebBrowserUtility
    {
        public static readonly DependencyProperty ShouldHandleNavigatedProperty =
       DependencyProperty.RegisterAttached(
           "ShouldHandleNavigated",
           typeof(Boolean),
           typeof(WebBrowserUtility),
           new PropertyMetadata(false, ShouldHandleNavigatedPropertyChanged));

        public static readonly DependencyProperty BindableSourceProperty =
                           DependencyProperty.RegisterAttached("BindableSource", typeof(string),
                           typeof(WebBrowserUtility), new PropertyMetadata(null,
                           BindableSourcePropertyChanged));


        public static Boolean GetShouldHandleNavigated(DependencyObject obj)
        {
            return (Boolean)obj.GetValue(BindableSourceProperty);
        }

        public static void SetShouldHandleNavigated(DependencyObject obj, Boolean value)
        {
            obj.SetValue(ShouldHandleNavigatedProperty, value);
        }

        public static void ShouldHandleNavigatedPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser browser = o as WebBrowser;
            if (browser != null)
            {
                if ((bool)e.NewValue)
                {
                    browser.Navigated += new EventHandler<NavigationEventArgs>(Browser_Navigated);
    
                   
                }
                else
                {
                    browser.Navigated -= new EventHandler<NavigationEventArgs>(Browser_Navigated);
                   
                }
            }
        }

       


        private const string _SkipSourceChange = "Skip";

        public static void BindableSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser browser = o as WebBrowser;
            if (browser != null)
            {
                string uri = e.NewValue as string;
                if (!_SkipSourceChange.Equals(browser.Tag) )
                {
                    if (!string.IsNullOrEmpty(uri))
                        browser.Navigate(new Uri(uri));
                    else
                    {
                        browser.Source = null;
                    }

                }
            }
        }

        private static void Browser_Navigated(object sender, NavigationEventArgs e)
        {
            WebBrowser browser = sender as WebBrowser;
            if (browser != null)
            {
                if (GetBindableSource(browser) != e.Uri.ToString())
                {
                    browser.Tag = _SkipSourceChange;
                    browser.SetValue(BindableSourceProperty, e.Uri.ToString());
                    browser.Tag = null;
                }
            }
        }

        public static string GetBindableSource(DependencyObject obj)
        {
            return (string)obj.GetValue(BindableSourceProperty);
        }

        public static void  SetBindableSource(DependencyObject obj, object value)
        {
            obj.SetValue(BindableSourceProperty,value);
        }

    }
}