using Network.Game;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIRegister : MonoBehaviour
    {
        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private TMP_InputField accountNameInput;
        [SerializeField] private TMP_InputField firstNameInput;
        [SerializeField] private TMP_InputField surNameInput;
        [SerializeField] private UIBirthdayForm birthdayInput;

        [SerializeField] private Button loginButton;
        [SerializeField] private Button registerButton;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            loginButton.onClick.AddListener(UIManager.OpenLoginUI);
            registerButton.onClick.AddListener(Register);
        }

        private PlayerAccount ReadPlayerAccount()
        {
            return new()
            {
                email = emailInput.text,
                password = passwordInput.text,
                accountName = accountNameInput.text,
                firstName = firstNameInput.text,
                surName = surNameInput.text,
                birthDay = birthdayInput.Date
            };
        }

        public void Register()
        {
            GameClient.GameClient.Register(ReadPlayerAccount());
            //GameClient.GameClient.Register(emailInput.text, passwordInput.text, accountNameInput.text, firstNameInput.text, surNameInput.text,
            //    birthdayInput.Year, birthdayInput.Month, birthdayInput.Day);
        }
    }
}
