using Network.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Transform panel; 

        [SerializeField] private GameObject UI_Login;
        [SerializeField] private GameObject UI_Register;

        private static UIManager instance;
        private GameObject activeUI;

        private void Awake()
        {
            instance = this;
            OpenLoginUI();
            GameClient.GameClient.ConnectToServer("127.0.0.1", 22220);
        }

        private static void OpenUI(GameObject uiObject)
        {
            if (instance.activeUI != null)
                Destroy(instance.activeUI);

            instance.activeUI = Instantiate(uiObject, instance.panel);
        }

        public static void OpenLoginUI()
        {
            OpenUI(instance.UI_Login);
        }

        public static void OpenRegisterUI()
        {
            OpenUI(instance.UI_Register);
        }
    }
}
