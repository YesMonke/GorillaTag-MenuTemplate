using GorillaLocomotion;
using HarmonyLib;

namespace ModMenuPatch.HarmonyPatches;

[HarmonyPatch(typeof(Player))]
[HarmonyPatch(/*Could not decode attribute arguments.*/)]
internal class JumpPatch
{
	public static bool ResetSpeed;

	private static void Prefix()
	{
	}
}
