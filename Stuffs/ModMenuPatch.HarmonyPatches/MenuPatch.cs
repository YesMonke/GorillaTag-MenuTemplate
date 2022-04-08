using System;
using System.Collections.Generic;
using System.IO;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

namespace ModMenuPatch.HarmonyPatches;

[HarmonyPatch(typeof(Player))]
[HarmonyPatch(/*Could not decode attribute arguments.*/)]
internal class MenuPatch
{
	public enum PhotonEventCodes
	{
		left_jump_photoncode = 69,
		right_jump_photoncode,
		left_jump_deletion,
		right_jump_deletion
	}

	public static bool ResetSpeed = false;

	private static string[] buttons = new string[7] { "Toggle Super Monke", "Toggle Tag Gun", "Toggle Speed Boost", "Tag All", "Turn Off Tag Freeze", "Toggle Beacon", "Platform Monke" };

	private static bool?[] buttonsActive = new bool?[7] { false, false, false, false, false, false, false };

	private static bool gripDown;

	private static GameObject menu = null;

	private static GameObject canvasObj = null;

	private static GameObject reference = null;

	public static int framePressCooldown = 0;

	private static GameObject pointer = null;

	private static bool gravityToggled = false;

	private static bool flying = false;

	private static int btnCooldown = 0;

	private static int speedPlusCooldown = 0;

	private static int speedMinusCooldown = 0;

	private static float? maxJumpSpeed = null;

	private static float? jumpMultiplier = null;

	private static Vector3 scale = new Vector3(0.0125f, 0.28f, 0.3825f);

	private static bool gripDown_left;

	private static bool gripDown_right;

	private static bool once_left;

	private static bool once_right;

	private static bool once_left_false;

	private static bool once_right_false;

	private static bool once_networking;

	private static GameObject[] jump_left_network = (GameObject[])(object)new GameObject[9999];

	private static GameObject[] jump_right_network = (GameObject[])(object)new GameObject[9999];

	private static GameObject jump_left_local = null;

	private static GameObject jump_right_local = null;

