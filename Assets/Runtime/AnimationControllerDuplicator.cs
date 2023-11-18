#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
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
				AnimatorControllerLayer newAnimationLayer = new AnimatorControllerLayer();
				newAnimationLayer.avatarMask = TargetLayers[Index].avatarMask;
				newAnimationLayer.blendingMode = TargetLayers[Index].blendingMode;
				newAnimationLayer.defaultWeight = TargetLayers[Index].defaultWeight;
				newAnimationLayer.iKPass = TargetLayers[Index].iKPass;
				newAnimationLayer.name = TargetLayers[Index].name;
				newAnimationLayer.syncedLayerAffectsTiming = TargetLayers[Index].syncedLayerAffectsTiming;
				newAnimationLayer.syncedLayerIndex = TargetLayers[Index].syncedLayerIndex;

				AnimatorStateMachine oldStateMachines = TargetLayers[Index].stateMachine;
				AnimatorStateMachine newStateMachines = new AnimatorStateMachine();
				newStateMachines.anyStatePosition = oldStateMachines.anyStatePosition;
				newStateMachines.behaviours = oldStateMachines.behaviours;
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
			return newAnimatorLayers;
		}
	}
}
#endif