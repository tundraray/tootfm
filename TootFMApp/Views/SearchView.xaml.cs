using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Posmotrim.TootFM.App.ViewModel;

namespace Posmotrim.TootFM.App.Views
{
    public partial class SearchView : PhoneApplicationPage
    {
        public SearchView()
        {
            InitializeComponent();
        }

        #region Properties
        private SearchViewModel _mainViewModel;
        public SearchViewModel MainViewModel
        {
            get
            {
                return _mainViewModel ?? (_mainViewModel = this.DataContext as SearchViewModel);
            }
        }

        #endregion

       

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var p = (TextBox) sender;
                MainViewModel.SearchVenue = p.Text;
            }
        }
    }
}