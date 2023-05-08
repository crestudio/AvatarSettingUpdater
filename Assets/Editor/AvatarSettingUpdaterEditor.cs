using UnityEngine;
using UnityEditor;

/*
 * VRSuya Avatar Setting Updater Editor
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.AvatarSettingUpdater {

    [CustomEditor(typeof(AvatarSettingUpdater))]
    public class AvatarSettingUpdaterEditor : Editor {

        SerializedProperty SerializedAvatarGameObject;
        SerializedProperty SerializedChangeTwosidedShadow;
        SerializedProperty SerializedChangeAnchorOverride;
        SerializedProperty SerializedAvatarAnchorOverride;
		SerializedProperty SerializedStatusCode;
		SerializedProperty SerializedInstalledVRSuyaProductAvatarsEditor;

		// 제품 추가시 추가해야 될 변수
		SerializedProperty SerializedInstalledProductAFK;
		SerializedProperty SerializedInstalledProductMogumogu;
		SerializedProperty SerializedInstalledProductWotagei;
		SerializedProperty SerializedInstalledProductFeet;

		SerializedProperty SerializedInstallProductAFK;
		SerializedProperty SerializedInstallProductMogumogu;
		SerializedProperty SerializedInstallProductWotagei;
		SerializedProperty SerializedInstallProductFeet;

		public static int LanguageIndex = 0;
        public readonly string[] LanguageType = new[] { "English", "한국어", "日本語" };
		public static int AvatarType = 0;
		public static string[] AvatarNames = new string[0];
		public static string SelectedAvatarName = "";

		void OnEnable() {
            SerializedAvatarGameObject = serializedObject.FindProperty("AvatarGameObjectEditor");
            SerializedChangeTwosidedShadow = serializedObject.FindProperty("ChangeTwosidedShadowEditor");
            SerializedChangeAnchorOverride = serializedObject.FindProperty("ChangeAnchorOverrideEditor");
            SerializedAvatarAnchorOverride = serializedObject.FindProperty("AvatarAnchorOverrideEditor");
			SerializedStatusCode = serializedObject.FindProperty("StatusCode");
			SerializedInstalledVRSuyaProductAvatarsEditor = serializedObject.FindProperty("InstalledVRSuyaProductAvatarsEditor");

			// 제품 추가시 추가해야 될 변수
			SerializedInstalledProductAFK = serializedObject.FindProperty("InstalledProductAFKEditor");
			SerializedInstalledProductMogumogu = serializedObject.FindProperty("InstalledProductMogumoguEditor");
			SerializedInstalledProductWotagei = serializedObject.FindProperty("InstalledProductWotageiEditor");
			SerializedInstalledProductFeet = serializedObject.FindProperty("InstalledProductFeetEditor");

			SerializedInstallProductAFK = serializedObject.FindProperty("InstallProductAFKEditor");
			SerializedInstallProductMogumogu = serializedObject.FindProperty("InstallProductMogumoguEditor");
			SerializedInstallProductWotagei = serializedObject.FindProperty("InstallProductWotageiEditor");
			SerializedInstallProductFeet = serializedObject.FindProperty("InstallProductFeetEditor");
		}

        public override void OnInspectorGUI() {
            serializedObject.Update();
			AvatarNames = LanguageHelper.ReturnAvatarName(SerializedInstalledVRSuyaProductAvatarsEditor);
			LanguageIndex = EditorGUILayout.Popup(LanguageHelper.GetContextString("String_Language"), LanguageIndex, LanguageType);
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.PropertyField(SerializedAvatarGameObject, new GUIContent(LanguageHelper.GetContextString("String_TargetAvatar")));
			AvatarType = EditorGUILayout.Popup(LanguageHelper.GetContextString("String_Avatar"), AvatarType, AvatarNames);
			SelectedAvatarName = SerializedInstalledVRSuyaProductAvatarsEditor.enumNames[AvatarType];
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
			EditorGUILayout.PropertyField(SerializedChangeTwosidedShadow, new GUIContent(LanguageHelper.GetContextString("String_TwoSidedShadow")));
			EditorGUILayout.PropertyField(SerializedChangeAnchorOverride, new GUIContent(LanguageHelper.GetContextString("String_ChangeAnchorOverride")));
			EditorGUILayout.PropertyField(SerializedAvatarAnchorOverride, new GUIContent(LanguageHelper.GetContextString("String_ObjectAnchorOverride")));
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
			EditorGUILayout.LabelField(LanguageHelper.GetContextString("String_SetupProduct"), EditorStyles.boldLabel);

			// 제품 추가시 추가해야 될 변수
			GUI.enabled = ReturnInstalled(AvatarSettingUpdater.ProductName.AFK, SerializedInstalledProductAFK);
			EditorGUILayout.PropertyField(SerializedInstallProductAFK, new GUIContent(LanguageHelper.GetContextString("String_ProductAFK")));
			GUI.enabled = ReturnInstalled(AvatarSettingUpdater.ProductName.Mogumogu, SerializedInstalledProductMogumogu);
			EditorGUILayout.PropertyField(SerializedInstallProductMogumogu, new GUIContent(LanguageHelper.GetContextString("String_ProductMogumogu")));
			GUI.enabled = ReturnInstalled(AvatarSettingUpdater.ProductName.Wotagei, SerializedInstalledProductWotagei);
			EditorGUILayout.PropertyField(SerializedInstallProductWotagei, new GUIContent(LanguageHelper.GetContextString("String_ProductWotagei")));
			GUI.enabled = ReturnInstalled(AvatarSettingUpdater.ProductName.Feet, SerializedInstalledProductFeet);
			EditorGUILayout.PropertyField(SerializedInstallProductFeet, new GUIContent(LanguageHelper.GetContextString("String_ProductFeet")));
			
			GUI.enabled = true;
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
			if (!string.IsNullOrEmpty(SerializedStatusCode.stringValue)) {
                EditorGUILayout.HelpBox(LanguageHelper.GetContextString(SerializedStatusCode.stringValue), MessageType.Info);
            }
			serializedObject.ApplyModifiedProperties();
			if (GUILayout.Button(LanguageHelper.GetContextString("String_GetAvatarData"))) {
                (target as AvatarSettingUpdater).UpdateUnityEditorStatus();
            }
            if (GUILayout.Button(LanguageHelper.GetContextString("String_UpdateAvatarData") + " (" + SelectedAvatarName + ")")) {
                (target as AvatarSettingUpdater).UpdateAvatarSetting();
            }
			if (GUILayout.Button(LanguageHelper.GetContextString("String_Debug"))) {
				(target as AvatarSettingUpdater).DebugAvatarSetting();
			}
		}

		/// <summary>요청한 VRSuya 제품의 아바타 파일이 설치 되어있는지 검사합니다.</summary>
		/// <returns>에셋 설치 여부</returns>
		private static bool ReturnInstalled(AvatarSettingUpdater.ProductName RequestProductName, SerializedProperty ProductProperty) {
			bool ReturnResult = false;
			if (ProductProperty.boolValue && AvatarSettingUpdater.ReturnAvatarInstalled(RequestProductName, SelectedAvatarName)) {
				ReturnResult = true;
			}
			return ReturnResult;
		}
	}
}
