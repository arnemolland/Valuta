using System;
using Valuta.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.AppCompat;
using Java.Lang;
using Android.Support.Design.Widget;
using Android.Widget;
using Valuta.Droid;
using Android.Content;

[assembly: ExportRenderer(typeof(CustomTabbedPage), typeof(CustomTabbedPageRenderer))]
namespace Valuta.Droid
{
	public class CustomTabbedPageRenderer : TabbedPageRenderer
    {
		public CustomTabbedPageRenderer(Context context) : base(context)
		{
		}

		protected override void SetTabIcon(TabLayout.Tab tab, FileImageSource icon)
		{
			base.SetTabIcon(tab, icon);

			tab.SetCustomView(Resource.Layout.MyTabbar);
			var imageview = tab.CustomView.FindViewById<ImageView>(Resource.Id.icon);
			imageview.SetBackgroundDrawable(tab.Icon);
			                   
		}
	}
}
