using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UILogin : MonoBehaviour
    {
        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private TMP_InputField passwordInput;

        [SerializeField] private Button registerButton;
        [SerializeField] private Button loginButton;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            registerButton.onClick.AddListener(UIManager.OpenRegisterUI);
        }

        public void Login()
        {

        }

        public void Register()
        {

        }
    }
}