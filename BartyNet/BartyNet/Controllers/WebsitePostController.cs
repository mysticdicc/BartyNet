using BartyLib.Classes.Images;
using BartyLib.Classes.Posts;
using BartyNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BartyNet.Controllers
{
    [ApiController]
    public class WebsitePostController : ControllerBase
    {
        private readonly IDbContextFactory<BartyDb> _PortfolioFactory;

        public WebsitePostController(IDbContextFactory<BartyDb> PortfolioFactory)
        {
            _PortfolioFactory = PortfolioFactory;
        }

        [HttpGet]
        [Route("[controller]/get/all")]
        public string GetAll()
        {
            using var db = _PortfolioFactory.CreateDbContext();
            return JsonConvert.SerializeObject(db.WebsitePosts.Include(x => x.Images).ToList(),
                Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        [HttpGet]
        [Route("[controller]/get/byid")]
        public string GetById(string id)
        {
            using var db = _PortfolioFactory.CreateDbContext();
            var realGuid = new Guid(id);
            return JsonConvert.SerializeObject(db.WebsitePosts.Where(x => x.Id == realGuid).Include(x => x.Images).FirstOrDefault(),
                Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        [HttpGet]
        [Route("[controller]/get/bytype")]
        public string GetByPostType(string postType)
        {
            using var db = _PortfolioFactory.CreateDbContext();
            var postTypeIs = Enum.Parse<WebsitePost.PostType>(postType);
            return JsonConvert.SerializeObject(db.WebsitePosts.Where(x => x.PostTypeIs == postTypeIs).Include(x => x.Images).ToList(),
                                    Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        [HttpGet]
        [Route("[controller]/get/last3/bytype")]
        public string GetRecentByType(string postType)
        {
            using var db = _PortfolioFactory.CreateDbContext();
            var postTypeIs = Enum.Parse<WebsitePost.PostType>(postType);

            var list = db.WebsitePosts.Where(x => x.PostTypeIs == postTypeIs)
                                                                .Include(x => x.Images)
                                                                .ToList();

            list = list.OrderByDescending(x => x.Created).TakeLast(3).ToList();

            return JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        [HttpPost]
        [Authorize]
        [Route("[controller]/post/new")]
        public async Task<Results<BadRequest<string>, Created<WebsitePost>>> NewPost(WebsitePost post)
        {
            using var db = _PortfolioFactory.CreateDbContext();

            try
            {
                foreach (Image image in post.Images)
                {
                    if (null != image.Base64String)
                    {
                        await Image.SaveToFile(image.Base64String, image.LocalPath);
                        image.PostId = post.Id;
                        db.Images.Add(image);
                    }
                }

                db.WebsitePosts.Add(post);
                await db.SaveChangesAsync();

                return TypedResults.Created(post.Id.ToString(), post);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }

        }

        [HttpPost]
        [Authorize]
        [Route("[controller]/post/update")]
        public async Task<Results<BadRequest<string>, Ok<WebsitePost>>> UpdatePost(WebsitePost post)
        {
            using var db = _PortfolioFactory.CreateDbContext();

            var existingPost = db.WebsitePosts.Include(x => x.Images).FirstOrDefault(x => x.Id == post.Id);

            if (null != existingPost)
            {
                existingPost.Title = post.Title;
                existingPost.Body = post.Body;
                existingPost.LastSubmit = DateTime.Now;

                List<Guid> imageIds = post.Images.Select(x => x.Id).ToList();

                try
                {
                    foreach (Image image in existingPost.Images)
                    {
                        if (!imageIds.Contains(image.Id))
                        {
                            await Image.DeleteFile(image.LocalPath);
                            db.Images.Remove(image);
                        }
                    }

                    foreach (Image image in post.Images)
                    {
                        if (null != image.Base64String)
                        {
                            await Image.SaveToFile(image.Base64String, image.LocalPath);
                            image.PostId = post.Id;
                            db.Images.Add(image);
                        }
                    }

                    await db.SaveChangesAsync();

                    return TypedResults.Ok(existingPost);
                }
                catch (Exception ex)
                {
                    return TypedResults.BadRequest(ex.Message);
                }
            }
            else
            {
                return TypedResults.BadRequest($"Cannot find post with {post.Id}");
            }
        }

        [HttpPost]
        [Authorize]
        [Route("[controller]/delete/post")]
        public async Task<Results<BadRequest<string>, Ok<string>>> DeletePost(string id)
        {
            using var db = _PortfolioFactory.CreateDbContext();
            Guid? guid = Guid.Parse(id);

            if (guid != null)
            {
                var item = db.WebsitePosts.FirstOrDefault(x => x.Id == guid);

                if (item != null)
                {
                    try
                    {
                        foreach (Image image in item.Images)
                        {
                            if (null != image.Base64String)
                            {
                                await Image.DeleteFile(image.LocalPath);
                                var dbImage = db.Images.Find(image.Id);

                                if (null != dbImage)
                                {
                                    db.Images.Remove(dbImage);
                                }
                            }
                        }

                        db.WebsitePosts.Remove(item);
                        await db.SaveChangesAsync();

                        return TypedResults.Ok(id);
                    }
                    catch (Exception ex)
                    {
                        return TypedResults.BadRequest(ex.Message);
                    }
                }
                else
                {
                    return TypedResults.BadRequest("No item with this GUID in db context");
                }
            }
            else
            {
                return TypedResults.BadRequest("No GUID provided");
            }
        }
    }
}
