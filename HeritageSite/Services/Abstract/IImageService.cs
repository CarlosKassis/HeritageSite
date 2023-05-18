
namespace HeritageSite.Services.Abstract
{
    using System.Threading.Tasks;

    public interface IImageService
    {
        Task<byte[]> GetImage(string imageName);

        Task<byte[]> GetImageLowResolution(string imageName);
    }
}
