using KinoStars.Data;
using KinoStars.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace KinoStars.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReviewController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
                _db = db;
				_webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Review> reviews = _db.Reviews;
			IEnumerable<Review> sorted = reviews.OrderByDescending(review => review.Score).ThenBy(review => review.Title);
            return View(sorted);
        }


        public IActionResult Create()
		{
			return View();
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(ReviewViewModel obj)
		{

	        if (ModelState.IsValid)
            {
				//CONVERT BETWEEN MODELS TO SAVE
				Review modelobj = new Review();
				modelobj.Id = obj.Id;
				modelobj.Title = obj.Title;
				modelobj.ReviewText = obj.ReviewText;
				modelobj.Score = obj.Score;

				//UPLOAD FILE
				if( obj.ReviewPhoto != null){
				 	string folder = "uploads/";
				 	folder += Guid.NewGuid().ToString() + "-" + obj.ReviewPhoto.FileName;
				 	string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
					modelobj.PhotoPath = folder;
					FileStream streamy = new FileStream(serverFolder, FileMode.Create);
					await obj.ReviewPhoto.CopyToAsync(streamy);
					streamy.Close();

				} else {
					modelobj.PhotoPath = "none";
				}

                _db.Reviews.Add(modelobj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
		}

		public IActionResult Update(int? id)
		{
			if(id== null || id == 0)
			{
				return NotFound();
			}
			var reviewFromDb = _db.Reviews.Find(id);

				//CONVERT BETWEEN MODELS TO SAVE
				ReviewUpdateModel modelobj = new ReviewUpdateModel();
				modelobj.Id = reviewFromDb.Id;
				modelobj.Title = reviewFromDb.Title;
				modelobj.ReviewText = reviewFromDb.ReviewText;
				modelobj.Score = reviewFromDb.Score;
				modelobj.OldPath = reviewFromDb.PhotoPath;

				ViewData["PhotoPath"] = reviewFromDb.PhotoPath;
				string filepath = "wwwroot\\" + reviewFromDb.PhotoPath;
				var filestream = System.IO.File.OpenRead(filepath);
				modelobj.ReviewPhoto = new FormFile(filestream, 0, filestream.Length, null, Path.GetFileName(filestream.Name));
				filestream.Close();

			if(reviewFromDb == null)
			{
				return NotFound();
			}

			return View(modelobj);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateAsync(ReviewUpdateModel obj)
		{

			if (ModelState.IsValid)
			{
				//CONVERT BETWEEN MODELS TO SAVE
				Review modelobj = new Review();
				modelobj.Id = obj.Id;
				modelobj.Title = obj.Title;
				modelobj.ReviewText = obj.ReviewText;
				modelobj.Score = obj.Score;

				//DELETE FILE
				FileInfo img_file = new FileInfo(Path.Combine(_webHostEnvironment.WebRootPath, obj.OldPath));
				if (img_file.Exists){
					img_file.Delete();
				}

				//UPLOAD NEW FILE
				if( obj.ReviewPhoto != null){
				 	string folder = "uploads/";
				 	folder += Guid.NewGuid().ToString() + "-" + obj.ReviewPhoto.FileName;
				 	string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
					modelobj.PhotoPath = folder;
					FileStream streamy = new FileStream(serverFolder, FileMode.Create);
					await obj.ReviewPhoto.CopyToAsync(streamy);
					streamy.Close();

				} else {
					modelobj.PhotoPath = "none";
				}


				_db.Reviews.Update(modelobj);
				_db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(obj);
		}
		public IActionResult Delete(int? id)
		{
			if(id== null || id == 0)
			{
				return NotFound();
			}
			var reviewFromDb = _db.Reviews.Find(id);

			if(reviewFromDb == null)
			{
				return NotFound();
			}

			ViewData["PhotoPath"] = reviewFromDb.PhotoPath;

			return View(reviewFromDb);
		}

		[HttpPost, ActionName("DeletePOST")]
		[ValidateAntiForgeryToken]
		public IActionResult DeletePOST(int? id)
		{
			var obj = _db.Reviews.Find(id);

			if (obj == null)
			{
				return NotFound();
			}

			Review modelobj = new Review();
			modelobj.Id = obj.Id;
			modelobj.Title = obj.Title;
			modelobj.ReviewText = obj.ReviewText;
			modelobj.Score = obj.Score;
			modelobj.PhotoPath = obj.PhotoPath;

			//DELETE HOSTED IMAGE
			FileInfo img_file = new FileInfo(Path.Combine(_webHostEnvironment.WebRootPath, modelobj.PhotoPath));
			if (img_file.Exists){
				img_file.Delete();
			}

			//DELETE FROM DB
			_db.Reviews.Remove(obj);
			_db.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}
