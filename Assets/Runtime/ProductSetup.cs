#if UNITY_EDITOR
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

namespace com.vrsuya.avatarsettingupdater {

	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class ProductSetup : AvatarSettingUpdater {

		// 추가될 Unity 데이터
		protected static AnimatorControllerLayer[] VRSuyaLocomotionLayers;
		protected static AnimatorControllerLayer[] VRSuyaGestureLayers;
		protected static AnimatorControllerLayer[] VRSuyaActionLayers;
		protected static AnimatorControllerLayer[] VRSuyaFXLayers;

		protected static AnimatorControllerParameter[] VRSuyaLocomotionParameters;
		protected static AnimatorControllerParameter[] VRSuyaGestureParameters;
		protected static AnimatorControllerParameter[] VRSuyaActionParameters;
		protected static AnimatorControllerParameter[] VRSuyaFXParameters;

		// 추가될 VRCSDK 데이터
		protected static List<VRCExpressionsMenu.Control> VRSuyaMenus;
		protected static VRCExpressionParameters.Parameter[] VRSuyaParameters;

		/// <summary>상속 클래스가 존재하는지 확인 한 후 해당 제품의 최종 세팅 요청을 합니다.</summary>
		internal static void RequestSetup() {
			ClearVariable();
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
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_ModelWalking))) {
				if (Array.Exists(RequestSetupVRSuyaProductList, Product => Product.ProductName == ProductName.ModelWalking)) ProductSetup_ModelWalking.RequestSetting();
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
		internal static void RequestProductRegister() {
			InstalledVRSuyaProducts = new VRSuyaProduct[0];
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_AFK))) ProductSetup_AFK.RegisterProduct();
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Mogumogu))) ProductSetup_Mogumogu.RegisterProduct();
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Wotagei))) ProductSetup_Wotagei.RegisterProduct();
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Feet))) ProductSetup_Feet.RegisterProduct();
            if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Nyoronyoro))) ProductSetup_Nyoronyoro.RegisterProduct();
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_ModelWalking))) ProductSetup_ModelWalking.RegisterProduct();
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Handmotion))) ProductSetup_Handmotion.RegisterProduct();
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

		/// <summary>정적 변수를 초기화 합니다.</summary>
		private static void ClearVariable() {
			VRSuyaLocomotionLayers = new AnimatorControllerLayer[0];
			VRSuyaGestureLayers = new AnimatorControllerLayer[0];
			VRSuyaActionLayers = new AnimatorControllerLayer[0];
			VRSuyaFXLayers = new AnimatorControllerLayer[0];

			VRSuyaLocomotionParameters = new AnimatorControllerParameter[0];
			VRSuyaGestureParameters = new AnimatorControllerParameter[0];
			VRSuyaActionParameters = new AnimatorControllerParameter[0];
			VRSuyaFXParameters = new AnimatorControllerParameter[0];

			VRSuyaMenus = new List<VRCExpressionsMenu.Control>();
			VRSuyaParameters = new VRCExpressionParameters.Parameter[0];
			return;
		}

		/// <summary>설치된 VRSuya 제품에서 지원하는 아바타 목록을 추려 사용 가능한 아바타 목록을 만듭니다.</summary>
		private static void UpdateInstalledAvatarList() {
			Avatar[] AllInstalledAvatars = InstalledVRSuyaProducts.SelectMany(Product => Product.SupportAvatarList).ToArray();
			InstalledVRSuyaProductAvatars = AllInstalledAvatars.Distinct().ToArray();
			return;
		}

		/// <summary>아바타의 각 VRC 애니메이터에 세팅해야 하는 값을 보냅니다.</summary>
		private static void UpdateUnityAnimator() {
			AnimatorController VRCLocomotionLayer = null;
			AnimatorController VRCGestureLayer = null;
			AnimatorController VRCActionLayer = null;
			AnimatorController VRCFXLayer = null;
			if (Array.Find(AvatarVRCAvatarLayers, VRCAnimator => VRCAnimator.type == VRCAvatarDescriptor.AnimLayerType.Base).animatorController != null) {
				VRCLocomotionLayer = (AnimatorController)Array.Find(AvatarVRCAvatarLayers, VRCAnimator => VRCAnimator.type == VRCAvatarDescriptor.AnimLayerType.Base).animatorController;
			}
			if (Array.Find(AvatarVRCAvatarLayers, VRCAnimator => VRCAnimator.type == VRCAvatarDescriptor.AnimLayerType.Gesture).animatorController != null) {
				VRCGestureLayer = (AnimatorController)Array.Find(AvatarVRCAvatarLayers, VRCAnimator => VRCAnimator.type == VRCAvatarDescriptor.AnimLayerType.Gesture).animatorController;
			}
			if (Array.Find(AvatarVRCAvatarLayers, VRCAnimator => VRCAnimator.type == VRCAvatarDescriptor.AnimLayerType.Action).animatorController != null) {
				VRCActionLayer = (AnimatorController)Array.Find(AvatarVRCAvatarLayers, VRCAnimator => VRCAnimator.type == VRCAvatarDescriptor.AnimLayerType.Action).animatorController;
			}
			if (Array.Find(AvatarVRCAvatarLayers, VRCAnimator => VRCAnimator.type == VRCAvatarDescriptor.AnimLayerType.FX).animatorController != null) {
				VRCFXLayer = (AnimatorController)Array.Find(AvatarVRCAvatarLayers, VRCAnimator => VRCAnimator.type == VRCAvatarDescriptor.AnimLayerType.FX).animatorController;
			}
			if (VRCLocomotionLayer) UpdateTargetAnimatorController(VRCLocomotionLayer, VRSuyaLocomotionLayers, VRSuyaLocomotionParameters);
			if (VRCGestureLayer) UpdateTargetAnimatorController(VRCGestureLayer, VRSuyaGestureLayers, VRSuyaGestureParameters);
			if (VRCActionLayer) UpdateTargetAnimatorController(VRCActionLayer, VRSuyaActionLayers, VRSuyaActionParameters);
			if (VRCFXLayer) UpdateTargetAnimatorController(VRCFXLayer, VRSuyaFXLayers, VRSuyaFXParameters);
			return;
		}

		/// <summary>세팅해야 하는 애니메이터 컨트롤러에 파라메터와 레이어가 존재하는지 확인 후 추가합니다.</summary>
		private static void UpdateTargetAnimatorController(AnimatorController TargetController, AnimatorControllerLayer[] TargetLayers, AnimatorControllerParameter[] TargetParameters) {
			foreach (AnimatorControllerParameter NewParameter in TargetParameters) {
				if (!Array.Exists(TargetController.parameters, ExistParameter => NewParameter.name == ExistParameter.name)) {
					Undo.RecordObject(TargetController, "Added Unity Animator Controller Parameter");
					TargetController.parameters = TargetController.parameters.Concat(new AnimatorControllerParameter[] { NewParameter }).ToArray();
					EditorUtility.SetDirty(TargetController);
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
			}
			if (KeepLinkAnimatorLayer) {
				foreach (AnimatorControllerLayer NewLayer in TargetLayers) {
					if (!Array.Exists(TargetController.layers, ExistLayer => NewLayer.name == ExistLayer.name)) {
						Undo.RecordObject(TargetController, "Added Unity Animator Controller Layer");
						TargetController.layers = TargetController.layers.Concat(new AnimatorControllerLayer[] { NewLayer }).ToArray();
						EditorUtility.SetDirty(TargetController);
						Undo.CollapseUndoOperations(UndoGroupIndex);
					}
				}
			} else {
				AnimatorControllerLayer[] RequiredLayers = TargetLayers.Where(TargetLayer => !Array.Exists(TargetController.layers, ExistLayer => TargetLayer.name == ExistLayer.name)).ToArray();
				if (RequiredLayers.Length > 0) {
					AnimatorControllerLayer[] newAnimatorLayers = new AnimatorControllerLayer[TargetController.layers.Length + RequiredLayers.Length];
					Array.Copy(TargetController.layers, newAnimatorLayers, TargetController.layers.Length);
					for (int Index = 0; Index < RequiredLayers.Length; Index++) {
						AnimatorControllerLayer newAnimationLayer = new AnimatorControllerLayer();
						newAnimationLayer.avatarMask = RequiredLayers[Index].avatarMask;
						newAnimationLayer.blendingMode = RequiredLayers[Index].blendingMode;
						newAnimationLayer.defaultWeight = RequiredLayers[Index].defaultWeight;
						newAnimationLayer.iKPass = RequiredLayers[Index].iKPass;
						newAnimationLayer.name = RequiredLayers[Index].name;
						newAnimationLayer.syncedLayerAffectsTiming = RequiredLayers[Index].syncedLayerAffectsTiming;

						AnimatorStateMachine oldStateMachines = RequiredLayers[Index].stateMachine;
						AnimatorStateMachine newStateMachines = new AnimatorStateMachine();
						newStateMachines.anyStatePosition = oldStateMachines.anyStatePosition;
						newStateMachines.entryPosition = oldStateMachines.entryPosition;
						newStateMachines.exitPosition = oldStateMachines.exitPosition;
						newStateMachines.parentStateMachinePosition = oldStateMachines.parentStateMachinePosition;

						// State 복제
						for (int StateIndex = 0; StateIndex < oldStateMachines.states.Length; StateIndex++) {
							AnimatorState oldState = oldStateMachines.states[StateIndex].state;
							AnimatorState newState = newStateMachines.AddState(oldState.name);
							newState.behaviours = oldState.behaviours;
							newState.cycleOffset = oldState.cycleOffset;
							newState.cycleOffsetParameter = oldState.cycleOffsetParameter;
							newState.cycleOffsetParameterActive = oldState.cycleOffsetParameterActive;
							newState.iKOnFeet = oldState.iKOnFeet;
							newState.mirror = oldState.mirror;
							newState.mirrorParameter = oldState.mirrorParameter;
							newState.mirrorParameterActive = oldState.mirrorParameterActive;
							newState.motion = oldState.motion;
							newState.speed = oldState.speed;
							newState.speedParameter = oldState.speedParameter;
							newState.speedParameterActive = oldState.speedParameterActive;
							newState.tag = oldState.tag;
							newState.timeParameter = oldState.timeParameter;
							newState.timeParameterActive = oldState.timeParameterActive;
							newState.writeDefaultValues = oldState.writeDefaultValues;
						}

						// StateTransition 복제
						for (int StateIndex = 0; StateIndex < oldStateMachines.states.Length; StateIndex++) {
							AnimatorStateTransition[] oldStateTransitions = oldStateMachines.states[StateIndex].state.transitions;
							AnimatorStateTransition[] newStateTransitions = new AnimatorStateTransition[oldStateTransitions.Length];
							for (int TransitionIndex = 0; TransitionIndex < oldStateTransitions.Length; TransitionIndex++) {
								AnimatorState newTargetState = Array.Find(newStateMachines.states, ExistState => ExistState.state == oldStateTransitions[TransitionIndex].destinationState).state;
								AnimatorStateTransition newTransition = newStateMachines.states[StateIndex].state.AddTransition(newTargetState);
								newTransition.canTransitionToSelf = oldStateTransitions[TransitionIndex].canTransitionToSelf;
								newTransition.duration = oldStateTransitions[TransitionIndex].duration;
								newTransition.exitTime = oldStateTransitions[TransitionIndex].exitTime;
								newTransition.hasExitTime = oldStateTransitions[TransitionIndex].hasExitTime;
								newTransition.hasFixedDuration = oldStateTransitions[TransitionIndex].hasFixedDuration;
								newTransition.interruptionSource = oldStateTransitions[TransitionIndex].interruptionSource;
								newTransition.offset = oldStateTransitions[TransitionIndex].offset;
								newTransition.orderedInterruption = oldStateTransitions[TransitionIndex].orderedInterruption;
								newTransition.isExit = oldStateTransitions[TransitionIndex].isExit;
								newTransition.mute = oldStateTransitions[TransitionIndex].mute;
								newTransition.solo = oldStateTransitions[TransitionIndex].solo;
								newTransition.hideFlags = oldStateTransitions[TransitionIndex].hideFlags;
								newTransition.name = oldStateTransitions[TransitionIndex].name;
								for (int ConditionIndex = 0; ConditionIndex < oldStateTransitions[TransitionIndex].conditions.Length; ConditionIndex++) {
									newTransition.AddCondition(oldStateTransitions[TransitionIndex].conditions[ConditionIndex].mode, oldStateTransitions[TransitionIndex].conditions[ConditionIndex].threshold, oldStateTransitions[TransitionIndex].conditions[ConditionIndex].parameter);
								}
							}
						}

						newAnimationLayer.stateMachine = newStateMachines;
						newAnimatorLayers[TargetController.layers.Length + Index] = newAnimationLayer;
					}
					Undo.RecordObject(TargetController, "Added Unity Animator Controller Layer");
					TargetController.layers = newAnimatorLayers;
					EditorUtility.SetDirty(TargetController);
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
			}
			return;
		}

		/// <summary>세팅해야 하는 파라메터 큐를 아바타 파라메터에 존재하는지 확인 후 추가합니다.</summary>
		private static void UpdateAvatarParameters() {
            foreach (VRCExpressionParameters.Parameter NewParameter in VRSuyaParameters) {
                if (!Array.Exists(AvatarVRCParameter.parameters, ExistParameter => ExistParameter.name == NewParameter.name)) {
					Undo.RecordObject(AvatarVRCParameter, "Added VRC Parameter");
					AvatarVRCParameter.parameters = AvatarVRCParameter.parameters.Concat(new VRCExpressionParameters.Parameter[] { NewParameter }).ToArray();
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
            }
			EditorUtility.SetDirty(AvatarVRCParameter);
			return;
        }

		/// <summary>세팅해야 하는 메뉴 큐를 아바타 메뉴에 존재하는지 확인 후 추가합니다.</summary>
		private static void UpdateAvatarMenus() {
            foreach (VRCExpressionsMenu.Control NewMenu in VRSuyaMenus) {
                if (!AvatarVRCMenu.controls.Exists(ExistMenu => ExistMenu.subMenu == NewMenu.subMenu)) {
					Undo.RecordObject(AvatarVRCMenu, "Added VRC Menu");
					AvatarVRCMenu.controls.Add(NewMenu);
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
			}
			EditorUtility.SetDirty(AvatarVRCMenu);
			return;
		}

		/// <summary>요청한 GameObject의 Transform을 Origin에 맞춰주는 메소드</summary>
		internal static void TransformPrefab(GameObject TargetGameObject, GameObject BaseGameObject, bool KeepScale) {
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