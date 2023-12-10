using UnityEngine;

namespace Lullaby
{
	public class GameTags
	{
		public static string Player = "Player";
		public static string Enemy = "Enemy";
		public static string Talker = "Talker";
		public static string Hazard = "Hazard";
		public static string Platform = "Platform";
		public static string Piano = "Platform/Piano";
		public static string Platter = "Platter";
		public static string Spring = "Spring";
		public static string Hitbox = "Hitbox";
		public static string InteractiveRail = "Interactive/Rail";
		public static string MoonLauncher = "Moon/Launcher";
		public static string MoonCameraTrigger = "Moon/CameraTrigger";
		public static string MoonPathCart = "Moon/PathCart";
		public static string MoonCartParent = "Moon/CartParent";
		public static string SurfaceGrass = "Surface/Grass";
		public static string SurfaceWood = "Surface/Wood";
		public static bool IsEntity(Collider collider) =>
			collider.CompareTag(Player) || collider.CompareTag(Enemy);

		public static bool IsHazard(Collider collider) => collider.CompareTag(Hazard);
	}
}
