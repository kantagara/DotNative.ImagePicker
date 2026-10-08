using DotNative.FilePicker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DotNative.ImagePicker;

public interface IImagePicker
{
    Task<PickedImage?> PickImageAsync(CancellationToken cancellationToken = default);
}

public static class ImagePickerServices
{
    public static IServiceCollection AddImagePicker(this IServiceCollection services)
    {
        services.AddFilePicker();
        services.TryAddSingleton<IImagePicker, FileBackedImagePicker>();
        return services;
    }
}

internal sealed class FileBackedImagePicker(IFilePicker picker) : IImagePicker
{
    private static readonly HashSet<string> Extensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png",
        ".jpg",
        ".jpeg",
        ".gif",
        ".bmp",
        ".webp",
        ".heic",
        ".heif",
        ".tif",
        ".tiff",
    };

    public async Task<PickedImage?> PickImageAsync(CancellationToken cancellationToken = default)
    {
        var file = await picker.PickAsync(cancellationToken).ConfigureAwait(false);
        if (file is null)
            return null;
        if (!Extensions.Contains(Path.GetExtension(file.Name)))
        {
            await file.DisposeAsync().ConfigureAwait(false);
            throw new InvalidDataException("The selected file is not a supported raster image.");
        }
        return new PickedImage(file);
    }
}

public sealed class PickedImage : IAsyncDisposable
{
    private readonly PickedFile file;

    internal PickedImage(PickedFile file) => this.file = file;

    public string Name => file.Name;

    public long? Length => file.Length;

    public Task<Stream> OpenReadAsync(CancellationToken cancellationToken = default) =>
        file.OpenReadAsync(cancellationToken);

    public ValueTask DisposeAsync() => file.DisposeAsync();
}
