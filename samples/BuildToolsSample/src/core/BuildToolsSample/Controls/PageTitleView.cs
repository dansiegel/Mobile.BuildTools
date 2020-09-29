using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace BuildToolsSample.Controls
{
    [ContentProperty(nameof(Content))]
    public class PageTitleView : Grid
    {
        public static readonly BindableProperty SubtitleProperty =
            BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(PageTitleView), null);

        public static readonly BindableProperty ContentProperty =
            BindableProperty.Create(nameof(Content), typeof(View), typeof(PageTitleView), null);

        public PageTitleView()
        {
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

            var label = new Label {
                TextColor = Color.White,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center
            };
            label.FontSize = Device.GetNamedSize(NamedSize.Subtitle, label);
            label.SetBinding(Label.TextProperty, new Binding(nameof(Subtitle), source: this));

            var headerbg = new BoxView { Color = Color.FromHex("#9E9E9E") };

            Children.Add(headerbg, 0, 0);
            Children.Add(label, 0, 0);

            var content = new ContentView
            {
                Padding = new Thickness(20)
            };
            content.SetBinding(ContentView.ContentProperty, new Binding(nameof(Content), source: this));
            Children.Add(content, 0, 1);
        }

        public string Subtitle
        {
            get => (string)GetValue(SubtitleProperty);
            set => SetValue(SubtitleProperty, value);
        }

        public View Content
        {
            get => (View)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }
    }
}
