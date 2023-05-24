﻿#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

using VRC.SDK3.Avatars.Components;
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
		protected AnimatorControllerLayer[] VRSuyaLocomotionLayers = new AnimatorControllerLayer[0];
		protected AnimatorControllerLayer[] VRSuyaGestureLayers = new AnimatorControllerLayer[0];
		protected AnimatorControllerLayer[] VRSuyaActionLayers = new AnimatorControllerLayer[0];
		protected AnimatorControllerLayer[] VRSuyaFXLayers = new AnimatorControllerLayer[0];

		protected AnimatorControllerParameter[] VRSuyaLocomotionParameters = new AnimatorControllerParameter[0];
		protected AnimatorControllerParameter[] VRSuyaGestureParameters = new AnimatorControllerParameter[0];
		protected AnimatorControllerParameter[] VRSuyaActionParameters = new AnimatorControllerParameter[0];
		protected AnimatorControllerParameter[] VRSuyaFXParameters = new AnimatorControllerParameter[0];

		// 추가될 VRCSDK 데이터
		protected List<VRCExpressionsMenu.Control> VRSuyaMenus = new List<VRCExpressionsMenu.Control>();
		protected VRCExpressionParameters.Parameter[] VRSuyaParameters = new VRCExpressionParameters.Parameter[0];

		/// <summary>상속 클래스가 존재하는지 확인 한 후 해당 제품의 최종 세팅 요청을 합니다.</summary>
		internal void RequestSetup() {
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_AFK))) {
				if (Array.Exists(RequestSetupVRSuyaProductList, Product => Product.ProductName == ProductName.AFK)) {
					ProductSetup_AFK AFKProcess = new ProductSetup_AFK();
					AFKProcess.RequestSetting();
				}
			}
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Mogumogu))) {
				if (Array.Exists(RequestSetupVRSuyaProductList, Product => Product.ProductName == ProductName.Mogumogu)) {
					ProductSetup_Mogumogu MogumoguProcess = new ProductSetup_Mogumogu();
					MogumoguProcess.RequestSetting();
				}
			}
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Wotagei))) {
				if (Array.Exists(RequestSetupVRSuyaProductList, Product => Product.ProductName == ProductName.Wotagei)) {
					ProductSetup_Wotagei WotageiProcess = new ProductSetup_Wotagei();
					WotageiProcess.RequestSetting();
				}
			}
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Feet))) {
				if (Array.Exists(RequestSetupVRSuyaProductList, Product => Product.ProductName == ProductName.Feet)) {
					ProductSetup_Feet FeetProcess = new ProductSetup_Feet();
					FeetProcess.RequestSetting();
				}
			}
			if (RequestSetupVRSuyaProductList.Length > 0) {
				foreach (var RequestedAllLayers in RequestSetupVRSuyaProductList.Select(Product => Product.RequiredAnimatorLayers)) {
					foreach (KeyValuePair<VRCAvatarDescriptor.AnimLayerType, AnimatorControllerLayer[]> RequestedLayers in RequestedAllLayers) {
						switch (RequestedLayers.Key) {
							case VRCAvatarDescriptor.AnimLayerType.Base:
								VRSuyaLocomotionLayers = VRSuyaLocomotionLayers.Concat(RequestedLayers.Value).ToArray();
								break;
							case VRCAvatarDescriptor.AnimLayerType.Gesture:
								VRSuyaGestureLayers = VRSuyaGestureLayers.Concat(RequestedLayers.Value).ToArray();
								break;
							case VRCAvatarDescriptor.AnimLayerType.Action:
								VRSuyaActionLayers = VRSuyaActionLayers.Concat(RequestedLayers.Value).ToArray();
								break;
							case VRCAvatarDescriptor.AnimLayerType.FX:
								VRSuyaFXLayers = VRSuyaFXLayers.Concat(RequestedLayers.Value).ToArray();
								break;
						}
					}
				}
				foreach (var RequestedAllParameters in RequestSetupVRSuyaProductList.Select(Product => Product.RequiredAnimatorParameters)) {
					foreach (KeyValuePair<VRCAvatarDescriptor.AnimLayerType, AnimatorControllerParameter[]> RequestedParamter in RequestedAllParameters) {
						switch (RequestedParamter.Key) {
							case VRCAvatarDescriptor.AnimLayerType.Base:
								VRSuyaLocomotionParameters = VRSuyaLocomotionParameters.Concat(RequestedParamter.Value).ToArray();
								break;
							case VRCAvatarDescriptor.AnimLayerType.Gesture:
								VRSuyaGestureParameters = VRSuyaGestureParameters.Concat(RequestedParamter.Value).ToArray();
								break;
							case VRCAvatarDescriptor.AnimLayerType.Action:
								VRSuyaActionParameters = VRSuyaActionParameters.Concat(RequestedParamter.Value).ToArray();
								break;
							case VRCAvatarDescriptor.AnimLayerType.FX:
								VRSuyaFXParameters = VRSuyaFXParameters.Concat(RequestedParamter.Value).ToArray();
								break;
						}
					}
				}
				foreach (var RequestedMenus in RequestSetupVRSuyaProductList.Select(Product => Product.RequiredVRCMenus)) {
					VRSuyaMenus.AddRange(RequestedMenus);
				}
				foreach (var RequestedParameters in RequestSetupVRSuyaProductList.Select(Product => Product.RequiredVRCParameters)) {
					VRSuyaParameters = VRSuyaParameters.Concat(RequestedParameters).ToArray();
				}
			}
			bool[] RequestUnityAnimator = new bool[] { VRSuyaLocomotionLayers.Any(), VRSuyaGestureLayers.Any(), VRSuyaActionLayers.Any(), VRSuyaFXLayers.Any(),
				VRSuyaLocomotionParameters.Any(), VRSuyaGestureParameters.Any(), VRSuyaActionParameters.Any(), VRSuyaFXParameters.Any() };
			if (RequestUnityAnimator.Any(Result => Result == true)) UpdateUnityAnimator();
			if (VRSuyaParameters.Length > 0) UpdateAvatarParameters();
			if (VRSuyaMenus.Count > 0) UpdateAvatarMenus();
			return;
		}

		/// <summary>상속 클래스가 존재하는지 확인 한 후 해당 제품의 업데이트 및 등록 요청을 합니다.</summary>
		internal void RequestProductRegister() {
			InstalledVRSuyaProducts = new VRSuyaProduct[0];
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_AFK))) {
				ProductSetup_AFK AFKProcess = new ProductSetup_AFK();
				AFKProcess.RegisterProduct();
			}
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Mogumogu))) {
				ProductSetup_Mogumogu MogumoguProcess = new ProductSetup_Mogumogu();
				MogumoguProcess.RegisterProduct();
			}
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Wotagei))) {
				ProductSetup_Wotagei WotageiProcess = new ProductSetup_Wotagei();
				WotageiProcess.RegisterProduct();
			}
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Feet))) {
				ProductSetup_Feet FeetProcess = new ProductSetup_Feet();
				FeetProcess.RegisterProduct();
			}
			UpdateInstalledAvatarList();
			return;
		}

		/// <summary>아바타 하위의 GameObject들을 검사하여 VRSuya GameObject 목록을 작성합니다.</summary>
		/// <returns>VRSuya 이름이 들어간 GameObject 배열</returns>
		internal GameObject[] GetVRSuyaGameObjects() {
			VRSuyaGameObjects = Array.FindAll(AvatarGameObject.GetComponentsInChildren<Transform>(true), transform => transform.gameObject.name.Contains("VRSuya")).Select(transform => transform.gameObject).ToArray();
			VRSuyaGameObjects = VRSuyaGameObjects.Where(gameObject => gameObject != AvatarGameObject).ToArray();
			return VRSuyaGameObjects;
		}

		/// <summary>설치된 VRSuya 제품에서 지원하는 아바타 목록을 추려 사용 가능한 아바타 목록을 만듭니다.</summary>
		private void UpdateInstalledAvatarList() {
			Avatar[] AllInstalledAvatars = InstalledVRSuyaProducts.SelectMany(Product => Product.SupportAvatarList).ToArray();
			InstalledVRSuyaProductAvatars = AllInstalledAvatars.Distinct().ToArray();
			return;
		}

		/// <summary>아바타의 각 VRC 애니메이터에 세팅해야 하는 값을 보냅니다.</summary>
		private void UpdateUnityAnimator() {
			AnimatorController VRCLocomotionLayer = (AnimatorController)Array.Find(AvatarVRCAvatarLayers, VRCAnimator => VRCAnimator.type == VRCAvatarDescriptor.AnimLayerType.Base).animatorController;
			AnimatorController VRCGestureLayer = (AnimatorController)Array.Find(AvatarVRCAvatarLayers, VRCAnimator => VRCAnimator.type == VRCAvatarDescriptor.AnimLayerType.Gesture).animatorController;
			AnimatorController VRCActionLayer = (AnimatorController)Array.Find(AvatarVRCAvatarLayers, VRCAnimator => VRCAnimator.type == VRCAvatarDescriptor.AnimLayerType.Action).animatorController;
			AnimatorController VRCFXLayer = (AnimatorController)Array.Find(AvatarVRCAvatarLayers, VRCAnimator => VRCAnimator.type == VRCAvatarDescriptor.AnimLayerType.FX).animatorController;
			if (VRCLocomotionLayer) UpdateTargetAnimatorController(VRCLocomotionLayer, VRSuyaLocomotionLayers, VRSuyaLocomotionParameters);
			if (VRCGestureLayer) UpdateTargetAnimatorController(VRCGestureLayer, VRSuyaGestureLayers, VRSuyaGestureParameters);
			if (VRCActionLayer) UpdateTargetAnimatorController(VRCActionLayer, VRSuyaActionLayers, VRSuyaActionParameters);
			if (VRCFXLayer) UpdateTargetAnimatorController(VRCFXLayer, VRSuyaFXLayers, VRSuyaFXParameters);
			return;
		}

		/// <summary>세팅해야 하는 애니메이터 컨트롤러에 파라메터와 레이어가 존재하는지 확인 후 추가합니다.</summary>
		private void UpdateTargetAnimatorController(AnimatorController TargetController, AnimatorControllerLayer[] TargetLayers, AnimatorControllerParameter[] TargetParameters) {
			foreach (AnimatorControllerParameter NewParameter in TargetParameters) {
				if (!Array.Exists(TargetController.parameters, ExistParameter => NewParameter.name == ExistParameter.name)) {
					TargetController.parameters = TargetController.parameters.Concat(new AnimatorControllerParameter[] { NewParameter }).ToArray();
					EditorUtility.SetDirty(TargetController);
				}
			}
			foreach (AnimatorControllerLayer NewLayer in TargetLayers) {
				if (!Array.Exists(TargetController.layers, ExistLayer => NewLayer.name == ExistLayer.name)) {
					TargetController.layers = TargetController.layers.Concat(new AnimatorControllerLayer[] { NewLayer }).ToArray();
					EditorUtility.SetDirty(TargetController);
				}
			}
			return;
		}

		/// <summary>세팅해야 하는 파라메터 큐를 아바타 파라메터에 존재하는지 확인 후 추가합니다.</summary>
		private void UpdateAvatarParameters() {
            foreach (VRCExpressionParameters.Parameter NewParameter in VRSuyaParameters) {
                if (!Array.Exists(AvatarVRCParameter.parameters, ExistParameter => ExistParameter.name == NewParameter.name)) {
					AvatarVRCParameter.parameters = AvatarVRCParameter.parameters.Concat(new VRCExpressionParameters.Parameter[] { NewParameter }).ToArray();
				}
            }
			EditorUtility.SetDirty(AvatarVRCParameter);
			return;
        }

		/// <summary>세팅해야 하는 메뉴 큐를 아바타 메뉴에 존재하는지 확인 후 추가합니다.</summary>
		private void UpdateAvatarMenus() {
            foreach (VRCExpressionsMenu.Control NewMenu in VRSuyaMenus) {
                if (!AvatarVRCMenu.controls.Exists(ExistMenu => ExistMenu.subMenu == NewMenu.subMenu)) {
					AvatarVRCMenu.controls.Add(NewMenu);
				}
			}
			EditorUtility.SetDirty(AvatarVRCMenu);
			return;
		}
	}
}
#endif