	private static void Prefix()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_081a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0833: Unknown result type (might be due to invalid IL or missing references)
		if (ModMenuPatch.allowSpaceMonke)
		{
		}
		try
		{
			if (!maxJumpSpeed.HasValue)
			{
				maxJumpSpeed = Player.get_Instance().maxJumpSpeed;
			}
			if (!jumpMultiplier.HasValue)
			{
				jumpMultiplier = Player.get_Instance().jumpMultiplier;
			}
			List<InputDevice> list = new List<InputDevice>();
			InputDevices.GetDevicesWithCharacteristics((InputDeviceCharacteristics)324, list);
			InputDevice val = list[0];
			((InputDevice)(ref val)).TryGetFeatureValue(CommonUsages.secondaryButton, ref gripDown);
			if (gripDown && (Object)(object)menu == (Object)null)
			{
				Draw();
				if ((Object)(object)reference == (Object)null)
				{
					reference = GameObject.CreatePrimitive((PrimitiveType)0);
					Object.Destroy((Object)(object)reference.GetComponent<MeshRenderer>());
					reference.get_transform().set_parent(Player.get_Instance().rightHandTransform);
					reference.get_transform().set_localPosition(new Vector3(0f, -0.1f, 0f));
					reference.get_transform().set_localScale(new Vector3(0.01f, 0.01f, 0.01f));
				}
			}
			else if (!gripDown && (Object)(object)menu != (Object)null)
			{
				Object.Destroy((Object)(object)menu);
				menu = null;
				Object.Destroy((Object)(object)reference);
				reference = null;
			}
			if (gripDown && (Object)(object)menu != (Object)null)
			{
				menu.get_transform().set_position(Player.get_Instance().leftHandTransform.get_position());
				menu.get_transform().set_rotation(Player.get_Instance().leftHandTransform.get_rotation());
			}
			if (buttonsActive[0] == true)
			{
				bool flag = false;
				bool flag2 = false;
				list = new List<InputDevice>();
				InputDevices.GetDevicesWithCharacteristics((InputDeviceCharacteristics)580, list);
				val = list[0];
				((InputDevice)(ref val)).TryGetFeatureValue(CommonUsages.primaryButton, ref flag);
				val = list[0];
				((InputDevice)(ref val)).TryGetFeatureValue(CommonUsages.secondaryButton, ref flag2);
				if (flag)
				{
					Transform transform = ((Component)Player.get_Instance()).get_transform();
					transform.set_position(transform.get_position() + ((Component)Player.get_Instance().headCollider).get_transform().get_forward() * Time.get_deltaTime() * 12f);
					((Component)Player.get_Instance()).GetComponent<Rigidbody>().set_velocity(Vector3.get_zero());
					if (!flying)
					{
						flying = true;
					}
				}
				else if (flying)
				{
					((Component)Player.get_Instance()).GetComponent<Rigidbody>().set_velocity(((Component)Player.get_Instance().headCollider).get_transform().get_forward() * Time.get_deltaTime() * 36f);
					flying = false;
				}
				if (flag2)
				{
					if (!gravityToggled && ((Collider)Player.get_Instance().bodyCollider).get_attachedRigidbody().get_useGravity())
					{
						((Collider)Player.get_Instance().bodyCollider).get_attachedRigidbody().set_useGravity(false);
						gravityToggled = true;
					}
					else if (!gravityToggled && !((Collider)Player.get_Instance().bodyCollider).get_attachedRigidbody().get_useGravity())
					{
						((Collider)Player.get_Instance().bodyCollider).get_attachedRigidbody().set_useGravity(true);
						gravityToggled = true;
					}
				}
				else
				{
					gravityToggled = false;
				}
			}
			if (buttonsActive[1] == true)
			{
				bool flag3 = false;
				bool flag4 = false;
				list = new List<InputDevice>();
				InputDevices.GetDevices(list);
				InputDevices.GetDevicesWithCharacteristics((InputDeviceCharacteristics)580, list);
				val = list[0];
				((InputDevice)(ref val)).TryGetFeatureValue(CommonUsages.triggerButton, ref flag3);
				val = list[0];
				((InputDevice)(ref val)).TryGetFeatureValue(CommonUsages.gripButton, ref flag4);
				if (flag4)
				{
					RaycastHit val2 = default(RaycastHit);
					Physics.Raycast(Player.get_Instance().rightHandTransform.get_position() - Player.get_Instance().rightHandTransform.get_up(), -Player.get_Instance().rightHandTransform.get_up(), ref val2);
					if ((Object)(object)pointer == (Object)null)
					{
						pointer = GameObject.CreatePrimitive((PrimitiveType)0);
						Object.Destroy((Object)(object)pointer.GetComponent<Rigidbody>());
						Object.Destroy((Object)(object)pointer.GetComponent<SphereCollider>());
						pointer.get_transform().set_localScale(new Vector3(0.2f, 0.2f, 0.2f));
					}
					pointer.get_transform().set_position(((RaycastHit)(ref val2)).get_point());
					Player val3 = default(Player);
					bool flag5 = GorillaTagger.get_Instance().TryToTag(val2, true, ref val3);
					if (flag3 && !flag5)
					{
						pointer.GetComponent<Renderer>().get_material().SetColor("_Color", Color.get_red());
					}
					else if (!flag3 && flag5)
					{
						pointer.GetComponent<Renderer>().get_material().SetColor("_Color", Color.get_blue());
					}
					else if (flag3 && flag5)
					{
						pointer.GetComponent<Renderer>().get_material().SetColor("_Color", Color.get_green());
						PhotonView.Get((Component)(object)((Component)GorillaGameManager.instance).GetComponent<GorillaGameManager>()).RPC("ReportTagRPC", (RpcTarget)2, new object[1] { val3 });
					}
				}
				else
				{
					Object.Destroy((Object)(object)pointer);
					pointer = null;
				}
			}
			if (buttonsActive[2] == true)
			{
				Player.get_Instance().maxJumpSpeed = ModMenuPatch.speedMultiplier.get_Value();
				Player.get_Instance().jumpMultiplier = ModMenuPatch.jumpMultiplier.get_Value();
			}
			else
			{
				Player.get_Instance().maxJumpSpeed = maxJumpSpeed.Value;
				Player.get_Instance().jumpMultiplier = 1.15f;
			}
			if (buttonsActive[3] == true && btnCooldown == 0)
			{
				btnCooldown = Time.get_frameCount() + 30;
				Player[] playerList = PhotonNetwork.get_PlayerList();
				foreach (Player val4 in playerList)
				{
					PhotonView.Get((Component)(object)((Component)GorillaGameManager.instance).GetComponent<GorillaGameManager>()).RPC("ReportTagRPC", (RpcTarget)2, new object[1] { val4 });
				}
				Object.Destroy((Object)(object)menu);
				menu = null;
				Draw();
			}
			if (buttonsActive[4] == true)
			{
				Player.get_Instance().disableMovement = false;
			}
			if (buttonsActive[5] == true)
			{
				VRRig[] array = (VRRig[])(object)Object.FindObjectsOfType(typeof(VRRig));
				VRRig[] array2 = array;
				foreach (VRRig val5 in array2)
				{
					if (!val5.isOfflineVRRig && !val5.isMyPlayer && !((MonoBehaviourPun)val5).get_photonView().get_IsMine())
					{
						GameObject val6 = GameObject.CreatePrimitive((PrimitiveType)2);
						Object.Destroy((Object)(object)val6.GetComponent<BoxCollider>());
						Object.Destroy((Object)(object)val6.GetComponent<Rigidbody>());
						Object.Destroy((Object)(object)val6.GetComponent<Collider>());
						val6.get_transform().set_rotation(Quaternion.get_identity());
						val6.get_transform().set_localScale(new Vector3(0.04f, 200f, 0.04f));
						val6.get_transform().set_position(((Component)val5).get_transform().get_position());
						((Renderer)val6.GetComponent<MeshRenderer>()).set_material(((Renderer)val5.mainSkin).get_material());
						Object.Destroy((Object)(object)val6, Time.get_deltaTime());
					}
				}
			}
			if (buttonsActive[6] == true)
			{
				ProcessPlatformMonke();
			}
			if (btnCooldown > 0 && Time.get_frameCount() > btnCooldown)
			{
				btnCooldown = 0;
				buttonsActive[3] = false;
				Object.Destroy((Object)(object)menu);
				menu = null;
				Draw();
			}
		}
		catch (Exception ex)
		{
			File.WriteAllText("vey-spacemonkemodmenu_error.log", ex.ToString());
		}
	}

	private static void ProcessPlatformMonke()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Expected O, but got Unknown
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Expected O, but got Unknown
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Expected O, but got Unknown
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Expected O, but got Unknown
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		if (!once_networking)
		{
			PhotonNetwork.NetworkingClient.add_EventReceived((Action<EventData>)PlatformNetwork);
			once_networking = true;
		}
		List<InputDevice> list = new List<InputDevice>();
		InputDevices.GetDevicesWithCharacteristics((InputDeviceCharacteristics)324, list);
		InputDevice val = list[0];
		((InputDevice)(ref val)).TryGetFeatureValue(CommonUsages.gripButton, ref gripDown_left);
		InputDevices.GetDevicesWithCharacteristics((InputDeviceCharacteristics)580, list);
		val = list[0];
		((InputDevice)(ref val)).TryGetFeatureValue(CommonUsages.gripButton, ref gripDown_right);
		if (gripDown_right)
		{
			if (!once_right && (Object)(object)jump_right_local == (Object)null)
			{
				jump_right_local = GameObject.CreatePrimitive((PrimitiveType)3);
				jump_right_local.GetComponent<Renderer>().get_material().SetColor("_Color", Color.get_black());
				jump_right_local.get_transform().set_localScale(scale);
				jump_right_local.get_transform().set_position(new Vector3(0f, -0.0075f, 0f) + Player.get_Instance().rightHandTransform.get_position());
				jump_right_local.get_transform().set_rotation(Player.get_Instance().rightHandTransform.get_rotation());
				object[] array = new object[2]
				{
					new Vector3(0f, -0.0075f, 0f) + Player.get_Instance().rightHandTransform.get_position(),
					Player.get_Instance().rightHandTransform.get_rotation()
				};
				RaiseEventOptions val2 = new RaiseEventOptions
				{
					Receivers = (ReceiverGroup)0
				};
				PhotonNetwork.RaiseEvent((byte)70, (object)array, val2, SendOptions.SendReliable);
				once_right = true;
				once_right_false = false;
			}
		}
		else if (!once_right_false && (Object)(object)jump_right_local != (Object)null)
		{
			Object.Destroy((Object)(object)jump_right_local);
			jump_right_local = null;
			once_right = false;
			once_right_false = true;
			RaiseEventOptions val3 = new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)0
			};
			PhotonNetwork.RaiseEvent((byte)72, (object)null, val3, SendOptions.SendReliable);
		}
		if (gripDown_left)
		{
			if (!once_left && (Object)(object)jump_left_local == (Object)null)
			{
				jump_left_local = GameObject.CreatePrimitive((PrimitiveType)3);
				jump_left_local.GetComponent<Renderer>().get_material().SetColor("_Color", Color.get_black());
				jump_left_local.get_transform().set_localScale(scale);
				jump_left_local.get_transform().set_position(Player.get_Instance().leftHandTransform.get_position());
				jump_left_local.get_transform().set_rotation(Player.get_Instance().leftHandTransform.get_rotation());
				object[] array2 = new object[2]
				{
					Player.get_Instance().leftHandTransform.get_position(),
					Player.get_Instance().leftHandTransform.get_rotation()
				};
				RaiseEventOptions val4 = new RaiseEventOptions
				{
					Receivers = (ReceiverGroup)0
				};
				PhotonNetwork.RaiseEvent((byte)69, (object)array2, val4, SendOptions.SendReliable);
				once_left = true;
				once_left_false = false;
			}
		}
		else if (!once_left_false && (Object)(object)jump_left_local != (Object)null)
		{
			Object.Destroy((Object)(object)jump_left_local);
			jump_left_local = null;
			once_left = false;
			once_left_false = true;
			RaiseEventOptions val5 = new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)0
			};
			PhotonNetwork.RaiseEvent((byte)71, (object)null, val5, SendOptions.SendReliable);
		}
		if (!PhotonNetwork.get_InRoom())
		{
			for (int i = 0; i < jump_right_network.Length; i++)
			{
				Object.Destroy((Object)(object)jump_right_network[i]);
			}
			for (int j = 0; j < jump_left_network.Length; j++)
			{
				Object.Destroy((Object)(object)jump_left_network[j]);
			}
		}
	}

	private static void PlatformNetwork(EventData eventData)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		switch (eventData.Code)
		{
		case 69:
		{
			object[] array2 = (object[])eventData.get_CustomData();
			jump_left_network[eventData.get_Sender()] = GameObject.CreatePrimitive((PrimitiveType)3);
			jump_left_network[eventData.get_Sender()].GetComponent<Renderer>().get_material().SetColor("_Color", Color.get_black());
			jump_left_network[eventData.get_Sender()].get_transform().set_localScale(scale);
			jump_left_network[eventData.get_Sender()].get_transform().set_position((Vector3)array2[0]);
			jump_left_network[eventData.get_Sender()].get_transform().set_rotation((Quaternion)array2[1]);
			break;
		}
		case 70:
		{
			object[] array = (object[])eventData.get_CustomData();
			jump_right_network[eventData.get_Sender()] = GameObject.CreatePrimitive((PrimitiveType)3);
			jump_right_network[eventData.get_Sender()].GetComponent<Renderer>().get_material().SetColor("_Color", Color.get_black());
			jump_right_network[eventData.get_Sender()].get_transform().set_localScale(scale);
			jump_right_network[eventData.get_Sender()].get_transform().set_position((Vector3)array[0]);
			jump_right_network[eventData.get_Sender()].get_transform().set_rotation((Quaternion)array[1]);
			break;
		}
		case 71:
			Object.Destroy((Object)(object)jump_left_network[eventData.get_Sender()]);
			jump_left_network[eventData.get_Sender()] = null;
			break;
		case 72:
			Object.Destroy((Object)(object)jump_right_network[eventData.get_Sender()]);
			jump_right_network[eventData.get_Sender()] = null;
			break;
		}
	}

	private static void AddButton(float offset, string text)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Expected O, but got Unknown
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
		Object.Destroy((Object)(object)val.GetComponent<Rigidbody>());
		((Collider)val.GetComponent<BoxCollider>()).set_isTrigger(true);
		val.get_transform().set_parent(menu.get_transform());
		val.get_transform().set_rotation(Quaternion.get_identity());
		val.get_transform().set_localScale(new Vector3(0.09f, 0.8f, 0.08f));
		val.get_transform().set_localPosition(new Vector3(0.56f, 0f, 0.28f - offset));
		val.AddComponent<BtnCollider>().relatedText = text;
		int num = -1;
		for (int i = 0; i < buttons.Length; i++)
		{
			if (text == buttons[i])
			{
				num = i;
				break;
			}
		}
		if (buttonsActive[num] == false)
		{
			val.GetComponent<Renderer>().get_material().SetColor("_Color", Color.get_red());
		}
		else if (buttonsActive[num] == true)
		{
			val.GetComponent<Renderer>().get_material().SetColor("_Color", Color.get_green());
		}
		else
		{
			val.GetComponent<Renderer>().get_material().SetColor("_Color", Color.get_grey());
		}
		GameObject val2 = new GameObject();
		val2.get_transform().set_parent(canvasObj.get_transform());
		Text val3 = val2.AddComponent<Text>();
		_003F val4 = val3;
		Object builtinResource = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
		((Text)val4).set_font((Font)(object)((builtinResource is Font) ? builtinResource : null));
		val3.set_text(text);
		val3.set_fontSize(1);
		val3.set_alignment((TextAnchor)4);
		val3.set_resizeTextForBestFit(true);
		val3.set_resizeTextMinSize(0);
		RectTransform component = ((Component)val3).GetComponent<RectTransform>();
		((Transform)component).set_localPosition(Vector3.get_zero());
		component.set_sizeDelta(new Vector2(0.2f, 0.03f));
		((Transform)component).set_localPosition(new Vector3(0.064f, 0f, 0.111f - offset / 2.55f));
		((Transform)component).set_rotation(Quaternion.Euler(new Vector3(180f, 90f, 90f)));
	}

	public static void Draw()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected O, but got Unknown
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Expected O, but got Unknown
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		menu = GameObject.CreatePrimitive((PrimitiveType)3);
		Object.Destroy((Object)(object)menu.GetComponent<Rigidbody>());
		Object.Destroy((Object)(object)menu.GetComponent<BoxCollider>());
		Object.Destroy((Object)(object)menu.GetComponent<Renderer>());
		menu.get_transform().set_localScale(new Vector3(0.1f, 0.3f, 0.4f));
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
		Object.Destroy((Object)(object)val.GetComponent<Rigidbody>());
		Object.Destroy((Object)(object)val.GetComponent<BoxCollider>());
		val.get_transform().set_parent(menu.get_transform());
		val.get_transform().set_rotation(Quaternion.get_identity());
		val.get_transform().set_localScale(new Vector3(0.1f, 1f, 1.25f));
		val.GetComponent<Renderer>().get_material().SetColor("_Color", Color.get_black());
		val.get_transform().set_position(new Vector3(0.05f, 0f, 0f));
		canvasObj = new GameObject();
		canvasObj.get_transform().set_parent(menu.get_transform());
		Canvas val2 = canvasObj.AddComponent<Canvas>();
		CanvasScaler val3 = canvasObj.AddComponent<CanvasScaler>();
		canvasObj.AddComponent<GraphicRaycaster>();
		val2.set_renderMode((RenderMode)2);
		val3.set_dynamicPixelsPerUnit(1000f);
		GameObject val4 = new GameObject();
		val4.get_transform().set_parent(canvasObj.get_transform());
		Text val5 = val4.AddComponent<Text>();
		_003F val6 = val5;
		Object builtinResource = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
		((Text)val6).set_font((Font)(object)((builtinResource is Font) ? builtinResource : null));
		val5.set_text("Monke mod menu");
		val5.set_fontSize(1);
		val5.set_alignment((TextAnchor)4);
		val5.set_resizeTextForBestFit(true);
		val5.set_resizeTextMinSize(0);
		RectTransform component = ((Component)val5).GetComponent<RectTransform>();
		((Transform)component).set_localPosition(Vector3.get_zero());
		component.set_sizeDelta(new Vector2(0.28f, 0.05f));
		((Transform)component).set_position(new Vector3(0.06f, 0f, 0.175f));
		((Transform)component).set_rotation(Quaternion.Euler(new Vector3(180f, 90f, 90f)));
		for (int i = 0; i < buttons.Length; i++)
		{
			AddButton((float)i * 0.13f, buttons[i]);
		}
	}

	public static void Toggle(string relatedText)
	{
		int num = -1;
		for (int i = 0; i < buttons.Length; i++)
		{
			if (relatedText == buttons[i])
			{
				num = i;
				break;
			}
		}
		if (buttonsActive[num].HasValue)
		{
			buttonsActive[num] = !buttonsActive[num];
			Object.Destroy((Object)(object)menu);
			menu = null;
			Draw();
		}
	}
}
