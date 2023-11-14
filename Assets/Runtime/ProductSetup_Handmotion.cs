#if UNITY_EDITOR
using System;
using System.Linq;

using UnityEditor;
using UnityEngine;

/*
 * VRSuya Avatar Setting Updater
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.avatarsettingupdater {

	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class ProductSetup_Handmotion : ProductSetup {

		private static VRSuyaProduct Handmotion;
		private static GameObject VRSuyaHandmotionGameObject;

		/// <summary>제품 정보를 AssetManager에게 요청하여 업데이트 한 후, 설치된 에셋 목록에 추가합니다.</summary>
		internal static void RegisterProduct() {
			InstalledProductHandmotion = false;
			Handmotion = new VRSuyaProduct();
			Handmotion = AssetManager.UpdateProductInformation(ProductName.Handmotion);
			InstalledVRSuyaProducts = InstalledVRSuyaProducts.Concat(new VRSuyaProduct[] { Handmotion }).ToArray();
			if (Handmotion.SupportAvatarList.Length > 0) InstalledProductHandmotion = true;
			return;
		}

		/// <summary>외부의 세팅 요청을 처리하는 메인 메소드 입니다.</summary>
		internal static void RequestSetting() {
			if (InstallProductHandmotion) {
				VRSuyaHandmotionGameObject = Array.Find(VRSuyaGameObjects, gameObject => gameObject.name.Contains("VRSuya_Handmotion"));
				if (!VRSuyaHandmotionGameObject) SetupPrefab();
			}
			return;
		}

		/// <summary>아바타에 Prefab이 있는지 검사하고 없으면 설치하는 메소드 입니다.</summary>
		private static void SetupPrefab() {
			string[] ChildAvatarGameObjectNames = new string[0];
			foreach (Transform ChildTransform in AvatarGameObject.transform) {
				ChildAvatarGameObjectNames = ChildAvatarGameObjectNames.Concat(new string[] { ChildTransform.name }).ToArray();
			}
			if (!Array.Exists(ChildAvatarGameObjectNames, GameObjectName => GameObjectName.Contains("VRSuya_Handmotion"))) {
				string[] PrefabFilePaths = new string[0];
				PrefabFilePaths = Handmotion.PrefabGUID.Select(AssetGUID => AssetDatabase.GUIDToAssetPath(AssetGUID)).ToArray();
				string TargetPrefabPath = Array.Find(PrefabFilePaths, FilePath => FilePath.Split('/')[FilePath.Split('/').Length - 1].Contains("VRSuya_Handmotion_" + AvatarType.ToString()));
				if (string.IsNullOrEmpty(TargetPrefabPath)) TargetPrefabPath = Array.Find(PrefabFilePaths, FilePath => FilePath.Split('/')[FilePath.Split('/').Length - 1].Contains("VRSuya_Handmotion"));
				if (TargetPrefabPath != null) {
					GameObject TargetPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(TargetPrefabPath, typeof(GameObject));
					GameObject TargetInstance = (GameObject)PrefabUtility.InstantiatePrefab(TargetPrefab);
					Undo.RegisterCreatedObjectUndo(TargetInstance, "Added New GameObject");
					TargetInstance.transform.parent = AvatarGameObject.transform;
					TransformPrefab(TargetInstance, AvatarGameObject, true);
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
			}
			GetVRSuyaGameObjects();
			VRSuyaHandmotionGameObject = Array.Find(VRSuyaGameObjects, gameObject => gameObject.name.Contains("VRSuya_Handmotion"));
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
	}
}
#endif