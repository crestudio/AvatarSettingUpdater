﻿#if UNITY_EDITOR
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

		private readonly string[] dictHeadGameObjectName = { "Body", "Head", "Face" };

		/// <summary>외부에서 요청한 아바타 정보를 업데이트를 처리하는 메소드 입니다.</summary>
		internal void UpdateAvatarStatus() {
			GetAvatarSkinnedMeshRenderers();
			GetAvatarMeshRenderers();
			GetAvatarAnchorOverride();
			return;
		}

		/// <summary>외부의 세팅 요청을 처리하는 메인 메소드 입니다.</summary>
		internal void UpdateAvatarData() {
			if (ChangeTwosidedShadow) UpdateTwosidedShadow();
			if (ChangeAnchorOverride) UpdateAnchorOverride();
			return;
		}

		/* Unity 관련 아바타 변수 업데이트 */

		/// <summary>아바타의 SkinnedMeshRenderer 목록을 작성합니다.</summary>
		/// <returns>아바타의 모든 SkinnedMeshRenderer 배열</returns>
		internal SkinnedMeshRenderer[] GetAvatarSkinnedMeshRenderers() {
            AvatarSkinnedMeshRenderers = AvatarGameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            return AvatarSkinnedMeshRenderers;
        }

		/// <summary>아바타의 MeshRenderer 목록을 작성합니다.</summary>
		/// <returns>아바타의 모든 MeshRenderer 배열</returns>
		internal MeshRenderer[] GetAvatarMeshRenderers() {
            AvatarMeshRenderers = AvatarGameObject.GetComponentsInChildren<MeshRenderer>(true);
            return AvatarMeshRenderers;
        }

		/// <summary>아바타의 대표 AnchorOverride 포인트를 획득하는 메소드 입니다.</summary>
		/// <returns>기준이 되는 AnchorOverride 트랜스폼</returns>
		private Transform GetAvatarAnchorOverride() {
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

		/// <summary>아바타의 렌더러 세팅을 Two-sided 그림자로 설정 합니다.</summary>
		private void UpdateTwosidedShadow() {
			foreach (SkinnedMeshRenderer TargetSkinnedMeshRenderer in AvatarSkinnedMeshRenderers) {
				TargetSkinnedMeshRenderer.shadowCastingMode = ShadowCastingMode.TwoSided;
			}
			foreach (MeshRenderer TargetMeshRenderer in AvatarMeshRenderers) {
				TargetMeshRenderer.shadowCastingMode = ShadowCastingMode.TwoSided;
			}
			return;
		}

		/// <summary>아바타의 AnchorOverride 세팅을 설정 합니다.</summary>
		private void UpdateAnchorOverride() {
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