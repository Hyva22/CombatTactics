using Network.Game;
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
            loginButton.onClick.AddListener(Login);
            registerButton.onClick.AddListener(UIManager.OpenRegisterUI);
        }

        public void Login()
        {
            PlayerAccount playerAccount = new()
            {
                email = emailInput.text,
                password = passwordInput.text,
            };

            GameClient.GameClient.SendLogin(playerAccount);
        }
    }
}