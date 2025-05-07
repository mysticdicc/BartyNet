using BartyNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using BartyLib.Classes.Images;

namespace BartyNet.Controllers
{
    [ApiController]
    public class ImageController : Controller
    {
        private readonly IDbContextFactory<BartyDb> _PortfolioFactory;

        public ImageController(IDbContextFactory<BartyDb> portfolioFactory)
        {
            _PortfolioFactory = portfolioFactory;
        }

        [HttpGet]
        [Authorize]
        [Route("[controller]/get/all")]
        public string GetAll()
        {
            using var db = _PortfolioFactory.CreateDbContext();
            return JsonConvert.SerializeObject(db.Images.ToList());
        }

        [HttpGet]
        [Authorize]
        [Route("[controller]/get/byid")]
        public string GetById(Guid guid)
        {
            using var db = _PortfolioFactory.CreateDbContext();
            var image = db.Images.Where(x => x.Id == guid).FirstOrDefault();
            return JsonConvert.SerializeObject(image);
        }

        [HttpPost]
        [Authorize]
        [Route("[controller]/post/new")]
        public async Task<Results<Ok<Image>, BadRequest<string>>> PostNew(Image image)
        {
            if (null != image.Base64String)
            {
                using var db = _PortfolioFactory.CreateDbContext();
                db.Images.Add(image);

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return TypedResults.BadRequest(ex.Message);
                }

                try
                {
                    await Image.SaveToFile(image.Base64String, image.LocalPath);
                    return TypedResults.Ok(image);
                }
                catch (Exception ex)
                {
                    return TypedResults.BadRequest(ex.Message);
                }
            }
            else
            {
                return TypedResults.BadRequest("Base64 image string not provided");
            }
        }

        [HttpPost]
        [Authorize]
        [Route("[controller]/post/update")]
        public async Task<Results<BadRequest<string>, Ok<string>>> UpdateExisting(Image uploadImage)
        {
            if (null != uploadImage.Base64String)
            {
                try
                {
                    await Image.SaveToFile(uploadImage.Base64String, uploadImage.LocalPath);
                    return TypedResults.Ok("Image replaced");
                }
                catch (Exception ex)
                {
                    return TypedResults.BadRequest<string>(ex.Message);
                }
            }
            else
            {
                return TypedResults.BadRequest<string>("No image provided");
            }
        }

        [HttpPost]
        [Authorize]
        [Route("[controller]/post/delete")]
        public async Task<Results<BadRequest<string>, Ok<string>>> DeleteImage(Image image)
        {
            using var db = _PortfolioFactory.CreateDbContext();
            var dbImage = db.Images.Find(image.Id);

            if (null != dbImage)
            {
                try
                {
                    await Image.DeleteFile(image.LocalPath);
                }
                catch (Exception ex)
                {
                    return TypedResults.BadRequest(ex.Message);
                }

                try
                {
                    db.Images.Remove(dbImage);
                    await db.SaveChangesAsync();
                    return TypedResults.Ok($"{image.Id} deleted");
                }
                catch (Exception ex)
                {
                    return TypedResults.BadRequest(ex.Message);
                }
            }
            else
            {
                return TypedResults.BadRequest<string>("Unable to find image with this ID");
            }
        }
    }
}
