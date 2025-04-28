using TMPro;
using UnityEngine;

namespace SNL_3PLUS_MGP {
    public class TextMeshShaderLoader : MonoBehaviour {
        string myFontAssetName;
        //22 22
        //22 22/2//8/4/5/6
        //22 

        private void Start(){
            TMP_FontAsset font;
            TMP_Text mytext1;
            mytext1 = GetComponent<TMP_Text>();
            myFontAssetName = mytext1.font.name;

            font = MGPSDK.AllResourceContainer.instance.GetTMPfontAssets(myFontAssetName);

            if (font == null){
                Debug.Log("SNL || TextMeshShaderLoader || TMP Font Not Found ==> " + myFontAssetName);
            }

            mytext1.font = font;
            mytext1.font.material.shader=Shader.Find(mytext1.font.material.shader.name);
        }
    }
}