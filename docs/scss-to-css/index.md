# SCSS to Xamarin.Forms CSS

While CSS support in Xamarin.Forms has been a highly controversial topic in which you likely either love it or hate it, it has been my view that CSS support in Xamarin.Forms is the most revolutionary change to Styling XAML. CSS though is traditionally problematic on larger projects as it can quickly become hard to maintain, and error prone as it lacks reusability of common values which could include setting various properties or reusing the same color from one element to the next. With SCSS you gain the ability to break your stylesheets into logical reusable chunks and you gain the ability to define variables and functions for creating your styles. For those developers who want to start using CSS in their Xamarin.Forms application, the Mobile.BuildTools empowers you to leverage SCSS which is compiled into Xamarin.Forms compatible CSS at build.

## Valid Xamarin.Forms CSS

Perhaps one of the most frustrating things about Xamarin.Forms CSS is that the spec is NOT compliant with standard CSS linting. Specifically Xamarin.Forms requires the `^` prefix when looking to have a style apply to any inheriting type which you may want to do for instance when styling a Page.

```css
^button {
  background-color: transparent;
}

.primary ^button {
  background-color: #78909c;
}
```

The Mobile.BuildTools will post process your SCSS to generate valid CSS for Xamarin.Forms when using the selectors `any` or `all`.

## Valid SCSS used by the Mobile.BuildTools

```css
button:any {
  background-color: transparent;
}

.primary button:all {
  background-color: #78909c;
}
```

## Getting Started

To get started, simply add any scss format stylesheets you want to your project and make sure that the build action is set to `None`. The Mobile.BuildTools will automatically detect them and generate a CSS file for each non-partial (anything not starting with an underscore). For more information on how to get started with SCSS see the [Getting Started Guide](https://sass-lang.com/guide) from LibSass.

!!! note "Note"
    Xamarin.Forms does not support minimized files
