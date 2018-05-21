using System;
using System.Collections.Generic;
using Prism.Navigation;
using Xamarin.Forms;
using Naxam.Controls.Forms;

namespace Valuta.Views
{
	public partial class CustomTabbedPage : TabbedPage
    {
        public CustomTabbedPage()
        {
			InitializeComponent();
			this.BarBackgroundColor = Color.FromHex("#FAFAFA");
			this.BarTextColor =Color.FromHex("#AB47BC");
			NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
