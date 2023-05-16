﻿#if UNITY_EDITOR
using System;
using System.Linq;

using UnityEngine;
using UnityEngine.Animations;

/*
 * VRSuya Avatar Setting Updater
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.AvatarSettingUpdater {

	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class ProductSetup_Wotagei : ProductSetup {

		private static VRSuyaProduct Wotagei = new VRSuyaProduct();

		private static GameObject VRSuyaWotageiGameObject = Array.Find(VRSuyaGameObjects, gameObject => gameObject.name.Contains("VRSuya_Wotagei"));

		/// <summary>제품 정보를 AssetManager에게 요청하여 업데이트 한 후, 설치된 에셋 목록에 추가합니다.</summary>
		internal static void RegisterProduct() {
			Wotagei = AssetManager.UpdateProductInformation(ProductName.Wotagei);
			InstalledVRSuyaProducts = InstalledVRSuyaProducts.Concat(new VRSuyaProduct[] { Wotagei }).ToArray();
			InstalledProductWotagei = true;
			return;
		}

		/// <summary>외부의 세팅 요청을 처리하는 메인 메소드 입니다.</summary>
		internal static void RequestSetting() {
			if (VRSuyaWotageiGameObject) {
				UpdateParentConstraints();
				UpdatePrefabName();
			}
			return;
		}

		/// <summary>Parent Constraint 컴포넌트와 아바타의 손을 연결합니다.</summary>
		private static void UpdateParentConstraints() {
			GameObject LeftHandGameObject = Array.Find(VRSuyaWotageiGameObject.GetComponentsInChildren<Transform>(true), gameObject => gameObject.name == "LeftHand").gameObject;
			GameObject RightHandGameObject = Array.Find(VRSuyaWotageiGameObject.GetComponentsInChildren<Transform>(true), gameObject => gameObject.name == "RightHand").gameObject;
			if (LeftHandGameObject) {
				ParentConstraint AnchorParentConstraint = LeftHandGameObject.GetComponent<ParentConstraint>();
				if (AnchorParentConstraint) {
					AnchorParentConstraint.SetSource(0, new ConstraintSource() { sourceTransform = AvatarAnimator.GetBoneTransform(HumanBodyBones.LeftHand), weight = 1 });
					AnchorParentConstraint.constraintActive = true;
				}
			}
			if (RightHandGameObject) {
				ParentConstraint AnchorParentConstraint = RightHandGameObject.GetComponent<ParentConstraint>();
				if (AnchorParentConstraint) {
					AnchorParentConstraint.SetSource(0, new ConstraintSource() { sourceTransform = AvatarAnimator.GetBoneTransform(HumanBodyBones.RightHand), weight = 1 });
					AnchorParentConstraint.constraintActive = true;
				}
			}
			return;
		}

		/// <summary>Prefab의 이름을 애니메이션 Path 규격에 맞춰 변경합니다.</summary>
		private static void UpdatePrefabName() {
			if (VRSuyaWotageiGameObject.name != "VRSuya_Wotagei") {
				VRSuyaWotageiGameObject.name = "VRSuya_Wotagei";
			}
		}
	}
}
#endif