using System.IO;

namespace SoloProjectManagement.Desktop.Persistence
{
	internal class DatabaseSettings
	{

		public static string DatabasePath
		{
			get
			{
				string folder = Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
					"BIT606", "SoleProjectManagement");

				Directory.CreateDirectory(folder);
				return Path.Combine(folder, "projects.db");
			}
		}

		public static string ConnectionString
		{
			get
			{
				string escapedPath = DatabasePath.Replace("\"", "\"\"");
				return "Data Source=\"" + escapedPath + "\"";
			}
		}
	}
}
