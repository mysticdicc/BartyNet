using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BartyLib.Classes.Images
{
    public class ImageAPI(HttpClient httpClient)
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<List<Image>> GetAll()
        {
            List<Image>? images = await _httpClient.GetFromJsonAsync<List<Image>>("/image/get/all");

            if (null != images)
            {
                return images;
            }
            else
            {
                throw new Exception("No list");
            }
        }

        public async Task<Image> GetById(Guid id)
        {
            Image? image = await _httpClient.GetFromJsonAsync<Image>($"/image/get/byid?id={id}");

            if (null != image)
            {
                return image;
            }
            else
            {
                throw new Exception("No image");
            }
        }

        public async Task<List<Image>> GetByType(Image.ImageType imageType) 
        {
            List<Image>? images = await _httpClient.GetFromJsonAsync<List<Image>>($"/image/get/bytype?imagetype={imageType}");

            if (null != images)
            {
                return images;
            }
            else
            {
                throw new Exception("No list");
            }
        }

        public async Task AddNew(Image image)
        {
            await _httpClient.PostAsJsonAsync<Image>("/image/post/new", image);
        }

        public async Task UpdateExisting(Image image)
        {
            await _httpClient.PostAsJsonAsync<Image>("/image/post/update", image);
        }

        public async Task DeleteExisting(Image image)
        {
            await _httpClient.PostAsJsonAsync<Image>("/image/post/delete", image);
        }
    }
}
