# GeneXus.Drawing-DotNet
A cross-platform implementation of selected `System.Drawing` elements tailored for GeneXus development.

## Status
| Branch | Build
|--------|---
| main   | |[![](https://github.com/genexuslabs/GeneXus.Drawing-DotNet/workflows/Build/badge.svg?branch=main)](https://github.com/genexuslabs/genexus.drawing-dotnet/actions?query=workflow%3ABuild+branch%3Amain)

## History
The creation of this library was driven by Microsoft's decision to make the `System.Drawing.Common` NuGet package as Windows-specific as of .NET 6. For further details about this annoucement, please refer to [Microsoft Learn Article: System.Drawing.Common only supported on Windows](https://learn.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/6.0/system-drawing-common-windows-only).

In response, this project aims to replicate the behaviour of `System.Drawing` library but focusing on GeneXus needs, rather than attempting to cover every use case of the original library. The primeray goal is to enable developers to seamlessy replace the `using` statements referencing the `System.Drawing` library in the GeneXus platform code with `GeneXus.Drawing` namespace. To achieve this goal, this project relies on the `SkiaSharp` library, a robust cross-platform graphics framework.

## Considerations
There are a few important considerations regarding `GeneXus.Drawing` that users should be aware of:

1) The library may not fully replicate all the functionalities provided by `System.Drawing`, either because of limitations in `SkiaSharp` or subtle differences in behavior between `SkiaSharp` and `System.Drawing`.

2) The library is focused on meeting the GeneXus needs, which means that not every feature of `System.Drawing` is required or implemented.

## Repository
This repository is organized as follows:
```
Root
├─ src                           : source code
│  ├─ Common
│  │  ├─ Drawing2D               : elements for GeneXus.Drawing.Drawing2D
│  │  │  └─ ...
│  │  ├─ Imaging                 : elements for GeneXus.Drawing.Imaging
│  │  │  └─ ...
│  │  ├─ Interop                 : elements for GeneXus.Drawing.Interop
│  │  │  └─ ...
│  │  ├─ Text                    : elements for GeneXus.Drawing.Text
│  │  │  └─ ...
│  │  └─ ...                     : elements for GeneXus.Drawing
│  │ 
│  └─ PackageREADME.md           : package readme file
│
├─ test                          : unit tests
│  ├─ Common
│  │  ├─ Drawing2D               : unit-tests for GeneXus.Drawing.Drawing2D
│  │  │  └─ ...
│  │  ├─ Text                    : unit-tests for GeneXus.Drawing.Text
│  │  │  └─ ...
│  │  └─ ...                     : unit-tests for GeneXus.Drawing
│  │
│  └─ Directory.Build.props      : specific configuration for tests
│
├─ .editorconfig                 : configuration for Visual Studio 
├─ .gitignore                    : patters in this repo to be ignored 
├─ CONTRIBUTION.md               : contribution policies
├─ Directory.Build.props         : default configuration for projects
├─ GeneXus.Drawing-DotNet.sln    : solutuon to build every project
├─ global.json                   : SDKs versions used in this project
├─ LICENCE                       : licence agreement
├─ Nuget.Config                  : nuget config settings
└─ README.md                     : this file
```

The `src/` directory is organized in alignment with the structure of `System.Drawing`, reflecting the corresponding `namespace` for each implemented component. Similarly, the `test/` directory mirrors the structure of the `src/` directory, with each test case file named according to the tested component, suffixed with `UnitTest` (e.g. `test/BitmapUnitTest.cs`).

## Modules
This section describes each module (namespace) specifiying which elements are part of `GeneXus.Drawing` library, either totally or partially supported.

### GeneXus.Drawing.Common
Basic graphics funcionalities based on `System.Drawing`.

| Name                    | Type     | Description
|-------------------------|----------|--------------
| `Bitmap`                | Class    | Represents an image defined by pixels.
| `Brush`                 | Class    | Abstract class for brushes used to fill graphics shapes.
| `Color`                 | Class    | Defines colors used for drawing.
| `Font`                  | Class    | Defines a format for text, including font family, size, and style.
| `Graphics`              | Class    | Provides methods for drawing on a drawing surface.
| `Icon`                  | Class    | Represents an icon image.
| `Image`                 | Class    | Represents an image in a specific format.
| `Pen`                   | Class    | Defines an object used to draw lines and curves.
| `Region`                | Class    | Defines the area of a drawing surface.
| `Svg` <sup>(1)</sup>    | Class    | Represents Scalable Vector Graphics.
| `TextureBrush`          | Class    | Defines a brush that uses an image to fill shapes.
| `Point`                 | Struct   | Defines an x and y coordinate in a 2D plane.
| `PointF`                | Struct   | Defines a floating-point x and y coordinate in a 2D plane.
| `Rectangle`             | Struct   | Defines an x, y, width, and height of a rectangle.
| `RectangleF`            | Struct   | Defines a floating-point x, y, width, and height of a rectangle.
| `Size`                  | Struct   | Defines the width and height of a rectangular area.
| `SizeF`                 | Struct   | Defines the width and height of a rectangular area with floating-point values.
| `CopyPixelOperation`    | Enum     | Specifies the type of pixel copying operation.
| `FontSlant`             | Enum     | Specifies the slant of a font.
| `FontStyle`             | Enum     | Specifies the style of a font.
| `GraphicsUnit`          | Enum     | Specifies the unit of measure for drawing operations.
| `KnownColor`            | Enum     | Defines predefined colors.
| `RotateFlipType`        | Enum     | Specifies how an image is rotated or flipped.
| `StringAlignment`       | Enum     | Specifies the alignment of text within a string.
| `StringDigitSubstitute` | Enum     | Specifies how digits are substituted in a string.
| `StringFormatFlags`     | Enum     | Specifies formatting options for strings.
| `StringTrimming`        | Enum     | Specifies how text is trimmed when it does not fit.

