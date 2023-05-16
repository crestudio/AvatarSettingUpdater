#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using VRC.SDK3.Avatars.ScriptableObjects;

/*
 * VRSuya Avatar Setting Updater
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.AvatarSettingUpdater {

	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class ProductSetup : AvatarSettingUpdater {

		// 추가될 Unity 데이터
		protected static AnimatorControllerParameter[] VRSuyaLocomotionParameters = new AnimatorControllerParameter[0];
		protected static AnimatorControllerParameter[] VRSuyaGestureParameters = new AnimatorControllerParameter[0];
		protected static AnimatorControllerParameter[] VRSuyaActionParameters = new AnimatorControllerParameter[0];
		protected static AnimatorControllerParameter[] VRSuyaFXParameters = new AnimatorControllerParameter[0];

		// 추가될 VRCSDK 데이터
		protected static List<VRCExpressionsMenu.Control> VRSuyaMenus = new List<VRCExpressionsMenu.Control>();
		protected static VRCExpressionParameters.Parameter[] VRSuyaParameters = new VRCExpressionParameters.Parameter[0];

		/// <summary>상속 클래스가 존재하는지 확인 한 후 해당 제품의 최종 세팅 요청을 합니다.</summary>
		internal static void RequestSetup() {
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_AFK))) {
				if (Array.Exists(RequestSetupVRSuyaProductList, Product => Product.ProductName == ProductName.AFK)) ProductSetup_AFK.RequestSetting();
			}
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Mogumogu))) {
				if (Array.Exists(RequestSetupVRSuyaProductList, Product => Product.ProductName == ProductName.Mogumogu)) ProductSetup_Mogumogu.RequestSetting();
			}
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Wotagei))) {
				if (Array.Exists(RequestSetupVRSuyaProductList, Product => Product.ProductName == ProductName.Wotagei)) ProductSetup_Wotagei.RequestSetting();
			}
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Feet))) {
				if (Array.Exists(RequestSetupVRSuyaProductList, Product => Product.ProductName == ProductName.Feet)) ProductSetup_Feet.RequestSetting();
			}
			if (RequestSetupVRSuyaProductList.Length > 0) {
				foreach (var RequestedMenus in RequestSetupVRSuyaProductList.Select(Product => Product.RequiredVRCMenus)) {
					VRSuyaMenus.AddRange(RequestedMenus);
				}
				foreach (var RequestedParameters in RequestSetupVRSuyaProductList.Select(Product => Product.RequiredVRCParameters)) {
					VRSuyaParameters = VRSuyaParameters.Concat(RequestedParameters).ToArray();
				}
			}
			if (VRSuyaParameters.Length > 0) UpdateAvatarParameters();
			if (VRSuyaMenus.Count > 0) UpdateAvatarMenus();
			return;
		}

		/// <summary>상속 클래스가 존재하는지 확인 한 후 해당 제품의 업데이트 및 등록 요청을 합니다.</summary>
		internal static void RequestProductRegister() {
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_AFK))) ProductSetup_AFK.RegisterProduct();
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Mogumogu))) ProductSetup_Mogumogu.RegisterProduct();
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Wotagei))) ProductSetup_Wotagei.RegisterProduct();
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Feet))) ProductSetup_Feet.RegisterProduct();
			UpdateInstalledAvatarList();
			return;
		}

		/// <summary>아바타 하위의 GameObject들을 검사하여 VRSuya GameObject 목록을 작성합니다.</summary>
		/// <returns>VRSuya 이름이 들어간 GameObject 배열</returns>
		internal static GameObject[] GetVRSuyaGameObjects() {
			VRSuyaGameObjects = Array.FindAll(AvatarGameObject.GetComponentsInChildren<Transform>(true), transform => transform.gameObject.name.Contains("VRSuya")).Select(transform => transform.gameObject).ToArray();
			VRSuyaGameObjects = VRSuyaGameObjects.Where(gameObject => gameObject != AvatarGameObject).ToArray();
			return VRSuyaGameObjects;
		}

		/// <summary>설치된 VRSuya 제품에서 지원하는 아바타 목록을 추려 사용 가능한 아바타 목록을 만듭니다.</summary>
		private static void UpdateInstalledAvatarList() {
			Avatar[] AllInstalledAvatars = InstalledVRSuyaProducts.SelectMany(Product => Product.SupportAvatarList).ToArray();
			InstalledVRSuyaProductAvatars = AllInstalledAvatars.Distinct().ToArray();
			return;
		}

		/// <summary>아바타의 VRC 애니메이터에 세팅해야 하는 값을 보냅니다.</summary>
		private static void UpdateUnityAnimator() {
			return;
		}

		/// <summary>세팅해야 하는 파라메터 큐를 아바타 파라메터에 존재하는지 확인 후 추가합니다.</summary>
		private static void UpdateAvatarParameters() {
            foreach (var NewParameter in VRSuyaParameters) {
                if (!Array.Exists(AvatarVRCParameter.parameters, ExistParameter => ExistParameter.name == NewParameter.name)) {
					AvatarVRCParameter.parameters = AvatarVRCParameter.parameters.Concat(new VRCExpressionParameters.Parameter[] { NewParameter }).ToArray();
				}
            }
			return;
        }

		/// <summary>세팅해야 하는 메뉴 큐를 아바타 메뉴에 존재하는지 확인 후 추가합니다.</summary>
		private static void UpdateAvatarMenus() {
            foreach (VRCExpressionsMenu.Control NewMenu in VRSuyaMenus) {
                if (!AvatarVRCMenu.controls.Exists(ExistMenu => ExistMenu.subMenu == NewMenu.subMenu)) {
					AvatarVRCMenu.controls.Add(NewMenu);
				}
			}
		}
	}
}
#endif