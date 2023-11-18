#if UNITY_EDITOR
using System;

using UnityEditor.Animations;
using UnityEngine;

/*
 * VRSuya Avatar Setting Updater
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.avatarsettingupdater {

	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class AnimationControllerDuplicator : AvatarSettingUpdater {

		/// <summary>요청한 애니메이터 컨트롤러에 파라메터를 추가합니다.</summary>
		internal static void AddParameter(AnimatorController TargetController, AnimatorControllerParameter TargetParameter) {
			AnimatorControllerParameter newParameter = new AnimatorControllerParameter {
				defaultBool = TargetParameter.defaultBool,
				defaultFloat = TargetParameter.defaultFloat,
				defaultInt = TargetParameter.defaultInt,
				name = TargetParameter.name,
				type = TargetParameter.type
			};
			TargetController.AddParameter(newParameter);
			return;
		}

		/// <summary>요청한 애니메이터 컨트롤러에 레이어를 복제하여 추가합니다.</summary>
		/// <returns>데이터가 추가된 새로운 애니메이터 컨트롤러 레이어</returns>
		internal static AnimatorControllerLayer[] DuplicateAnimatorLayers(AnimatorController TargetController, AnimatorControllerLayer[] TargetLayers) {
			AnimatorControllerLayer[] newAnimatorLayers = new AnimatorControllerLayer[TargetController.layers.Length + TargetLayers.Length];
			Array.Copy(TargetController.layers, newAnimatorLayers, TargetController.layers.Length);
			for (int Index = 0; Index < TargetLayers.Length; Index++) {
				AnimatorControllerLayer newAnimatorLayer = DuplicateAnimatorLayer(TargetLayers[Index]);
				newAnimatorLayers[TargetController.layers.Length + Index] = newAnimatorLayer;
			}
			return newAnimatorLayers;
		}

		/// <summary>요청한 애니메이터 레이어를 복제하여 반환합니다.</summary>
		/// <returns>복제된 애니메이터 레이어</returns>
		private static AnimatorControllerLayer DuplicateAnimatorLayer(AnimatorControllerLayer TargetAnimatorLayer) {
			AnimatorControllerLayer newAnimatorLayer = new AnimatorControllerLayer {
				avatarMask = TargetAnimatorLayer.avatarMask,
				blendingMode = TargetAnimatorLayer.blendingMode,
				defaultWeight = TargetAnimatorLayer.defaultWeight,
				iKPass = TargetAnimatorLayer.iKPass,
				name = TargetAnimatorLayer.name,
				stateMachine = DuplicateStateMachine(TargetAnimatorLayer.stateMachine),
				syncedLayerAffectsTiming = TargetAnimatorLayer.syncedLayerAffectsTiming,
				syncedLayerIndex = TargetAnimatorLayer.syncedLayerIndex
			};
			return newAnimatorLayer;
		}

		/// <summary>요청한 StateMachine을 복제하여 반환합니다.</summary>
		/// <returns>복제된 StateMachine</returns>
		private static AnimatorStateMachine DuplicateStateMachine(AnimatorStateMachine TargetStateMachine) {
			AnimatorStateMachine newStateMachines = new AnimatorStateMachine {
				anyStatePosition = TargetStateMachine.anyStatePosition,
				behaviours = TargetStateMachine.behaviours,
				entryPosition = TargetStateMachine.entryPosition,
				exitPosition = TargetStateMachine.exitPosition,
				parentStateMachinePosition = TargetStateMachine.parentStateMachinePosition,
				stateMachines = DuplicateChildStateMachine(TargetStateMachine.stateMachines),
				states = DuplicateChildAnimatorState(TargetStateMachine.states),
				hideFlags = TargetStateMachine.hideFlags,
				name = TargetStateMachine.name
			};
			return newStateMachines;
		}

		/// <summary>요청한 하위 StateMachine을 복제하여 반환합니다.</summary>
		/// <returns>복제된 하위 StateMachine</returns>
		private static ChildAnimatorStateMachine[] DuplicateChildStateMachine(ChildAnimatorStateMachine[] TargetChildStateMachines) {
			ChildAnimatorStateMachine[] newChildStateMachines = new ChildAnimatorStateMachine[TargetChildStateMachines.Length];
			for (int Index = 0; Index < TargetChildStateMachines.Length; Index++) {
				ChildAnimatorStateMachine newChildStateMachine = new ChildAnimatorStateMachine {
					position = TargetChildStateMachines[Index].position,
					stateMachine = DuplicateStateMachine(TargetChildStateMachines[Index].stateMachine)
				};
				newChildStateMachines[Index] = newChildStateMachine;
			}
			return newChildStateMachines;
		}

		/// <summary>요청한 ChildAnimatorState을 복제하여 반환합니다.</summary>
		/// <returns>복제된 ChildAnimatorState</returns>
		private static ChildAnimatorState[] DuplicateChildAnimatorState(ChildAnimatorState[] TargetChildAnimatorStates) {
			ChildAnimatorState[] newChildAnimatorStates = new ChildAnimatorState[TargetChildAnimatorStates.Length];
			for (int Index = 0; Index < TargetChildAnimatorStates.Length; Index++) {
				ChildAnimatorState newChildAnimatorState = new ChildAnimatorState {
					position = TargetChildAnimatorStates[Index].position,
					state = DuplicateAnimatorState(TargetChildAnimatorStates[Index].state)
				};
				newChildAnimatorStates[Index] = newChildAnimatorState;
			}
			return newChildAnimatorStates;
		}

		/// <summary>요청한 ChildAnimatorState을 복제하여 반환합니다.</summary>
		/// <returns>복제된 ChildAnimatorState</returns>
		private static AnimatorState DuplicateAnimatorState(AnimatorState TargetAnimatorState) {
			AnimatorState newState = new AnimatorState {
				behaviours = TargetAnimatorState.behaviours,
				cycleOffset = TargetAnimatorState.cycleOffset,
				cycleOffsetParameter = TargetAnimatorState.cycleOffsetParameter,
				cycleOffsetParameterActive = TargetAnimatorState.cycleOffsetParameterActive,
				iKOnFeet = TargetAnimatorState.iKOnFeet,
				mirror = TargetAnimatorState.mirror,
				mirrorParameter = TargetAnimatorState.mirrorParameter,
				mirrorParameterActive = TargetAnimatorState.mirrorParameterActive,
				motion = TargetAnimatorState.motion,
				speed = TargetAnimatorState.speed,
				speedParameter = TargetAnimatorState.speedParameter,
				speedParameterActive = TargetAnimatorState.speedParameterActive,
				tag = TargetAnimatorState.tag,
				timeParameter = TargetAnimatorState.timeParameter,
				timeParameterActive = TargetAnimatorState.timeParameterActive,
				transitions = new AnimatorStateTransition[TargetAnimatorState.transitions.Length],
				writeDefaultValues = TargetAnimatorState.writeDefaultValues,
				hideFlags = TargetAnimatorState.hideFlags,
				name = TargetAnimatorState.name
			};
			return newState;
		}

		/// <summary>요청한 Transition을 복제하여 반환합니다.</summary>
		/// <returns>복제된 Transition</returns>
		private static AnimatorStateTransition[] DuplicateTransitions(AnimatorStateTransition[] TargetStateTransitions) {
			AnimatorStateTransition[] newStateTransitions = new AnimatorStateTransition[TargetStateTransitions.Length];
			for (int Index = 0; Index < TargetStateTransitions.Length; Index++) {
				AnimatorStateTransition newTransition = new AnimatorStateTransition {
					canTransitionToSelf = TargetStateTransitions[Index].canTransitionToSelf,
					duration = TargetStateTransitions[Index].duration,
					exitTime = TargetStateTransitions[Index].exitTime,
					hasExitTime = TargetStateTransitions[Index].hasExitTime,
					hasFixedDuration = TargetStateTransitions[Index].hasFixedDuration,
					interruptionSource = TargetStateTransitions[Index].interruptionSource,
					offset = TargetStateTransitions[Index].offset,
					orderedInterruption = TargetStateTransitions[Index].orderedInterruption,
					conditions = DuplicateConditions(TargetStateTransitions[Index].conditions),
					destinationState = TargetStateTransitions[Index].destinationState,
					destinationStateMachine = TargetStateTransitions[Index].destinationStateMachine,
					isExit = TargetStateTransitions[Index].isExit,
					mute = TargetStateTransitions[Index].mute,
					solo = TargetStateTransitions[Index].solo,
					hideFlags = TargetStateTransitions[Index].hideFlags,
					name = TargetStateTransitions[Index].name
				};
				newStateTransitions[Index] = newTransition;
			}
			return newStateTransitions;
		}

		/// <summary>요청한 조건을 복제하여 반환합니다.</summary>
		/// <returns>복제된 조건</returns>
		private static AnimatorCondition[] DuplicateConditions(AnimatorCondition[] TargetConditions) {
			AnimatorCondition[] newConditions = new AnimatorCondition[TargetConditions.Length];
			for (int Index = 0; Index < TargetConditions.Length; Index++) {
				AnimatorCondition newCondition = new AnimatorCondition {
					mode = TargetConditions[Index].mode,
					threshold = TargetConditions[Index].threshold,
					parameter = TargetConditions[Index].parameter
				};
				newConditions[Index] = newCondition;
			}
			return newConditions;
		}
	}
}
#endif