<small><sup>(1)</sup> New element (does not belogs to `System.Drawing` library).</small>

### GeneXus.Drawing.Drawing2D
Advanced 2D graphics functionalities based on `System.Drawing.Drawing2D` for complex vector graphics and rendering tasks.

| Name                  | Type     | Description
|-----------------------|----------|--------------
| `Blend`               | Class    | Defines a blend of colors along a gradient.
| `ColorBlend`          | Class    | Defines a blend of colors for a gradient.
| `GraphicsPath`        | Class    | Represents a series of connected lines and curves.
| `HatchBrush`          | Class    | Defines a brush with a hatching pattern.
| `PathGradientBrush`   | Class    | Defines a brush that fills an area with a gradient of colors.
| `LinearGradientBrush` | Class    | Defines a brush that fills an area with a linear gradient of colors.
| `Matrix`              | Struct   | Defines a transformation matrix for graphics operations.
| `PathData`            | Struct   | Contains data associated with a GraphicsPath object.
| `CombineMode`         | Enum     | Specifies how two graphics objects are combined.
| `CompositingQuality`  | Enum     | Specifies the quality of compositing operations.
| `CoordinateSpace`     | Enum     | Specifies the coordinate space for transformations.
| `DashCap`             | Enum     | Specifies the cap style for dashed lines.
| `FillMode`            | Enum     | Specifies the fill mode for filling shapes.
| `InterpolationMode`   | Enum     | Specifies the interpolation mode for scaling and resizing.
| `LineCap`             | Enum     | Specifies the shape of the end of a line.
| `LineJoin`            | Enum     | Specifies the shape used to join two connected lines.
| `MatrixOrder`         | Enum     | Specifies the order of matrix transformations.
| `PenAlignment`        | Enum     | Specifies the alignment of a pen's stroke.
| `PenType`             | Enum     | Specifies the type of pen used for drawing.
| `PixelOffsetMode`     | Enum     | Specifies how to offset pixels when drawing.
| `SmoothingMode`       | Enum     | Specifies the level of smoothing applied to graphics.
| `WarpMode`            | Enum     | Specifies how text is warped.
| `WrapMode`            | Enum     | Specifies how text wraps within its container.

### GeneXus.Drawing.Imaging
Advanced image processing based on `System.Drawing.Imaging` to support sophisticated image manipulation and format handling.

| Name               | Type     | Description
|--------------------|----------|--------------
| `ColorPalette`     | Class    | Defines a color palette for an image.
| `ImageFormat`      | Class    | Specifies the format of an image file.
| `PixelFormat`      | Class    | Specifies the format of the pixels in an image.
| `ImageFlags`       | Enum     | Specifies options for image processing.
| `ImageLockMode`    | Enum     | Specifies the locking mode for an image.

### GeneXus.Drawing.Text
Advanced typographic features based on `System.Drawing.Text` for managing and rendering fonts and text.

| Name                      | Type     | Description
|---------------------------|----------|--------------
| `FontCollection`          | Class    | Represents a collection of fonts.
| `InstalledFontCollection` | Class    | Represents a collection of installed fonts.
| `PrivateFontCollection`   | Class    | Represents a collection of private fonts.
| `GenericFontFamilies`     | Enum     | Specifies generic font families.
| `HotkeyPrefix`            | Enum     | Specifies how hotkey prefixes are rendered.
| `TextRenderingHint`       | Enum     | Specifies the level of text rendering quality.

### GeneXus.Drawing.Interop
Advanced interoperability utilities based on `System.Drawing.Interop` that includes definitions used in font management and graphics rendering.

| Name      | Type     | Description
|-----------|----------|--------------
| `GDIDEFS` | Enum     | Defines constants for GDI+.
| `LOGFONT` | Struct   | Defines the logical font information for text rendering.

# How to build

## Requirements
To build and run this project, the primary requirement is the ability to execute SkiaSharp. This necessitates having one of the following frameworks installed:
- .NET 6 or higher
- .NET Standard 1.3 or higher
- .NET Core.

## Instructions
1) **Verify Installation Requirements:**<br>
   Ensure that the necessary .NET framework is installed by executing the following command in your console or terminal:
   ```properties
   dotnet --version 
   ```

2) **Navigate to the Project Directory:**<br>
   Make sure you are in the correct project directory before proceeding. Use the appropriate command for your operating system. For example:
   
   Windows Command Line:
   ```properties
   cd C:/GeneXus.Drawing-DotNet
   ```

   Mac Terminal:
   ```properties
   cd ~/Documents/GeneXus.Drawing-DotNet
   ```
   
   Linux Shell:
   ```properties
   cd /home/GeneXus.Drawing-DotNet
   ```

3) **Build the project:**<br>
   Run the following command to compile the code:
   ```properties
   dotnet build GeneXus.Drawing-DotNet.sln
   ```


4) **Run Unit Tests:**<br>
   Run the following command to execute the unit test:
   ```properties
   dotnet test GeneXus.Drawing-DotNet.sln
   ```

# License
```
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
```

# Third-Party License Attributions

The `GeneXus.Drawing` library is based on the [SkiaSharp project](https://github.com/mono/SkiaSharp) which is licensed under open source license. 

## SkiaSharp

> SkiaSharp is a cross-platform 2D graphics API for .NET platforms based on Google's Skia Graphics Library (skia.org). It provides a comprehensive 2D API that can be used across mobile, server and desktop models to render images.

License (MIT): https://github.com/mono/SkiaSharp/blob/main/LICENSE.md
