using System.ComponentModel;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using ModMenuPatch.HarmonyPatches;
using Utilla;

namespace ModMenuPatch;

[Description("HauntedModMenu")]
[BepInPlugin("org.legoandmars.gorillatag.modmenupatch", "Mod Menu Patch", "1.0.0")]
[BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
[ModdedGamemode]
public class ModMenuPatch : BaseUnityPlugin
{
	public static bool allowSpaceMonke = true;

	public static ConfigEntry<float> multiplier;

	public static ConfigEntry<float> speedMultiplier;

	public static ConfigEntry<float> jumpMultiplier;

	private void OnEnable()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		ModMenuPatches.ApplyHarmonyPatches();
		ConfigFile val = new ConfigFile(Path.Combine(Paths.get_ConfigPath(), "ModMonkeyPatch.cfg"), true);
		speedMultiplier = val.Bind<float>("Configuration", "SpeedMultiplier", 100f, "How much to multiply the speed. 10 = 10x higher jumps");
		jumpMultiplier = val.Bind<float>("Configuration", "JumpMultiplier", 1.5f, "How much to multiply the jump height/distance by. 10 = 10x higher jumps");
	}

	private void OnDisable()
	{
		ModMenuPatches.RemoveHarmonyPatches();
	}

	[ModdedGamemodeJoin]
	private void RoomJoined()
	{
		allowSpaceMonke = true;
	}

	[ModdedGamemodeLeave]
	private void RoomLeft()
	{
		allowSpaceMonke = true;
	}
}
