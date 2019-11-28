## Supported Selectors

| Selector | Example | Description |
| -------- | ------- | ----------- |
| .class | .header | Selects all elements with the StyleClass property containing 'header' |
| #id | #email | Selects all elements with StyleId set to email. If StyleId is not set, fallback to x:Name. When using Xaml, always prefer x:Name over StyleId. |
| * | * | Selects all elements |
| element | label | Selects all elements of type Label (but not subclasses). Case irrelevant. |
| ^base | ^contentpage | Selects all elements with ContentPage as base class, including ContentPage itself. Case irrelevant. This selector isn't present in the CSS specification, and only applies to XF. |
| element,element | label,button | Selects all Buttons and all Labels |
| element element | stacklayout label | Selects all Labels inside of a StackLayout |
| element>element | stacklayout>label | Selects all Labels with StackLayout as a direct parent |
| element+element | label+entry | Selects all Entries directly after a Label |
| element~element | label~entry | Selects all Entries preceded by a Label |

## Unsupported Selectors (for this version)

- `[attribute]` selectors
- `@media` or `@supports` selectors
- `:` or `::` selectors

## Selector combinations

Selectors can be combined without limitation, like in StackLayout > ContentView > label.email. But keep it sane !

## Precedence

Styles with matching selectors are all applied, on by one, in definition order. Styles defined on the item itself is always applied last.

This is the expected behavior in most cases, even if doesn't 100% match common CSS implementations.

Specificity, and specificity overrides (`!important`) are not supported. This is a known issue.

## Unsupported Common Properties

- `all: initial`
- layout properties (box, or grid). FlexLayout is coming, and it'll be CSS stylable,
- shorthand properties, like `font`, `border`

## Colors

- one of the 140 X11 color (https://en.wikipedia.org/wiki/X11_color_names), which happens to match CSS Colors, UWP predefined colors and XF Colors. Case insensitive
- HEX: `#rgb`, `#argb`, `#rrggbb`, `#aarrggbb`
- RGB: `rgb(255,0,0)`, `rgb(100%,0%,0%)` => values in range 0-255 or 0%-100%
- RGBA: `rgba(255, 0, 0, 0.8)`, `rgba(100%, 0%, 0%, 0.8)` => opacity is 0.0-1.0
- HSL: `hsl(120, 100%, 50%)` => h is 0-360, s and l are 0%-100%
- HSLA: `hsla(120, 100%, 50%, .8)` => opacity is 0.0-1.0

## Thickness

One, two, three or four values, separated by white spaces.

- a single value indicates uniform thickness
- two values indicates (resp.) vertical and horizontal thickness
- three values indicates (resp.) top, horizontal (left and right) and bottom thickness
- when using four values, they are top, right, bottom, left

> IMPORTANT: This differs from Xaml thickness definitions, which are
>
> 1. separated by commas (`,`)
> 1. are in the form of `uniform`, `horizontal`, `vertical` or `left`, `top`, `right`, `bottom`

## NamedSize

One of the following value, case insensitive. Exact meaning depends of the platform and the control

- `default`
- `micro`
- `small`
- `medium`
- `large`

## Initial

`initial` is a valid value for all properties. It clears the value (resets to default) that was set from another Style.

## Additional remarks

- no inheritance supported, meaning no `inherit` value and that you can't set the font-size to a layout, and expect all the labels in that layout to inherit the value. The only exception is the `direction` property, which supports `inherit`, and that's the default value.
- element are matched by name, no support for xmlns