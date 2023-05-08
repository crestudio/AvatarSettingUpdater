#if UNITY_EDITOR
using System;

using UnityEngine;
using UnityEngine.Rendering;

/*
 * VRSuya Avatar Setting Updater
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.AvatarSettingUpdater {

	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class UnitySetup : AvatarSettingUpdater{

		private static readonly string[] dictHeadGameObjectName = { "Body", "Head", "Face" };

		// 메인 아바타 변수 업데이트 로직
		internal static void UpdateAvatarStatus() {
			GetAvatarSkinnedMeshRenderers();
			GetAvatarMeshRenderers();
			GetAvatarAnchorOverride();
			return;
		}

		// 메인 실제 아바타 데이터 업데이트 로직
		internal static void UpdateAvatarData() {
			if (ChangeTwosidedShadow) UpdateTwosidedShadow();
			if (ChangeAnchorOverride) UpdateAnchorOverride();
			return;
		}

		/* Unity 관련 아바타 변수 업데이트 */

		// SkinnedMeshRenderer 리스트 업데이트
		private static SkinnedMeshRenderer[] GetAvatarSkinnedMeshRenderers() {
            AvatarSkinnedMeshRenderers = AvatarGameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            return AvatarSkinnedMeshRenderers;
        }

		// MeshRenderer 리스트 업데이트
		private static MeshRenderer[] GetAvatarMeshRenderers() {
            AvatarMeshRenderers = AvatarGameObject.GetComponentsInChildren<MeshRenderer>(true);
            return AvatarMeshRenderers;
        }

		// 아바타의 AnchorOverride 획득
		private static Transform GetAvatarAnchorOverride() {
            foreach (SkinnedMeshRenderer TargetSkinnedMeshRenderer in AvatarSkinnedMeshRenderers) {
                if (Array.Exists(dictHeadGameObjectName, HeadName => TargetSkinnedMeshRenderer.gameObject.name == HeadName) == true) {
                    if (TargetSkinnedMeshRenderer.probeAnchor) {
                        AvatarAnchorOverride = TargetSkinnedMeshRenderer.probeAnchor.transform;
                        break;
                    }
                }
            }
			if (!AvatarAnchorOverride) {
				if (AvatarAnimator.GetBoneTransform(HumanBodyBones.Head)) {
					AvatarAnchorOverride = AvatarAnimator.GetBoneTransform(HumanBodyBones.Head);
				} else if (AvatarAnimator.GetBoneTransform(HumanBodyBones.Hips)) {
					AvatarAnchorOverride = AvatarAnimator.GetBoneTransform(HumanBodyBones.Hips);
				}
			}
            return AvatarAnchorOverride;
        }

		/* 실제 아바타 데이터 업데이트 */

		// Two-sided 그림자 업데이트
		private static void UpdateTwosidedShadow() {
			foreach (SkinnedMeshRenderer TargetSkinnedMeshRenderer in AvatarSkinnedMeshRenderers) {
				TargetSkinnedMeshRenderer.shadowCastingMode = ShadowCastingMode.TwoSided;
			}
			foreach (MeshRenderer TargetMeshRenderer in AvatarMeshRenderers) {
				TargetMeshRenderer.shadowCastingMode = ShadowCastingMode.TwoSided;
			}
			return;
		}

		// AnchorOverride 업데이트
		private static void UpdateAnchorOverride() {
			foreach (SkinnedMeshRenderer TargetSkinnedMeshRenderer in AvatarSkinnedMeshRenderers) {
				TargetSkinnedMeshRenderer.probeAnchor = AvatarAnchorOverride;
			}
			foreach (MeshRenderer TargetMeshRenderer in AvatarMeshRenderers) {
				TargetMeshRenderer.probeAnchor = AvatarAnchorOverride;
			}
			return;
		}
	}
}
#endif