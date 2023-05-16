﻿#if UNITY_EDITOR
using System;
using System.Linq;

using UnityEngine;

using VRC.SDK3.Dynamics.PhysBone.Components;

/*
 * VRSuya Avatar Setting Updater
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.AvatarSettingUpdater {

	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class ProductSetup_Mogumogu : ProductSetup {

		private static VRSuyaProduct Mogumogu = new VRSuyaProduct();

		private static GameObject VRSuyaMogumoguGameObject = Array.Find(VRSuyaGameObjects, gameObject => gameObject.name.Contains("VRSuya_Mogumogu_PhysBone"));
		private static Transform[] AvatarCheekBoneTransforms = Array.FindAll(AvatarAnimator.GetBoneTransform(HumanBodyBones.Head).GetComponentsInChildren<Transform>(true), transform => transform.name.Contains("Cheek"));

		/// <summary>제품 정보를 AssetManager에게 요청하여 업데이트 한 후, 설치된 에셋 목록에 추가합니다.</summary>
		internal static void RegisterProduct() {
			Mogumogu = AssetManager.UpdateProductInformation(ProductName.Mogumogu);
			InstalledVRSuyaProducts = InstalledVRSuyaProducts.Concat(new VRSuyaProduct[] { Mogumogu }).ToArray();
			InstalledProductMogumogu = true;
			return;
		}

		/// <summary>외부의 세팅 요청을 처리하는 메인 메소드 입니다.</summary>
		internal static void RequestSetting() {
			if (VRSuyaMogumoguGameObject) {
				UpdatePhysBoneSetting();
				DisableExistPhysBone();
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
				if (Cheek_L && PhysBone_Cheek_L) PhysBone_Cheek_L.rootTransform = Cheek_L;
				if (Cheek_R && PhysBone_Cheek_R) PhysBone_Cheek_R.rootTransform = Cheek_R;
			}
			return;
		}

		/// <summary>기존 아바타에 존재하는 PhysBone 컴포넌트를 비활성화 합니다.</summary>
		private static void DisableExistPhysBone() {
			if (AvatarCheekBoneTransforms.Length > 0) {
				foreach (Transform TargetTransform in AvatarCheekBoneTransforms) {
					if (TargetTransform.GetComponent<VRCPhysBone>()) {
						TargetTransform.GetComponent<VRCPhysBone>().enabled = false;
					}
				}
			}
			return;
		}
	}
}
#endif