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
	public class ProductSetup_ModelWalking : ProductSetup {

		private static VRSuyaProduct ModelWalking;
		private static GameObject VRSuyaModelWalkingGameObject;

		/// <summary>제품 정보를 AssetManager에게 요청하여 업데이트 한 후, 설치된 에셋 목록에 추가합니다.</summary>
		internal static void RegisterProduct() {
			InstalledProductModelWalking = false;
			ModelWalking = new VRSuyaProduct();
			ModelWalking = AssetManager.UpdateProductInformation(ProductName.ModelWalking);
			InstalledVRSuyaProducts = InstalledVRSuyaProducts.Concat(new VRSuyaProduct[] { ModelWalking }).ToArray();
			if (ModelWalking.SupportAvatarList.Length > 0) InstalledProductModelWalking = true;
			PrintProductInformation(ModelWalking);
			return;
		}

		/// <summary>외부의 세팅 요청을 처리하는 메인 메소드 입니다.</summary>
		internal static void RequestSetting() {
			if (InstallProductModelWalking) {
				VRSuyaModelWalkingGameObject = Array.Find(VRSuyaGameObjects, gameObject => gameObject.name.Contains("VRSuya_ModelWalking"));
				if (!VRSuyaModelWalkingGameObject) SetupPrefab();
				if (!AvatarAnimator.applyRootMotion) ApplyRootMotion();
			}
			return;
		}

		/// <summary>아바타에 Prefab이 있는지 검사하고 없으면 설치하는 메소드 입니다.</summary>
		private static void SetupPrefab() {
			string[] ChildAvatarGameObjectNames = new string[0];
			foreach (Transform ChildTransform in AvatarGameObject.transform) {
				ChildAvatarGameObjectNames = ChildAvatarGameObjectNames.Concat(new string[] { ChildTransform.name }).ToArray();
			}
			if (!Array.Exists(ChildAvatarGameObjectNames, GameObjectName => GameObjectName.Contains("VRSuya_ModelWalking"))) {
				string[] PrefabFilePaths = new string[0];
				PrefabFilePaths = ModelWalking.PrefabGUID.Select(AssetGUID => AssetDatabase.GUIDToAssetPath(AssetGUID)).ToArray();
				string TargetPrefabPath = Array.Find(PrefabFilePaths, FilePath => FilePath.Split('/')[FilePath.Split('/').Length - 1].Contains("VRSuya_ModelWalking_" + AvatarType.ToString()));
				if (string.IsNullOrEmpty(TargetPrefabPath)) TargetPrefabPath = Array.Find(PrefabFilePaths, FilePath => FilePath.Split('/')[FilePath.Split('/').Length - 1].Contains("VRSuya_ModelWalking"));
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
			VRSuyaModelWalkingGameObject = Array.Find(VRSuyaGameObjects, gameObject => gameObject.name.Contains("VRSuya_ModelWalking"));
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

		/// <summary>아바타의 Root Motion을 활성화하는 메소드</summary>
		private static void ApplyRootMotion() {
			Undo.RecordObject(AvatarAnimator, "Changed Avatar Root Motion");
			AvatarAnimator.applyRootMotion = true;
			EditorUtility.SetDirty(AvatarAnimator);
			Undo.CollapseUndoOperations(UndoGroupIndex);
		}
	}
}
#endif