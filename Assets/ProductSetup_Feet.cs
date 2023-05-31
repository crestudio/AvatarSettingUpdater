#if UNITY_EDITOR
using System;
using System.Linq;

using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

using VRC.SDK3.Dynamics.PhysBone.Components;

/*
 * VRSuya Avatar Setting Updater
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.AvatarSettingUpdater {

	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class ProductSetup_Feet : ProductSetup {

		private static VRSuyaProduct Feet;
		private static GameObject VRSuyaHopedskyDFeetGameObject;
		private static Transform[] FeetTransforms;
		private static readonly string[] dictToeName = {
			"ThumbToe1_L", "ThumbToe2_L", "ThumbToe3_L",
			"ThumbToe1_R", "ThumbToe2_R", "ThumbToe3_R",
			"IndexToe1_L", "IndexToe2_L", "IndexToe3_L",
			"IndexToe1_R", "IndexToe2_R", "IndexToe3_R",
			"MiddleToe1_L", "MiddleToe2_L", "MiddleToe3_L",
			"MiddleToe1_R", "MiddleToe2_R", "MiddleToe3_R",
			"RingToe1_L", "RingToe2_L", "RingToe3_L",
			"RingToe1_R", "RingToe2_R", "RingToe3_R",
			"LittleToe1_L", "LittleToe2_L", "LittleToe3_L",
			"LittleToe1_R", "LittleToe2_R", "LittleToe3_R"
		};

		/// <summary>제품 정보를 AssetManager에게 요청하여 업데이트 한 후, 설치된 에셋 목록에 추가합니다.</summary>
		internal static void RegisterProduct() {
			Feet = new VRSuyaProduct();
			Feet = AssetManager.UpdateProductInformation(ProductName.Feet);
			InstalledVRSuyaProducts = InstalledVRSuyaProducts.Concat(new VRSuyaProduct[] { Feet }).ToArray();
			InstalledProductFeet = true;
			return;
		}

		/// <summary>외부의 세팅 요청을 처리하는 메인 메소드 입니다.</summary>
		internal static void RequestSetting() {
			if (InstallProductFeet) {
				VRSuyaHopedskyDFeetGameObject = Array.Find(VRSuyaGameObjects, gameObject => gameObject.name.Contains("VRSuya_HopeskyD_Feet"));
				if (!VRSuyaHopedskyDFeetGameObject) SetupPrefab();
				if (VRSuyaHopedskyDFeetGameObject) {
					GetFeetTransforms();
					UpdateParentConstraints();
					UpdatePhysBones();
					UpdatePrefabName();
				}
			}
			return;
		}

		/// <summary>아바타에 Prefab이 있는지 검사하고 없으면 설치하는 메소드 입니다.</summary>
		private static void SetupPrefab() {
			string[] ChildAvatarGameObjectNames = new string[0];
			foreach (Transform ChildTransform in AvatarGameObject.transform) {
				ChildAvatarGameObjectNames = ChildAvatarGameObjectNames.Concat(new string[] { ChildTransform.name }).ToArray();
			}
			if (!Array.Exists(ChildAvatarGameObjectNames, GameObjectName => GameObjectName.Contains("VRSuya_HopeskyD_Feet"))) {
				string[] PrefabFilePaths = new string[0];
				PrefabFilePaths = Feet.PrefabGUID.Select(AssetGUID => AssetDatabase.GUIDToAssetPath(AssetGUID)).ToArray();
				string TargetPrefabPath = Array.Find(PrefabFilePaths, FilePath => FilePath.Split('/')[FilePath.Split('/').Length - 1].Contains("VRSuya_HopeskyD_Feet_" + AvatarType.ToString()));
				if (TargetPrefabPath != null) {
					GameObject TargetPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(TargetPrefabPath, typeof(GameObject));
					GameObject TargetInstance = (GameObject)PrefabUtility.InstantiatePrefab(TargetPrefab);
					Undo.RecordObject(TargetInstance, "Added New GameObject");
					TargetInstance.transform.parent = AvatarGameObject.transform;
					TransformPrefab(TargetInstance, AvatarGameObject, false);
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
			}
			GetVRSuyaGameObjects();
			VRSuyaHopedskyDFeetGameObject = Array.Find(VRSuyaGameObjects, gameObject => gameObject.name.Contains("VRSuya_HopeskyD_Feet"));
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

		/// <summary>발 하위의 모든 Transform을 Array에 추가합니다.</summary>
		private static void GetFeetTransforms() {
			FeetTransforms = new Transform[0];
			FeetTransforms = FeetTransforms.Concat(AvatarAnimator.GetBoneTransform(HumanBodyBones.LeftFoot).GetComponentsInChildren<Transform>(true)).ToArray();
			FeetTransforms = FeetTransforms.Concat(AvatarAnimator.GetBoneTransform(HumanBodyBones.RightFoot).GetComponentsInChildren<Transform>(true)).ToArray();
			return;
		}

		/// <summary>Parent Constraint 컴포넌트와 아바타를 연결합니다.</summary>
		private static void UpdateParentConstraints() {
			GameObject ArmatureGameObject = Array.Find(VRSuyaHopedskyDFeetGameObject.GetComponentsInChildren<Transform>(true), transform => transform.gameObject.name == "Armature").gameObject;
			if (ArmatureGameObject) {
				ParentConstraint[] AnchorParentConstraints = ArmatureGameObject.GetComponentsInChildren<ParentConstraint>();
				if (AnchorParentConstraints != null) {
					foreach (ParentConstraint TargetParentConstraint in AnchorParentConstraints) {
						Transform TargetTransform = null;
						switch (TargetParentConstraint.gameObject.name) {
							case ("Left ankle"):
								TargetTransform = AvatarAnimator.GetBoneTransform(HumanBodyBones.LeftFoot);
								break;
							case ("Right ankle"):
								TargetTransform = AvatarAnimator.GetBoneTransform(HumanBodyBones.RightFoot);
								break;
							case ("Left toe"):
								TargetTransform = AvatarAnimator.GetBoneTransform(HumanBodyBones.LeftToes);
								break;
							case ("Right toe"):
								TargetTransform = AvatarAnimator.GetBoneTransform(HumanBodyBones.RightToes);
								break;
						}
						if (TargetTransform) {
							Undo.RecordObject(TargetParentConstraint, "Changed Parent Constraint");
							TargetParentConstraint.SetSource(0, new ConstraintSource() { sourceTransform = TargetTransform, weight = 1 });
							TargetParentConstraint.constraintActive = true;
							EditorUtility.SetDirty(TargetParentConstraint);
							Undo.CollapseUndoOperations(UndoGroupIndex);
						}
					}
				}
			}
			return;
		}

		/// <summary>발가락 PhysBone 컴포넌트와 아바타의 발가락을 연결합니다.</summary>
		private static void UpdatePhysBones() {
			GameObject FeetPhysBoneGameObject = Array.Find(VRSuyaHopedskyDFeetGameObject.GetComponentsInChildren<Transform>(true), transform => transform.gameObject.name == "PhysBone").gameObject;
			foreach (Transform TargetTransform in FeetPhysBoneGameObject.GetComponentsInChildren<Transform>(true)) {
				if (Array.Exists(dictToeName, ToeName => TargetTransform.name == ToeName)) {
					VRCPhysBone ToePhysBone = TargetTransform.GetComponent<VRCPhysBone>();
					if (ToePhysBone) {
						Transform TargetToeTransform = Array.Find(FeetTransforms, FeetTransform => TargetTransform.name == FeetTransform.name);
						if (TargetTransform) {
							Undo.RecordObject(ToePhysBone, "Changed PhysBone Root Transform");
							ToePhysBone.rootTransform = TargetToeTransform;
							EditorUtility.SetDirty(ToePhysBone);
							Undo.CollapseUndoOperations(UndoGroupIndex);
						}
					}
				}
			}
			return;
		}

		/// <summary>Prefab의 이름을 애니메이션 Path 규격에 맞춰 변경합니다.</summary>
		private static void UpdatePrefabName() {
			if (VRSuyaHopedskyDFeetGameObject.name != "VRSuya_HopeskyD_Feet") {
				Undo.RecordObject(VRSuyaHopedskyDFeetGameObject, "Changed GameObject Name");
				VRSuyaHopedskyDFeetGameObject.name = "VRSuya_HopeskyD_Feet";
				EditorUtility.SetDirty(VRSuyaHopedskyDFeetGameObject);
				Undo.CollapseUndoOperations(UndoGroupIndex);
			}
		}
	}
}
#endif