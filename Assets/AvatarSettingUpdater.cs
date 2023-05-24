﻿#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor.Animations;

using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

/*
 * VRSuya Avatar Setting Updater
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.AvatarSettingUpdater {

    [ExecuteInEditMode]
	[AddComponentMenu("VRSuya Avatar Setting Updater")]
	public class AvatarSettingUpdater : MonoBehaviour {

        // 아바타 에디터용 변수
        public GameObject AvatarGameObjectEditor = null;
		public string AvatarTypeNameEditor = null;
		public Transform AvatarAnchorOverrideEditor = null;
		public bool ChangeTwosidedShadowEditor = false;
		public bool ChangeAnchorOverrideEditor = true;

		public Avatar[] InstalledVRSuyaProductAvatarsEditor = new Avatar[0];

		public bool InstalledProductAFKEditor = false;
		public bool InstalledProductMogumoguEditor = false;
		public bool InstalledProductWotageiEditor = false;
		public bool InstalledProductFeetEditor = false;

		public bool InstallProductAFKEditor = false;
		public bool InstallProductMogumoguEditor = false;
		public bool InstallProductWotageiEditor = false;
		public bool InstallProductFeetEditor = false;

		// 아바타 관련 변수
		protected GameObject AvatarGameObject = null;
		protected Avatar AvatarType = Avatar.NULL;
		protected Animator AvatarAnimator = null;
		protected SkinnedMeshRenderer[] AvatarSkinnedMeshRenderers = new SkinnedMeshRenderer[0];
		protected MeshRenderer[] AvatarMeshRenderers = new MeshRenderer[0];
		protected Transform AvatarAnchorOverride = null;

		protected VRCAvatarDescriptor AvatarVRCAvatarDescriptor = null;
        protected VRCAvatarDescriptor.CustomAnimLayer[] AvatarVRCAvatarLayers = new VRCAvatarDescriptor.CustomAnimLayer[0];
		protected VRCExpressionsMenu AvatarVRCMenu = null;
		protected VRCExpressionParameters AvatarVRCParameter = null;

		protected GameObject[] VRSuyaGameObjects = new GameObject[0];

		// 아바타 세팅 선택 옵션
		protected bool ChangeTwosidedShadow = false;
		protected bool ChangeAnchorOverride = true;

        // 제품 구조체
		public struct VRSuyaProduct {
			public ProductName ProductName;
            public Dictionary<VRCAvatarDescriptor.AnimLayerType, AnimatorControllerLayer[]> RequiredAnimatorLayers;
            public Dictionary<VRCAvatarDescriptor.AnimLayerType, AnimatorControllerParameter[]> RequiredAnimatorParameters;
            public List<VRCExpressionsMenu.Control> RequiredVRCMenus;
			public int RequiredVRCMemoryCount;
            public VRCExpressionParameters.Parameter[] RequiredVRCParameters;
            public Avatar[] SupportAvatarList;
            public string[] AnimationControllerGUID;
			public string[] MenuGUID;
			public string[] ParameterGUID;
			public string[] PrefabGUID;
		}

		// 제품 추가시 추가해야 될 변수
		public enum ProductName {
            AFK,
            Mogumogu,
            Wotagei,
            Feet
        }

		public enum Avatar {
			Aldina, Angura, Anon, Anri, Ash, Cygnet, Emmelie, EYO, Firina, Fuzzy, Glaze, Grus, Hakka, IMERIS,
			Karin, Kikyo, Kokoa, Koyuki, Kuronatu, Leefa, Leeme, Lunalitt, Maki, Mamehinata, Mariel, Marron,
			Maya, Merino, Milk, Minahoshi, Minase, Mint, Mir, Mishe, Moe, Nayu, Platinum, Quiche, Rainy,
			Ramune_Old, RINDO, Rue, Rusk, SELESTIA, Sephira, Sue, Suzuhana, Tien, Ukon, Usasaki, Wolferia,
			Yoll, Yuuko, NULL
		}

		public enum VRCAssetType {
			Template, Export, Locomotion, Gesture, Action, FX, Menu, Parameter
		}

		// 제품 설치 상태
		protected bool InstalledProductAFK = false;
		protected bool InstalledProductMogumogu = false;
		protected bool InstalledProductWotagei = false;
		protected bool InstalledProductFeet = false;

		protected bool InstallProductAFK = false;
		protected bool InstallProductMogumogu = false;
		protected bool InstallProductWotagei = false;
		protected bool InstallProductFeet = false;

		// 추가될 VRSuya 데이터
		protected VRSuyaProduct[] InstalledVRSuyaProducts = new VRSuyaProduct[0];
		protected Avatar[] InstalledVRSuyaProductAvatars = new Avatar[0];
		protected VRSuyaProduct[] RequestSetupVRSuyaProductList = new VRSuyaProduct[0];

		// 상태 반환
		public string StatusCode = "";

        // 컴포넌트 최초 로드시 동작
        void OnEnable() {
			if (!AvatarGameObjectEditor) AvatarGameObjectEditor = this.gameObject;
			UpdateUnityEditorStatus();
		}

		/// <summary>에디터 변수 -> 정적 변수 동기화합니다.</summary>
		private void SetStaticVariable() {
            AvatarGameObject = AvatarGameObjectEditor;
			if (AvatarTypeNameEditor != null) AvatarType = (Avatar)Enum.Parse(typeof(Avatar), AvatarTypeNameEditor);
			AvatarAnchorOverride = AvatarAnchorOverrideEditor;
			ChangeTwosidedShadow = ChangeTwosidedShadowEditor;
			ChangeAnchorOverride = ChangeAnchorOverrideEditor;
			InstallProductAFK = InstallProductAFKEditor;
			InstallProductMogumogu = InstallProductMogumoguEditor;
			InstallProductWotagei = InstallProductWotageiEditor;
			InstallProductFeet = InstallProductFeetEditor;
			return;
        }

		/// <summary>정적 변수 -> 에디터 변수 동기화합니다.</summary>
		private void SetEditorVariable() {
			AvatarGameObjectEditor = AvatarGameObject;
			AvatarAnchorOverrideEditor = AvatarAnchorOverride;
			InstalledProductAFKEditor = InstalledProductAFK;
			InstalledProductMogumoguEditor = InstalledProductMogumogu;
			InstalledProductWotageiEditor = InstalledProductWotagei;
			InstalledProductFeetEditor = InstalledProductFeet;
			InstalledVRSuyaProductAvatarsEditor = InstalledVRSuyaProductAvatars;
			return;
		}

		/// <summary>현재 Unity 프로젝트와 아바타 변수를 초기화 한 후 다시 검사합니다.</summary>
		public void UpdateUnityEditorStatus() {
            SetStaticVariable();
			ClearVariable();
            if (VerifyVariable()) {
				ProductSetup ProductSetupProcess = new ProductSetup();
				UnitySetup UnitySetupProcess = new UnitySetup();
				ProductSetupProcess.RequestProductRegister();
				UnitySetupProcess.UpdateAvatarStatus();
				ProductSetupProcess.GetVRSuyaGameObjects();
            }
			SetEditorVariable();
			return;
        }

		/// <summary>
		/// 본 프로그램의 메인 세팅 로직입니다.
		/// </summary>
		public void UpdateAvatarSetting() {
			SetStaticVariable();
			if (VerifyVariable()) {
				ProductSetup ProductSetupProcess = new ProductSetup();
				ProductSetupProcess.RequestProductRegister();
				AddRequestSetupVRSuyaProduct();
				if (VerifyVRCSDK()) {
					UnitySetup UnitySetupProcess = new UnitySetup();
					ProductSetupProcess.GetVRSuyaGameObjects();
					ProductSetupProcess.RequestSetup();
					UnitySetupProcess.GetAvatarSkinnedMeshRenderers();
					UnitySetupProcess.GetAvatarMeshRenderers();
					UnitySetupProcess.UpdateAvatarData();

					Debug.Log("[VRSuya AvatarSettingUpdater] Update Completed");
					DestroyImmediate(this);
				}
            }
            return;
        }

		/// <summary>정적 변수를 초기화 합니다.</summary>
		private void ClearVariable() {
            AvatarAnimator = null;
            AvatarSkinnedMeshRenderers = new SkinnedMeshRenderer[0];
            AvatarMeshRenderers = new MeshRenderer[0];
            VRSuyaGameObjects = new GameObject[0];
			InstalledProductAFK = false;
			InstalledProductMogumogu = false;
			InstalledProductWotagei = false;
			InstalledProductFeet = false;
			InstalledVRSuyaProducts = new VRSuyaProduct[0];
			InstalledVRSuyaProductAvatars = new Avatar[0];
			StatusCode = "";
            return;
        }

		/// <summary>아바타의 현재 상태를 검사하여 설치가 가능한지 확인합니다.</summary>
		/// <returns>설치 가능 여부</returns>
		private bool VerifyVariable() {
            if (!AvatarGameObject) {
				StatusCode = "NO_AVATAR";
				return false;
			}
            AvatarGameObject.TryGetComponent(typeof(Animator), out Component Animator);
            if (!Animator) {
                StatusCode = "NO_ANIMATOR";
				return false;
			} else {
				AvatarAnimator = AvatarGameObject.GetComponent<Animator>();
			}
			AvatarGameObject.TryGetComponent(typeof(VRCAvatarDescriptor), out Component AvatarDescriptor);
            if (!AvatarDescriptor) {
				StatusCode = "NO_VRCAVATARDESCRIPTOR";
				return false;
			} else {
				AvatarVRCAvatarDescriptor = AvatarGameObject.GetComponent<VRCAvatarDescriptor>();
				AvatarVRCAvatarLayers = AvatarVRCAvatarDescriptor.baseAnimationLayers;
			}
			return true;
        }

		/// <summary>설치된 제품과 세팅 요청한 제품을 검사하여, 세팅 요청 목록에 넣습니다.</summary>
		private void AddRequestSetupVRSuyaProduct() {
			RequestSetupVRSuyaProductList = new VRSuyaProduct[0];
			if (InstalledProductAFK && InstallProductAFK) {
				VRSuyaProduct RequestProduct = Array.Find(InstalledVRSuyaProducts, Product => Product.ProductName == ProductName.AFK);
				RequestSetupVRSuyaProductList = RequestSetupVRSuyaProductList.Concat(new VRSuyaProduct[] { RequestProduct }).ToArray();
			}
			if (InstalledProductMogumogu && InstallProductMogumogu) {
				VRSuyaProduct RequestProduct = Array.Find(InstalledVRSuyaProducts, Product => Product.ProductName == ProductName.Mogumogu);
				RequestSetupVRSuyaProductList = RequestSetupVRSuyaProductList.Concat(new VRSuyaProduct[] { RequestProduct }).ToArray();
			}
			if (InstalledProductWotagei && InstallProductWotagei) {
				VRSuyaProduct RequestProduct = Array.Find(InstalledVRSuyaProducts, Product => Product.ProductName == ProductName.Wotagei);
				RequestSetupVRSuyaProductList = RequestSetupVRSuyaProductList.Concat(new VRSuyaProduct[] { RequestProduct }).ToArray();
			}
			if (InstalledProductFeet && InstallProductFeet) {
				VRSuyaProduct RequestProduct = Array.Find(InstalledVRSuyaProducts, Product => Product.ProductName == ProductName.Feet);
				RequestSetupVRSuyaProductList = RequestSetupVRSuyaProductList.Concat(new VRSuyaProduct[] { RequestProduct }).ToArray();
			}
			return;
		}

		/// <summary>세팅해야 할 메뉴와 파라메터를 검사하여 세팅 가능한지 확인합니다.</summary>
		/// <returns>설치 가능 여부</returns>
		private bool VerifyVRCSDK() {
			AvatarVRCMenu = AvatarVRCAvatarDescriptor.expressionsMenu;
			AvatarVRCParameter = AvatarVRCAvatarDescriptor.expressionParameters;
			if (!AvatarVRCMenu) {
				StatusCode = "NO_VRCSDK_MENU";
				return false;
			}
			if (!AvatarVRCParameter) {
				StatusCode = "NO_VRCSDK_PARAMETER";
				return false;
			}
			int CurrentAvatarVRCMenuCount = AvatarVRCAvatarDescriptor.expressionsMenu.controls.Count;
			int CurrentAvatarVRCParameterCosts = AvatarVRCParameter.CalcTotalCost();
			int RequiredMenuCount = 0;
			int RequiredParametersCost = 0;
			if (RequestSetupVRSuyaProductList.Length > 0) {
				foreach (VRSuyaProduct RequestProduct in RequestSetupVRSuyaProductList) {
					foreach (VRCExpressionsMenu.Control RequestMenu in RequestProduct.RequiredVRCMenus) {
						if (!AvatarVRCAvatarDescriptor.expressionsMenu.controls.Exists(ExistMenu => ExistMenu.subMenu == RequestMenu.subMenu)) RequiredMenuCount++;
					}
					foreach (VRCExpressionParameters.Parameter RequestParameter in RequestProduct.RequiredVRCParameters) {
						if (!Array.Exists(AvatarVRCParameter.parameters, ExistParameter => ExistParameter.name == RequestParameter.name)) {
							switch (RequestParameter.valueType) {
								case VRCExpressionParameters.ValueType.Bool:
									RequiredParametersCost = RequiredParametersCost + 1;
									break;
								case VRCExpressionParameters.ValueType.Int:
								case VRCExpressionParameters.ValueType.Float:
									RequiredParametersCost = RequiredParametersCost + 8;
									break;
							}
						}
					}
				}
			}
			if (CurrentAvatarVRCMenuCount + RequiredMenuCount > VRCExpressionsMenu.MAX_CONTROLS) {
				StatusCode = "NO_MORE_MENU";
				return false;
			}
			if (CurrentAvatarVRCParameterCosts + RequiredParametersCost > VRCExpressionParameters.MAX_PARAMETER_COST) {
				StatusCode = "NO_MORE_PARAMETER";
				return false;
            }
			return true;
		}

		/// <summary>요청한 제품에서 아바타를 검색하여 세팅이 가능한지 확인합니다.</summary>
		/// <returns>아바타 파일 존재 여부</returns>
		public bool ReturnAvatarInstalled(ProductName Product, string AvatarName) {
			bool ReturnResult = false;
			if (!string.IsNullOrEmpty(AvatarName)) {
				Avatar RequestAvatarType;
				if (Enum.TryParse<Avatar>(AvatarName, out RequestAvatarType)) {
					if (Array.Exists(InstalledVRSuyaProducts, InstalledProduct => InstalledProduct.ProductName == Product)) {
						VRSuyaProduct TargetProduct = Array.Find(InstalledVRSuyaProducts, InstalledProduct => InstalledProduct.ProductName == Product);
						ReturnResult = Array.Exists(TargetProduct.SupportAvatarList, SupportAvatar => SupportAvatar == RequestAvatarType);
					}
				}
			}
			return ReturnResult;
		}

		/// <summary>디버그용 메소드</summary>
		public void DebugAvatarSetting() {
			SetStaticVariable();
			return;
		}
	}
}
#endif