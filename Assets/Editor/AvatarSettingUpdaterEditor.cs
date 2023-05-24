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

		public int LanguageIndex = 0;
        public readonly string[] LanguageType = new[] { "English", "한국어", "日本語" };
		public int AvatarType = 0;
		public string[] AvatarNames = new string[0];
		public string SelectedAvatarName = "";

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
			LanguageHelper LanguageHelperProcess = new LanguageHelper();
			AvatarNames = LanguageHelperProcess.ReturnAvatarName(SerializedInstalledVRSuyaProductAvatarsEditor);
			LanguageIndex = EditorGUILayout.Popup(LanguageHelperProcess.GetContextString("String_Language"), LanguageIndex, LanguageType);
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.PropertyField(SerializedAvatarGameObject, new GUIContent(LanguageHelperProcess.GetContextString("String_TargetAvatar")));
			AvatarType = EditorGUILayout.Popup(LanguageHelperProcess.GetContextString("String_Avatar"), AvatarType, AvatarNames);
			SelectedAvatarName = SerializedInstalledVRSuyaProductAvatarsEditor.enumNames[AvatarType];
			(target as AvatarSettingUpdater).AvatarTypeNameEditor = SelectedAvatarName;
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
			EditorGUILayout.PropertyField(SerializedChangeTwosidedShadow, new GUIContent(LanguageHelperProcess.GetContextString("String_TwoSidedShadow")));
			EditorGUILayout.PropertyField(SerializedChangeAnchorOverride, new GUIContent(LanguageHelperProcess.GetContextString("String_ChangeAnchorOverride")));
			EditorGUILayout.PropertyField(SerializedAvatarAnchorOverride, new GUIContent(LanguageHelperProcess.GetContextString("String_ObjectAnchorOverride")));
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
			EditorGUILayout.LabelField(LanguageHelperProcess.GetContextString("String_SetupProduct"), EditorStyles.boldLabel);

			// 제품 추가시 추가해야 될 변수
			GUI.enabled = ReturnInstalled(AvatarSettingUpdater.ProductName.AFK, SerializedInstalledProductAFK);
			EditorGUILayout.PropertyField(SerializedInstallProductAFK, new GUIContent(LanguageHelperProcess.GetContextString("String_ProductAFK")));
			GUI.enabled = ReturnInstalled(AvatarSettingUpdater.ProductName.Mogumogu, SerializedInstalledProductMogumogu);
			EditorGUILayout.PropertyField(SerializedInstallProductMogumogu, new GUIContent(LanguageHelperProcess.GetContextString("String_ProductMogumogu")));
			GUI.enabled = ReturnInstalled(AvatarSettingUpdater.ProductName.Wotagei, SerializedInstalledProductWotagei);
			EditorGUILayout.PropertyField(SerializedInstallProductWotagei, new GUIContent(LanguageHelperProcess.GetContextString("String_ProductWotagei")));
			GUI.enabled = ReturnInstalled(AvatarSettingUpdater.ProductName.Feet, SerializedInstalledProductFeet);
			EditorGUILayout.PropertyField(SerializedInstallProductFeet, new GUIContent(LanguageHelperProcess.GetContextString("String_ProductFeet")));
			
			GUI.enabled = true;
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
			if (!string.IsNullOrEmpty(SerializedStatusCode.stringValue)) {
                EditorGUILayout.HelpBox(LanguageHelperProcess.GetContextString(SerializedStatusCode.stringValue), MessageType.Info);
            }
			serializedObject.ApplyModifiedProperties();
			if (GUILayout.Button(LanguageHelperProcess.GetContextString("String_GetAvatarData"))) {
                (target as AvatarSettingUpdater).UpdateUnityEditorStatus();
            }
            if (GUILayout.Button(LanguageHelperProcess.GetContextString("String_UpdateAvatarData") + " (" + SelectedAvatarName + ")")) {
                (target as AvatarSettingUpdater).UpdateAvatarSetting();
            }
			if (GUILayout.Button(LanguageHelperProcess.GetContextString("String_Debug"))) {
				(target as AvatarSettingUpdater).DebugAvatarSetting();
			}
		}

		/// <summary>요청한 VRSuya 제품의 아바타 파일이 설치 되어있는지 검사합니다.</summary>
		/// <returns>에셋 설치 여부</returns>
		private bool ReturnInstalled(AvatarSettingUpdater.ProductName RequestProductName, SerializedProperty ProductProperty) {
			AvatarSettingUpdater AvatarSettingUpdaterProcess = new AvatarSettingUpdater();
			bool ReturnResult = false;
			if (ProductProperty.boolValue && AvatarSettingUpdaterProcess.ReturnAvatarInstalled(RequestProductName, SelectedAvatarName)) {
				ReturnResult = true;
			}
			return ReturnResult;
		}
	}
}
