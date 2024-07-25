#if UNITY_EDITOR
using System.Linq;

using UnityEngine;

/*
 * VRSuya Avatar Setting Updater
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.avatarsettingupdater {

	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class ProductSetup_SoundPad : ProductSetup {

		private static VRSuyaProduct SoundPad;

		/// <summary>제품 정보를 AssetManager에게 요청하여 업데이트 한 후, 설치된 에셋 목록에 추가합니다.</summary>
		internal static void RegisterProduct() {
			InstalledProductSoundPad = false;
			SoundPad = new VRSuyaProduct();
			SoundPad = AssetManager.UpdateProductInformation(ProductName.SoundPad);
			InstalledVRSuyaProducts = InstalledVRSuyaProducts.Concat(new VRSuyaProduct[] { SoundPad }).ToArray();
			if (SoundPad.SupportAvatarList.Length > 0) InstalledProductSoundPad = true;
			return;
		}
	}
}
#endif