using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SNL_3PLUS_MGP
{
    public class SNL_3PLUS_MGP_LoadingController : MonoBehaviour
    {
        public Text statusText;
        public GameObject dashboard;
        internal void OpenLoadingScreen(string status)
        {
            gameObject.SetActive(true);
            statusText.text = status;
        }

        internal void CloseLoadingScreen()
        {
            gameObject.SetActive(false);
            dashboard.SetActive(false);
        }
    }
}