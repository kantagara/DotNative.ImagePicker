# DotNative.ImagePicker

Selects an existing raster image through DotNative.FilePicker and exposes a
bounded native read stream.

```csharp
builder.Services.AddImagePicker();
await using var image = await services.ImagePicker
    .PickImageAsync(cancellationToken);
if (image is not null)
{
    await using var stream = await image.OpenReadAsync(cancellationToken);
}
```

This release supports existing images on every platform where the referenced
DotNative.FilePicker implementation is installed. It checks the file extension
after selection, so the system picker may display non-image files; those are
rejected with `InvalidDataException`. Camera capture, video, native image-type
filters, multiple selection, and image metadata are not implemented. The package
is an independently authored MIT implementation.

## Service access

Import `DotNative.ImagePicker` to access the plugin through `IServiceProvider`:

```csharp
using DotNative.ImagePicker;

var plugin = services.ImagePicker;
```

The getter calls `GetRequiredService<IImagePicker>()` on every access, preserving
DI lifetimes and the usual missing-registration error. Register the plugin with
`AddImagePicker(...)` before building the provider.

A `net10.0` application uses the property syntax with C# 14 or later. A
`net9.0` application uses only the method equivalent:

```csharp
var plugin = services.ImagePicker();
```

The package contains separate `net9.0` and `net10.0` assemblies. NuGet selects
the assembly matching the application target framework. `NET10_0_OR_GREATER`
selects the property; the `#else` branch selects the method.

Build and pack both targets with .NET 10 SDK. A source build using .NET 9 SDK
builds only `net9.0`; it does not produce the .NET 10 assembly.
