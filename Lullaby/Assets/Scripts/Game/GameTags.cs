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
		public static string Panel = "Panel";
		public static string Spring = "Spring";
		public static string Hitbox = "Hitbox";
		public static string InteractiveRail = "Interactive/Rail";

		public static bool IsEntity(Collider collider) =>
			collider.CompareTag(Player) || collider.CompareTag(Enemy);

		public static bool IsHazard(Collider collider) => collider.CompareTag(Hazard);
	}
}
