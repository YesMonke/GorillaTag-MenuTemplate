using UnityEngine;

namespace ModMenuPatch.HarmonyPatches;

internal class BtnCollider : MonoBehaviour
{
	public string relatedText;

	private void OnTriggerEnter(Collider collider)
	{
		if (Time.get_frameCount() >= MenuPatch.framePressCooldown + 30)
		{
			MenuPatch.Toggle(relatedText);
			MenuPatch.framePressCooldown = Time.get_frameCount();
		}
	}
}
