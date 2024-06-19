namespace Adminpanal.Hellper
{
	public static class ImageSetting
	{
		public static string UplodaImage(IFormFile File , string FolderName)
		{

			var folderpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", FolderName); 
			
			var fileName = Guid.NewGuid()+File.FileName;

			var FilePath = Path.Combine(folderpath, fileName);

			var fs = new FileStream(FilePath, FileMode.Create);
			File.CopyTo(fs);

			return Path.Combine($"images\\{FolderName}", fileName); 

		}

		public static void DeleteImage(string Filename , string FolderNmae)
		{
			var FilePath = Path.Combine(Directory.GetCurrentDirectory(), Filename);

			if (File.Exists(FilePath)) {
		       
			   File.Delete(FilePath);	
			}
		}
	}
}
