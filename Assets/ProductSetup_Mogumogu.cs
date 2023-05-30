#if UNITY_EDITOR
using System;
using System.Linq;

using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Dynamics.PhysBone.Components;

/*
 * VRSuya Avatar Setting Updater
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.AvatarSettingUpdater {

	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class ProductSetup_Mogumogu : ProductSetup {

		private static VRSuyaProduct Mogumogu;
		private static GameObject VRSuyaMogumoguGameObject;
		private static Transform[] AvatarCheekBoneTransforms;

		private static readonly string[] dictSELESTIAMogumoguLayerName = new string[] { "Cheek_L_Stretch", "Cheek_R_Stretch" };

		/// <summary>제품 정보를 AssetManager에게 요청하여 업데이트 한 후, 설치된 에셋 목록에 추가합니다.</summary>
		internal static void RegisterProduct() {
			Mogumogu = new VRSuyaProduct();
			Mogumogu = AssetManager.UpdateProductInformation(ProductName.Mogumogu);
			InstalledVRSuyaProducts = InstalledVRSuyaProducts.Concat(new VRSuyaProduct[] { Mogumogu }).ToArray();
			InstalledProductMogumogu = true;
			return;
		}

		/// <summary>외부의 세팅 요청을 처리하는 메인 메소드 입니다.</summary>
		internal static void RequestSetting() {
			if (InstallProductMogumogu) {
				VRSuyaMogumoguGameObject = Array.Find(VRSuyaGameObjects, gameObject => gameObject.name.Contains("VRSuya_Mogumogu_PhysBone"));
				AvatarCheekBoneTransforms = Array.FindAll(AvatarAnimator.GetBoneTransform(HumanBodyBones.Head).GetComponentsInChildren<Transform>(true), transform => transform.name.Contains("Cheek"));
				if (!VRSuyaMogumoguGameObject) SetupPrefab();
				if (VRSuyaMogumoguGameObject) {
					UpdatePhysBoneSetting();
					DisableExistPhysBone();
					if (AvatarType == Avatar.SELESTIA) DisableExistMoumoguAnimatorLayer();
				}
			}
			return;
		}

		/// <summary>아바타에 Prefab이 있는지 검사하고 없으면 설치하는 메소드 입니다.</summary>
		private static void SetupPrefab() {
			string[] ChildAvatarGameObjectNames = new string[0];
			string[] HeadChildAvatarGameObjectNames = new string[0];
			foreach (Transform ChildTransform in AvatarGameObject.transform) {
				ChildAvatarGameObjectNames = ChildAvatarGameObjectNames.Concat(new string[] { ChildTransform.name }).ToArray();
			}
			foreach (Transform ChildTransform in AvatarAnimator.GetBoneTransform(HumanBodyBones.Head)) {
				HeadChildAvatarGameObjectNames = HeadChildAvatarGameObjectNames.Concat(new string[] { ChildTransform.name }).ToArray();
			}
			if (!Array.Exists(ChildAvatarGameObjectNames, GameObjectName => GameObjectName.Contains("VRSuya_Mogumogu_PhysBone"))) {
				string[] PrefabFilePaths = new string[0];
				PrefabFilePaths = Mogumogu.PrefabGUID.Select(AssetGUID => AssetDatabase.GUIDToAssetPath(AssetGUID)).ToArray();
				string TargetPrefabPath = Array.Find(PrefabFilePaths, FilePath => FilePath.Split('/')[FilePath.Split('/').Length - 1].Contains("VRSuya_Mogumogu_PhysBone_" + AvatarType.ToString()));
				if (TargetPrefabPath != null) {
					GameObject TargetPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(TargetPrefabPath, typeof(GameObject));
					GameObject TargetInstance = (GameObject)PrefabUtility.InstantiatePrefab(TargetPrefab);
					TargetInstance.transform.parent = AvatarGameObject.transform;
					TransformPrefab(TargetInstance, AvatarGameObject, true);
					Undo.IncrementCurrentGroup();
					Undo.RecordObject(TargetInstance, "Added New GameObject");
				}
			}
			if (!Array.Exists(HeadChildAvatarGameObjectNames, GameObjectName => GameObjectName.Contains("VRSuya_Mogumogu_Particle"))) {
				string[] PrefabFilePaths = new string[0];
				PrefabFilePaths = Mogumogu.PrefabGUID.Select(AssetGUID => AssetDatabase.GUIDToAssetPath(AssetGUID)).ToArray();
				string TargetPrefabPath = Array.Find(PrefabFilePaths, FilePath => FilePath.Split('/')[FilePath.Split('/').Length - 1].Contains("VRSuya_Mogumogu_Particle_" + AvatarType.ToString()));
				if (TargetPrefabPath != null) {
					GameObject TargetPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(TargetPrefabPath, typeof(GameObject));
					GameObject TargetInstance = (GameObject)PrefabUtility.InstantiatePrefab(TargetPrefab);
					TargetInstance.transform.parent = AvatarAnimator.GetBoneTransform(HumanBodyBones.Head);
					TransformPrefab(TargetInstance, AvatarGameObject, true);
					Undo.IncrementCurrentGroup();
					Undo.RecordObject(TargetInstance, "Added New GameObject");
				}
			}
			GetVRSuyaGameObjects();
			VRSuyaMogumoguGameObject = Array.Find(VRSuyaGameObjects, gameObject => gameObject.name.Contains("VRSuya_Mogumogu_PhysBone"));
			return;
		}

		/// <summary>요청한 GameObject의 Transform을 Origin에 맞춰주는 메소드</summary>
		private static void TransformPrefab(GameObject TargetGameObject, GameObject BaseGameObject, bool KeepScale) {
			TargetGameObject.transform.localPosition = new Vector3(0, 0, 0);
			TargetGameObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
			if (!KeepScale) {
				TargetGameObject.transform.localScale = BaseGameObject.transform.localScale;
			} else {
				TargetGameObject.transform.localScale = new Vector3(1, 1, 1);
			}
			return;
		}

		/// <summary>볼 PhysBone 컴포넌트와 아바타의 볼 본과 연결합니다.</summary>
		private static void UpdatePhysBoneSetting() {
			VRCPhysBone[] VRSuyaMogumoguPhysBones = VRSuyaMogumoguGameObject.GetComponentsInChildren<VRCPhysBone>();
			if (VRSuyaMogumoguPhysBones != null) {
				Transform Cheek_L = null;
				Transform Cheek_R = null;
				VRCPhysBone PhysBone_Cheek_L = Array.Find(VRSuyaMogumoguPhysBones, PhysBone => PhysBone.gameObject.name == "Cheek_L");
				VRCPhysBone PhysBone_Cheek_R = Array.Find(VRSuyaMogumoguPhysBones, PhysBone => PhysBone.gameObject.name == "Cheek_R");
				if (AvatarCheekBoneTransforms != null) {
					foreach (Transform TargetTransform in AvatarCheekBoneTransforms) {
						switch (TargetTransform.gameObject.name) {
							case "Cheek1_L":
							case "Cheek_Root_L":
							case "Cheek_root_L":
								Cheek_L = TargetTransform;
								break;
							case "Cheek1_R":
							case "Cheek_Root_R":
							case "Cheek_root_R":
								Cheek_R = TargetTransform;
								break;
						}
					}
				}
				if (Cheek_L && PhysBone_Cheek_L) {
					Undo.IncrementCurrentGroup();
					Undo.RecordObject(PhysBone_Cheek_L, "Changed PhysBone Root Transform");
					PhysBone_Cheek_L.rootTransform = Cheek_L;
					EditorUtility.SetDirty(PhysBone_Cheek_L);
				}
				if (Cheek_R && PhysBone_Cheek_R) {
					Undo.IncrementCurrentGroup();
					Undo.RecordObject(PhysBone_Cheek_R, "Changed PhysBone Root Transform");
					PhysBone_Cheek_R.rootTransform = Cheek_R;
					EditorUtility.SetDirty(PhysBone_Cheek_R);
				}
			}
			return;
		}

		/// <summary>기존 아바타에 존재하는 PhysBone 컴포넌트를 비활성화 합니다.</summary>
		private static void DisableExistPhysBone() {
			if (AvatarCheekBoneTransforms.Length > 0) {
				foreach (Transform TargetTransform in AvatarCheekBoneTransforms) {
					if (TargetTransform.GetComponent<VRCPhysBone>()) {
						Undo.IncrementCurrentGroup();
						Undo.RecordObject(TargetTransform.GetComponent<VRCPhysBone>(), "Disabled PhysBone");
						TargetTransform.GetComponent<VRCPhysBone>().enabled = false;
						EditorUtility.SetDirty(TargetTransform.GetComponent<VRCPhysBone>());
					}
				}
			}
			return;
		}

		/// <summary>셀레스티아의 피직스본 애니메이터 레이어를 비활성화 합니다.</summary>
		private static void DisableExistMoumoguAnimatorLayer() {
			AnimatorController VRCFXLayer = (AnimatorController)Array.Find(AvatarVRCAvatarLayers, VRCAnimator => VRCAnimator.type == VRCAvatarDescriptor.AnimLayerType.FX).animatorController;
			if (VRCFXLayer) {
				foreach (string TargetLayerName in dictSELESTIAMogumoguLayerName) {
					int Index = Array.FindIndex(VRCFXLayer.layers, Layer => TargetLayerName == Layer.name);
					if (Index != -1) {
						Undo.IncrementCurrentGroup();
						Undo.RecordObject(VRCFXLayer, "Disabled Animator Controller Cheek Bone Layer");
						VRCFXLayer.layers[Index].defaultWeight = 0;
						EditorUtility.SetDirty(VRCFXLayer);
					}
				}
			}
			return;
		}
	}
}
#endif