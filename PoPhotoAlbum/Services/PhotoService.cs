using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using PoPhotoAlbum.Data;
using PoPhotoAlbum.Models;
using System.IO;
using System.Threading.Tasks;

public interface IPhotoService
{
    Task UploadPhotoAsync(Stream fileStream, string fileName, string userId);
    Task DeletePhotoAsync(int photoId);
    Task<List<Photo>> GetPhotosByUserId(string userId);
}

public class PhotoService : IPhotoService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;
    private readonly ApplicationDbContext _context;

    public PhotoService(BlobServiceClient blobServiceClient, IConfiguration configuration, ApplicationDbContext context)
    {
        _blobServiceClient = blobServiceClient;
        _containerName = configuration.GetValue<string>("AzureBlobStorage:ContainerName");
        _context = context;
    }

    public async Task<List<Photo>> GetPhotosByUserId(string userId)
    {
        return await _context.Photos
                             .Where(photo => photo.UserId == userId)
                             .ToListAsync();
    }

    public async Task UploadPhotoAsync(Stream fileStream, string fileName, string userId)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        // Upload to Azure Blob Storage
        await blobClient.UploadAsync(fileStream, overwrite: true);

        // Save photo metadata in the database
        var photo = new Photo
        {
            FileName = fileName,
            Url = blobClient.Uri.ToString(),
            UserId = userId, // Implement method to get current user ID
            UploadDate = DateTime.UtcNow
        };
        _context.Photos.Add(photo);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePhotoAsync(int photoId)
    {
        var photo = _context.Photos.FirstOrDefault(p => p.PhotoId == photoId);
        if (photo != null)
        {
            // Delete from Azure Blob Storage
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(photo.FileName);
            await blobClient.DeleteIfExistsAsync();

            // Delete from database
            _context.Photos.Remove(photo);
            await _context.SaveChangesAsync();
        }
    }

   

}
