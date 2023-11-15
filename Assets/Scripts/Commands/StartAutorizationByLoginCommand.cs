using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Services.Localization;
using PFS.Assets.Scripts.Views.Login;
using System;
using TMPro;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Authorization
{
    public class StartAutorizationByLoginCommand : BaseCommand
    {
        private enum MessageTypes
        {
            LoginEmpty,
            PasswordEmpty,
            PasswordIncorrect,
            PasswordWrong,
            UnknownError
        }

        private UILoginScreenView uiLoginScreenView;

        public override void Execute()
        {
            Retain();
            if (EventData.data != null)
            {
                uiLoginScreenView = EventData.data as UILoginScreenView;
                if (uiLoginScreenView != null)
                {
                    StartLoginProcess();
                }
                else
                {
                    Release();
                }
            }
            else
            {
                Debug.Log("StarPasswordLoginCommand: EventData.data == null");
                Release();
            }



        }

        private void StartLoginProcess()
        {
            if (CheckLogin() && CheckPassword())
            {

                Action<string> failAction = (string error) =>
                {
                    SetMessageText(GetMessageTypeFromErrorString(error));
                    Release();
                };

                if (uiLoginScreenView.IsTeacherLogin)
                {
                    Dispatcher.Dispatch(EventGlobal.E_GetTeacherDataByPassword, new GetTeacherByPasswordRequestModel(uiLoginScreenView.loginInput.text,
                                                                                                                uiLoginScreenView.passwordInput.text,
                   () =>
                   {
                       Release();
                       SwitchModeModel.Mode = Conditions.GameModes.SchoolModeForTeacherLogin;
                       uiLoginScreenView.ProcessLoginAuthorizationFinish();
                   },
                   failAction));
                }
                else
                {
                    Dispatcher.Dispatch(EventGlobal.E_GetChildDataByPassword, new GetChildByPasswordRequestModel(uiLoginScreenView.loginInput.text,
                                                                                                                 PasswordsHelper.PasswordEncode(uiLoginScreenView.passwordInput.text),
                    () =>
                    {
                        Release();
                        SwitchModeModel.Mode = Conditions.GameModes.SchoolModeForChildLogin;
                        uiLoginScreenView.ProcessLoginAuthorizationFinish();
                    },
                    failAction));
                }
            }
            else
            {
                Release();
            }
        }

        private bool CheckLogin()
        {
            if (uiLoginScreenView.loginInput.text == "")
            {
                SetMessageText(MessageTypes.LoginEmpty);
                return false;
            }

            return true;
        }

        private bool CheckPassword()
        {
            if (uiLoginScreenView.passwordInput.text == "")
            {
                SetMessageText(MessageTypes.PasswordEmpty);
                return false;
            }

            return true;
        }

        private void SetMessageText(MessageTypes type)
        {
            string res = "";
            bool isPasswordMessage = true; ;

            switch (type)
            {
                case MessageTypes.LoginEmpty:
                    isPasswordMessage = false;
                    res = LocalizationManager.GetLocalizationText(LocalizationKeys.LoginErrorEmptyLogin);
                    break;
                case MessageTypes.PasswordEmpty:
                    res = LocalizationManager.GetLocalizationText(LocalizationKeys.LoginErrorEmptyPassword);
                    break;
                case MessageTypes.PasswordIncorrect:
                    res = LocalizationManager.GetLocalizationText(LocalizationKeys.LoginErrorIncorrectPassword);
                    break;
                case MessageTypes.PasswordWrong:
                    res = LocalizationManager.GetLocalizationText(LocalizationKeys.LoginErrorWrongPassword);
                    break;
                case MessageTypes.UnknownError:
                    res = LocalizationManager.GetLocalizationText(LocalizationKeys.LoginErrorUnknown);
                    break;
                default:
                    break;
            }

            (isPasswordMessage ? uiLoginScreenView.passwordInput : uiLoginScreenView.loginInput).SetTextWithoutNotify(string.Empty);

            TextMeshProUGUI messageText = (isPasswordMessage ? uiLoginScreenView.passwordInput.placeholder : uiLoginScreenView.loginInput.placeholder) as TextMeshProUGUI;

            if (messageText)
            {
                messageText.color = uiLoginScreenView.loginErrorOutline.color;
                messageText.text = res;
            }

            uiLoginScreenView.helpPasswordButton.gameObject.SetActive(isPasswordMessage);
            uiLoginScreenView.wrongPasswordLabel.gameObject.SetActive(isPasswordMessage);
            uiLoginScreenView.loginErrorOutline.gameObject.SetActive(!isPasswordMessage);
            uiLoginScreenView.passwordErrorOutline.gameObject.SetActive(isPasswordMessage);
        }

        private MessageTypes GetMessageTypeFromErrorString(string error)
        {
            MessageTypes result = MessageTypes.UnknownError;

            switch (error)
            {
                case "INCORRECT_PASSWORD":
                    result = MessageTypes.PasswordWrong;
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}