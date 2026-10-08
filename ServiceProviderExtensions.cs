using System;
using Microsoft.Extensions.DependencyInjection;

namespace DotNative.ImagePicker;

public static class ImagePickerServiceProviderExtensions
{
#if NET10_0_OR_GREATER
    extension(IServiceProvider services)
    {
        /// <summary>Resolves the registered plugin using the provider's DI lifetime.</summary>
        public IImagePicker ImagePicker => services.GetRequiredService<IImagePicker>();
    }
#else
    /// <summary>Resolves the registered plugin using the provider's DI lifetime.</summary>
    public static IImagePicker ImagePicker(this IServiceProvider services) =>
        services.GetRequiredService<IImagePicker>();
#endif
}
