using System.Reflection;
using HarmonyLib;

namespace ModMenuPatch.HarmonyPatches;

public class ModMenuPatches
{
	private static Harmony instance;

	public const string InstanceId = "com.legoandmars.gorillatag.modmenupatch";

	public static bool IsPatched { get; private set; }

	internal static void ApplyHarmonyPatches()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		if (!IsPatched)
		{
			if (instance == null)
			{
				instance = new Harmony("com.legoandmars.gorillatag.modmenupatch");
			}
			instance.PatchAll(Assembly.GetExecutingAssembly());
			IsPatched = true;
		}
	}

	internal static void RemoveHarmonyPatches()
	{
		if (instance != null && IsPatched)
		{
			instance.UnpatchSelf();
			IsPatched = false;
		}
	}
}
