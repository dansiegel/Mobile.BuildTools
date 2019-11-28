| Property | Applies To | Value Type / Info |
|----------|------------|-------------------|
| `background-color` | `VisualElement` | BackgroundColorProperty |
| `background-image` | `Page` | BackgroundImageSourceProperty |
| `border-color` | `IBorderElement` | BorderColorProperty |
| `border-radius` | `ICornerElement` | CornerRadiusProperty |
| `border-radius` | `Button` | CornerRadiusProperty |
| `border-radius` | `Frame` | CornerRadiusProperty |
| `border-radius` | `ImageButton` | CornerRadiusProperty |
| `border-width` | `IBorderElement` | BorderWidthProperty |
| `color` | `IColorElement` | ColorProperty, Inherited = true |
| `color` | `ITextElement` | TextColorProperty, Inherited = true |
| `color` | `ProgressBar` | ProgressBar.ProgressColorProperty |
| `color` | `Switch` | OnColorProperty |
| `column-gap` | `Grid` | ColumnSpacingProperty |
| `direction` | `VisualElement` | FlowDirectionProperty, Inherited = true |
| `font-family` | `IFontElement` | FontFamilyProperty, Inherited = true |
| `font-size` | `IFontElement` | FontSizeProperty, Inherited = true |
| `font-style` | `IFontElement` | FontAttributesProperty, Inherited = true |
| `height` | `VisualElement` | HeightRequestProperty |
| `margin` | `View` | View.MarginProperty |
| `margin-left` | `View` | View.MarginLeftProperty |
| `margin-top` | `View` | View.MarginTopProperty |
| `margin-right` | `View` | View.MarginRightProperty |
| `margin-bottom` | `View` | View.MarginBottomProperty |
| `max-lines` | `Label` | Label.MaxLinesProperty |
| `min-height` | `VisualElement` | MinimumHeightRequestProperty |
| `min-width` | `VisualElement` | MinimumWidthRequestProperty |
| `opacity` | `VisualElement` | OpacityProperty |
| `padding` | `IPaddingElement` | PaddingProperty |
| `padding-left` | `IPaddingElement` | PaddingLeftProperty, PropertyOwnerType = PaddingElement |
| `padding-top` | `IPaddingElement` | PaddingTopProperty, PropertyOwnerType = PaddingElement |
| `padding-right` | `IPaddingElement` | PaddingRightProperty, PropertyOwnerType = PaddingElement |
| `padding-bottom` | `IPaddingElement` | PaddingBottomProperty, PropertyOwnerType = PaddingElement |
| `row-gap` | `Grid` | RowSpacingProperty |
| `text-align` | `ITextAlignmentElement` | HorizontalTextAlignmentProperty, Inherited = true |
| `text-decoration` | `IDecorableTextElement` | DecorableTextElement.TextDecorationsProperty |
| `transform` | `VisualElement` | TransformProperty |
| `transform-origin` | `VisualElement` | TransformOriginProperty |
| `vertical-align` | `ITextAlignmentElement` | VerticalTextAlignmentProperty |
| `visibility` | `VisualElement` | IsVisibleProperty, Inherited = true |
| `width` | `VisualElement` | WidthRequestProperty |
| `letter-spacing` | `ITextElement` | CharacterSpacingProperty, Inherited = true |
| `line-height` | `ILineHeightElement` | LineHeightProperty, Inherited = true |

## FlexLayout

| Property | Applies To | Value Type / Info |
|----------|------------|-------------------|
| `align-content` | `FlexLayout` | AlignContentProperty |
| `align-items` | `FlexLayout` | AlignItemsProperty |
| `align-self` | `VisualElement` | AlignSelfProperty, PropertyOwnerType = FlexLayout |
| `flex-direction` | `FlexLayout` | DirectionProperty |
| `flex-basis` | `VisualElement` | BasisProperty, PropertyOwnerType = FlexLayout |
| `flex-grow` | `VisualElement` | GrowProperty, PropertyOwnerType = FlexLayout |
| `flex-shrink` | `VisualElement` | ShrinkProperty, PropertyOwnerType = FlexLayout |
| `flex-wrap` | `VisualElement` | WrapProperty, PropertyOwnerType = FlexLayout |
| `justify-content` | `FlexLayout` | JustifyContentProperty |
| `order` | `VisualElement` | OrderProperty, PropertyOwnerType = FlexLayout |
| `position` | `FlexLayout` | PositionProperty |

## Xamarin.Forms Specific Properties

| Property | Applies To | Value Type / Info |
|----------|------------|-------------------|
| `-xf-placeholder` | `IPlaceholderElement` | PlaceholderProperty |
| `-xf-placeholder-color` | `IPlaceholderElement` | PlaceholderColorProperty |
| `-xf-max-length` | `InputView` | MaxLengthProperty |
| `-xf-bar-background-color` | `IBarElement` | BarBackgroundColorProperty |
| `-xf-bar-text-color` | `IBarElement` | BarTextColorProperty |
| `-xf-orientation` | `ScrollView` | OrientationProperty |
| `-xf-horizontal-scroll-bar-visibility` | `ScrollView` | HorizontalScrollBarVisibilityProperty |
| `-xf-vertical-scroll-bar-visibility` | `ScrollView` | VerticalScrollBarVisibilityProperty |
| `-xf-min-track-color` | `Slider` | MinimumTrackColorProperty |
| `-xf-max-track-color` | `Slider` | MaximumTrackColorProperty |
| `-xf-thumb-color` | `Slider` | ThumbColorProperty |
| `-xf-spacing` | `StackLayout` | SpacingProperty |
| `-xf-orientation` | `StackLayout` | OrientationProperty |
| `-xf-visual` | `VisualElement` | VisualProperty |
| `-xf-vertical-text-alignment` | `Label` | VerticalTextAlignmentProperty |
| `-xf-thumb-color` | `Switch` | ThumbColorProperty |

## Shell Specific Properties

| Property | Applies To | Value Type / Info |
|----------|------------|-------------------|
| `-xf-flyout-background` | `Shell` | FlyoutBackgroundColorProperty |
| `-xf-shell-background` | `Element` | BackgroundColorProperty, PropertyOwnerType = Shell |
| `-xf-shell-disabled` | `Element` | DisabledColorProperty, PropertyOwnerType = Shell |
| `-xf-shell-foreground` | `Element` | ForegroundColorProperty, PropertyOwnerType = Shell |
| `-xf-shell-tabbar-background` | `Element` | TabBarBackgroundColorProperty, PropertyOwnerType = Shell |
| `-xf-shell-tabbar-disabled` | `Element` | TabBarDisabledColorProperty, PropertyOwnerType = Shell |
| `-xf-shell-tabbar-foreground` | `Element` | TabBarForegroundColorProperty, PropertyOwnerType = Shell |
| `-xf-shell-tabbar-title` | `Element` | TabBarTitleColorProperty, PropertyOwnerType = Shell |
| `-xf-shell-tabbar-unselected` | `Element` | TabBarUnselectedColorProperty, PropertyOwnerType = Shell |
| `-xf-shell-title` | `Element` | TitleColorProperty, PropertyOwnerType = Shell |
| `-xf-shell-unselected` | `Element` | UnselectedColorProperty, PropertyOwnerType = Shell |