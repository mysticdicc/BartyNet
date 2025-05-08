using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BartyLib.Classes.Posts
{
    public class WebsitePostAPI(HttpClient httpClient)
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly string controller = "websitepost";

        public async Task<List<WebsitePost>> GetAll()
        {
            List<WebsitePost>? items = await _httpClient.GetFromJsonAsync<List<WebsitePost>>($"/{controller}/get/all");

            if (null != items)
            {
                return items;
            }
            else
            {
                throw new Exception("No list");
            }
        }

        public async Task<WebsitePost> GetById(string guid)
        {
            var post = await _httpClient.GetFromJsonAsync<WebsitePost>($"/{controller}/get/byid?id={guid}");

            if (null != post)
            {
                return post;
            }
            else
            {
                throw new Exception("No post");
            }
        }

        public async Task<List<WebsitePost>> GetByType(WebsitePost.PostType postType)
        {
            var posts = await _httpClient.GetFromJsonAsync<List<WebsitePost>>($"/{controller}/get/bytype?posttype={postType}");

            if (null != posts)
            {
                return posts;
            }
            else
            {
                throw new Exception("No posts");
            }
        }

        public async Task<List<WebsitePost>> GetRecentByType(WebsitePost.PostType postType)
        {
            try
            {
                var posts = await _httpClient.GetFromJsonAsync<List<WebsitePost>>($"/{controller}/get/last3/bytype?posttype={postType}");

                if (null != posts)
                {
                    return posts;
                }
                else
                {
                    throw new Exception("No posts");
                }
            } 
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AddPost(WebsitePost post)
        {
            try
            {
                await _httpClient.PostAsJsonAsync<WebsitePost>($"/{controller}/post/new", post);
            }
            catch
            {
                throw;
            }
        }

        public async Task UpdatePost(WebsitePost post)
        {
            try
            {
                await _httpClient.PostAsJsonAsync<WebsitePost>($"/{controller}/post/update", post);
            }
            catch
            {
                throw;
            }
        }

        public async Task DeletePost(Guid id)
        {
            try
            {
                await _httpClient.PostAsync($"/{controller}/delete/post?id={id}", null);
            }
            catch
            {
                throw;
            }
        }
    }
}
