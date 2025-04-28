using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MGPSDK {
    public class AllResourceContainer : MonoBehaviour {
        public static AllResourceContainer instance;
        AssetBundle resourceAssetBundle;

        private void Awake(){
            if (instance == null) instance = this;
            DontDestroyOnLoad(gameObject);
            resourceAssetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/snlresourcebundle");
        }

        public List<Material> spriteMatirialList;
        public List<TMP_FontAsset> textMeshProFontList;

        public TMP_FontAsset GetTMPfontAssets(string fontName){
            TMP_FontAsset tMP_FontAsset = null;
            tMP_FontAsset = resourceAssetBundle.LoadAsset<TMP_FontAsset>(fontName);
            //tMP_FontAsset = textMeshProFontList.Find(x => x.name == fontName);
            if (tMP_FontAsset == null){
                Debug.Log(" All Resouce Holder || GetTMPfontAssets  ||  Not Found ||  " + fontName);
            }
            else{
                Debug.Log(" All Resouce Holder || GetTMPfontAssets" + fontName);
            }

            return tMP_FontAsset;
        }

        public Material GetSpriteMatirial(string matName){
            Material mat = null;
            mat = resourceAssetBundle.LoadAsset<Material>(matName);

            //mat = spriteMatirialList.Find(x => x.name == matName);
            if (mat == null)
                Debug.Log(" All Resouce Holder || GetSpriteMatirial Mat  ||  Not Found || " + matName);
            else
                Debug.Log(" All Resouce Holder || GetSpriteMatirial Mat  ||  " + matName);

            return mat;
        }

        public Material GetParticleMatirial(string matName){
            Material mat = null;
            mat = resourceAssetBundle.LoadAsset<Material>(matName);

            //mat = spriteMatirialList.Find(x => x.name == matName);
            if (mat == null){
                Debug.Log(" All Resouce Holder || GetParticleMatirial Mat  ||  Not Found || " + matName);
            }
            else{
                Debug.Log(" All Resouce Holder || GetParticleMatirial Mat" + matName);
            }
                
            return mat;
        }
    }
}