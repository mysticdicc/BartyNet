using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BartyLib.Classes.Images;
using System.ComponentModel.DataAnnotations.Schema;

namespace BartyLib.Classes.Posts
{
    public class WebsitePost
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastSubmit { get; set; }
        public List<Image> Images { get; set; }
        public enum PostType { Blog, Charity, Rights }
        public PostType PostTypeIs { get; set; }

        public WebsitePost(PostType postType)
        {
            Id = Guid.NewGuid();
            Title = String.Empty;
            Body = String.Empty;
            LastSubmit = DateTime.Now;
            Created = DateTime.Now;
            Images = [];
            PostTypeIs = postType;
            
        }

        public WebsitePost(string title, string body, List<Image> images, PostType postTypeIs)
        {
            Id = Guid.NewGuid();
            Title = title;
            Body = body;
            LastSubmit = DateTime.Now;
            Images = images;
            PostTypeIs = postTypeIs;
            Created = DateTime.Now;
        }

        public WebsitePost(string title, string body, PostType postTypeIs)
        {
            Id = Guid.NewGuid();
            Title = title;
            Body = body;
            LastSubmit = DateTime.Now;
            Images = [];
            PostTypeIs = postTypeIs;
            Created = DateTime.Now;
        }

        [JsonConstructor]
        public WebsitePost(Guid Id, string Title, string Body, DateTime LastSubmit, List<Image> Images, PostType PostTypeIs, DateTime Created)
        {
            this.Id = Id;
            this.Title = Title;
            this.Body = Body;
            this.LastSubmit = LastSubmit;
            this.Images = Images;
            this.PostTypeIs = PostTypeIs;
            this.Created = Created;
        }
    }
}
