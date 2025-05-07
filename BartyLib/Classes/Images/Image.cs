using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BartyLib.Classes.Posts;
using System.Xml.Linq;

namespace BartyLib.Classes.Images
{
    public class Image
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LocalPath { get; set; }
        public string RemotePath { get; set; }
        public string FileExtension { get; set; }
        [NotMapped] public string? Base64String { get; set; }
        public Guid? PostId { get; set; }
        [JsonIgnore] public WebsitePost? WebsitePost { get; set; }

        public Image(string name, string fileExtension)
        {
            Id = Guid.NewGuid();
            LocalPath = $"./wwwroot/img/{Id}.{fileExtension}";
            RemotePath = $"https://lifeofbarty.danknet.uk/img/{Id}.{fileExtension}";
            Name = name;
            FileExtension = fileExtension;
        }

        static public async Task<string> ConvertToBase64(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();
            var base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        static public async Task SaveToFile(string base64, string filePath)
        {
            using var memoryStream = new MemoryStream();
            byte[] bytes = Convert.FromBase64String(base64);

            if (File.Exists(filePath))
            {
                try
                {
                    await Task.Run(() => { File.Delete(filePath); });
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            try
            {
                await File.WriteAllBytesAsync(filePath, bytes);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        static public async Task DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    await Task.Run(() => { File.Delete(filePath); });
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                throw new Exception("File not exist");
            }
        }
    }

    public class ThumbnailImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string ThumbnailName { get; set; }
        public string LocalPath { get; set; }
        public string RemotePath { get; set; }
        public string FileExtension { get; set; }
        [NotMapped] public string? Base64String { get; set; }
        public Guid? PostId { get; set; }
        [JsonIgnore] public WebsitePost? ThumbnailPost { get; set; }

        public ThumbnailImage(string thumbnailName, string fileExtension)
        {
            Id = Guid.NewGuid();
            LocalPath = $"./wwwroot/img/{Id}.{fileExtension}";
            RemotePath = $"https://lifeofbarty.danknet.uk/img/{Id}.{fileExtension}";
            ThumbnailName = thumbnailName;
            FileExtension = fileExtension;
        }
    }
}
