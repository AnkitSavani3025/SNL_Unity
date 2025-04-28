using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SNL_3PLUS_MGP
{
    public class SNL_3PLUS_MGP_ProfileDownLoader : MonoBehaviour
    {
        Image myProfileImage;

        private void Awake()
        {
            myProfileImage = GetComponent<Image>();
        }
        internal void GetDisplayImage(string dpUrl)
        {
            if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
            {
                Debug.Log("Fetching User dp at url " + dpUrl);
            }

            var mysprite = SNL_3PLUS_MGP_GameManager.instance.profilePics.Find(x => x.name == dpUrl);
            if (mysprite)
            {
                myProfileImage.sprite = mysprite;
                return;
            }
            else
            {
                StartCoroutine(GetDpAsyncInGame(dpUrl));
            }
        }

        private IEnumerator GetDpAsyncInGame(string avatar)
        {
            var displayPicture = SNL_3PLUS_MGP_GameManager.instance.defaultProfilePic;
            if (string.IsNullOrEmpty(avatar))
            {
                if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
                {
                    Debug.Log("***!!!***");
                    Debug.Log("Returning default dp");
                }

                myProfileImage.sprite = displayPicture;
            }
            else
            {
                using (WWW www = new WWW(avatar))
                {
                    yield return www;
                    if (!(www.texture == null))
                    {
                        displayPicture = Sprite.Create(www.texture,
                            new Rect(0.0f, 0.0f, www.texture.width, www.texture.height),
                            new Vector2(0.0f, 0.0f));
                        if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
                        {
                            Debug.Log("Returning dpwnloaded dp = " + avatar + " = " + www.texture +
                                      " = " + displayPicture);
                        }

                        displayPicture.name = avatar;
                        SNL_3PLUS_MGP_GameManager.instance.profilePics.Add(displayPicture);
                        myProfileImage.sprite=displayPicture;
                    }
                }
            }
        }

    }
}