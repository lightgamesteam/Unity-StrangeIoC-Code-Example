using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using static PFS.Assets.Scripts.Services.Localization.LocalizationKeys;
using Assets.Scripts.Services.Analytics;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.Pool;
using PFS.Assets.Scripts.Models.Requests.Homeworks;
using PFS.Assets.Scripts.Models.ScreenManagerModels;
using PFS.Assets.Scripts.Models.Requests;
using PFS.Assets.Scripts.Views.Library;
using PFS.Assets.Scripts.Views.Homeworks;
using PFS.Assets.Scripts.Views.Avatar;

namespace PFS.Assets.Scripts.Views.MyProfile
{
    public class UIMyProfileView : BaseView
    {
        [Inject] public ChildModel ChildModel { get; set; }
        [Inject] public ChildStatsModel ChildStatsModel { get; set; }
        [Inject] public PoolModel Pool { get; set; }
        [Inject] public Analytics Analytics { get; private set; }


        [Header("Child Info Area")]
        [SerializeField] private TextMeshProUGUI childNickname;
        [SerializeField] private Button avatarButton;


        [SerializeField] private TextMeshProUGUI id;
        [SerializeField] private GameObject idParent;

        [SerializeField] private TextMeshProUGUI starsLable;
        [SerializeField] private TextMeshProUGUI starsCount;

        [SerializeField] private TextMeshProUGUI readBooksLable;
        [SerializeField] private TextMeshProUGUI readBooksCount;

        [SerializeField] private GameObject homeworkImage;
        [SerializeField] private TextMeshProUGUI homeworkLable;
        [SerializeField] private TextMeshProUGUI homeworkCount;

        [SerializeField] private TextMeshProUGUI readingTimeLable;
        [SerializeField] private TextMeshProUGUI readingTimeCount;

        [Header("My Classes Area")]
        [SerializeField] private TextMeshProUGUI myClassesTitle;
        [SerializeField] private TextMeshProUGUI connectToClassTitle;
        [SerializeField] private Button connectToClassButton;
        [SerializeField] private Transform classItemsContainer;
        [SerializeField] private UIClassInfoItemView classItemPrefab;

        [Header("Most Clicked Words Area")]
        [SerializeField] private TextMeshProUGUI mostClickedWordsTitle;
        [SerializeField] private TextMeshProUGUI sorryNoMostClickText;
        [SerializeField] private GameObject mostClickedWordsContainer;
        [SerializeField] private List<GameObject> wordLineExample;
        [SerializeField] private List<GameObject> valueLineExample;
        [SerializeField] private List<GameObject> clickWord;
        [SerializeField] private List<GameObject> objectLine;

        //[Header("Quiz statistics panel")]
        //[SerializeField] private UIQuizStatsView quizPanel;

        [Header("Most Read Books Area")]
        [SerializeField] private TextMeshProUGUI mostReadBooksTitle;
        [SerializeField] private TextMeshProUGUI sorryNoMostReadyText;
        [SerializeField] private GameObject mostReadBooksContainer;
        [SerializeField] private GameObject bookExample;

        [Header("Last Homework Area")]
        [SerializeField] private TextMeshProUGUI lastHomeworkTitle;
        [SerializeField] private Transform lastHomeworkItemContainer;
        [SerializeField] private UIDoneHomeworkItemView homeworkItemPrefab;
        [SerializeField] private GameObject assignmentsPanel;
        [SerializeField] private Material material1;
        [SerializeField] private Material material2;

        [Header("Animation Options")]
        [SerializeField, Range(0.0f, 3.0f)] private float classItemDelay;

        private int hours;
        private int residueMinutes;
        private bool isStatsLoaded = false;
        private bool isClassesLoaded = false;
        private IEnumerator showClassesCoroutine = null;

        private const int MostReadBooksShowCount = 8;

        ChildModel currentChild;

        public void LoadView()
        {
            SetScreenColliderSize();

            Dispatcher.Dispatch(EventGlobal.E_ShowBlocker);
            GetChildInfo();
            UpdateChildClasses();

            SetLocalization();
            Dispatcher.AddListener(EventGlobal.E_Reinitlocalization, SetLocalization);
            Dispatcher.AddListener(EventGlobal.E_UserAvatarUpdated, InitAvatarItem);
            Dispatcher.AddListener(EventGlobal.E_UpdateChildClasses, UpdateChildClasses);
            Dispatcher.AddListener(EventGlobal.E_ResetChildClasses, DeleteChildClasses);
            Dispatcher.AddListener(EventGlobal.E_BlinkTextMesh, BlinkText);


            if (PlayerPrefsModel.Mode == Conditions.GameModes.SchoolModeForTeacherFeide || PlayerPrefsModel.Mode == Conditions.GameModes.SchoolModeForTeacherLogin)
            {
                connectToClassButton.gameObject.SetActive(false);
                assignmentsPanel.SetActive(false);
                homeworkLable.gameObject.SetActive(false);
                homeworkCount.gameObject.SetActive(false);
                homeworkImage.gameObject.SetActive(false);
            }
            else if (PlayerPrefsModel.Mode == Conditions.GameModes.SchoolModeForChildFeide)
            {
                connectToClassButton.gameObject.SetActive(false);
            }
            //quizPanel.InitializePanel();
        }

        public void RemoveView()
        {
            Dispatcher.RemoveListener(EventGlobal.E_Reinitlocalization, SetLocalization);
            Dispatcher.RemoveListener(EventGlobal.E_UserAvatarUpdated, InitAvatarItem);
            Dispatcher.RemoveListener(EventGlobal.E_UpdateChildClasses, UpdateChildClasses);
            Dispatcher.RemoveListener(EventGlobal.E_ResetChildClasses, DeleteChildClasses);
            Dispatcher.RemoveListener(EventGlobal.E_BlinkTextMesh, BlinkText);

            if (showClassesCoroutine != null)
            {
                StopCoroutine(showClassesCoroutine);
            }
        }

        private void GetChildInfo()
        {
            isStatsLoaded = false;

            Dispatcher.Dispatch(EventGlobal.E_GetStatsChild, new BasicRequestModel(() =>
            {
                isStatsLoaded = true;

                readBooksCount.text = ChildStatsModel.Stats.ReadBooks.ToString();
                homeworkCount.text = ChildStatsModel.Stats.HomeworksCount.ToString();

            //Convert reading time
            double getMinutes = TimeSpan.FromSeconds(ChildStatsModel.Stats.ReadingTime).TotalMinutes;

                hours = (int)Math.Truncate(Math.Round(getMinutes, 0) / 60);
                residueMinutes = (int)Math.Round(getMinutes, 0) % 60;

            // Load most read books
            if (ChildStatsModel.Stats.MostRead != null && ChildStatsModel.Stats.MostRead.Length != 0)
                {
                    mostReadBooksTitle.gameObject.SetActive(true);
                    sorryNoMostReadyText.gameObject.SetActive(false);
                    GameObject bookObject;
                    for (int i = 0; i < ChildStatsModel.Stats.MostRead.Length && i < MostReadBooksShowCount; i++)
                    {
                        ChildStatsModel.Stats.MostRead[i].IsBookFromMostRead = true;
                        bookObject = Instantiate<GameObject>(bookExample, mostReadBooksContainer.transform);
                        bookObject.GetComponent<UIBookView>().SetBook(ChildStatsModel.Stats.MostRead[i], UIBookView.BookState.SetOutline);
                    }
                }
                else
                {
                    mostReadBooksTitle.gameObject.SetActive(false);
                    sorryNoMostReadyText.gameObject.SetActive(true);
                }

                SetLocalization();
                SetReadingTime();
                SetMostClickedWords();
                SetLastHomeworks();

                if (isClassesLoaded)
                {
                    Dispatcher.Dispatch(EventGlobal.E_HideBlocker);
                }

                Analytics.LogEvent(EventName.NavigationMyProfileDashboard,
                            new System.Collections.Generic.Dictionary<Property, object>()
                            {
                            { Property.Uuid, PlayerPrefsModel.CurrentChildId},
                            { Property.BookReadCount, ChildStatsModel.Stats.ReadBooks.ToString()},
                            { Property.WeeklyTimeSpent_s, ChildStatsModel.Stats.ReadingTime.ToString()},
                            { Property.CompletedAssignmentCount, ChildStatsModel.Stats.HomeworksCount.ToString()}

                            });
            },
            () =>
            {
                if (isClassesLoaded)
                {
                    Dispatcher.Dispatch(EventGlobal.E_HideBlocker);
                }
            }));

            currentChild = ChildModel.GetChild(PlayerPrefsModel.CurrentChildId);
            childNickname.text = currentChild.Nickname + " " + currentChild.Surname;
            if (String.IsNullOrEmpty(currentChild.Id))
            {
                idParent.gameObject.SetActive(false);
            }
            else
            {
                idParent.gameObject.SetActive(true);
                id.text = currentChild.AccountId;

            }
            starsCount.text = currentChild.Stars + "";

            connectToClassButton.onClick.AddListener(() =>
            {
                Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel() { screenName = UIScreens.UIConnectToClassPopup, isAddToScreensList = false, showSwitchAnim = false });
            });

            avatarButton.onClick.AddListener(() =>
                {
                    Dispatcher.Dispatch(EventGlobal.E_ShowScreen, new ShowScreenModel() { screenName = UIScreens.UIChooseAvatarPopup, isAddToScreensList = false, showSwitchAnim = false });
                    Analytics.LogEvent(EventName.NavigationSelectAvatar,
                      new System.Collections.Generic.Dictionary<Property, object>()
                      {
                        { Property.Uuid, PlayerPrefsModel.CurrentChildId}
                      });
                });

            InitAvatarItem();
        }

        private void UpdateChildClasses()
        {
            StartCoroutine(UpdateChildClassesCoroutine());
        }

        public void OnCopyButtonClick()
        {
            GUIUtility.systemCopyBuffer = id.text;
            Dispatcher.Dispatch(EventGlobal.E_BlinkTextMesh);
        }

        public void BlinkText()
        {
            StartCoroutine(ReturnPreviousColor(id));
        }

        private IEnumerator ReturnPreviousColor(TextMeshProUGUI textMesh)
        {
            if (textMesh == null)
            {
                Debug.LogError("SetColorForText error. TextMesh is null.");
                yield break;
            }
            var previousColor = id.color;
            id.color = Color.blue;

            yield return new WaitForSeconds(0.5f);

            textMesh.color = previousColor;
        }

        private IEnumerator UpdateChildClassesCoroutine()
        {
            yield return new WaitForEndOfFrame();

            isClassesLoaded = false;

            Dispatcher.Dispatch(EventGlobal.E_ShowBlocker);

            Dispatcher.Dispatch(EventGlobal.E_GetChildClasses, new GetChildClassesRequestModel((ClassModel[] classes) =>
            {
                isClassesLoaded = true;

                showClassesCoroutine = InitClassesScroll(classes);

                StartCoroutine(showClassesCoroutine);

                if (isStatsLoaded)
                {
                    Dispatcher.Dispatch(EventGlobal.E_HideBlocker);
                }
            },
            () =>
            {
                isClassesLoaded = true;

                if (isStatsLoaded)
                {
                    Dispatcher.Dispatch(EventGlobal.E_HideBlocker);
                }
            }));
        }

        private void DeleteChildClasses()
        {
            if (showClassesCoroutine != null)
            {
                StopCoroutine(showClassesCoroutine);
            }

            foreach (Transform child in classItemsContainer)
            {
                if (child.name.Contains("Clone"))
                {
                    Destroy(child.gameObject);
                }
            }
        }

        private IEnumerator InitClassesScroll(ClassModel[] classes)
        {
            foreach (var classInfo in classes)
            {
                UIClassInfoItemView classItem = Instantiate(classItemPrefab, classItemsContainer);
                classItem.SetClassInfo(classInfo);

                yield return new WaitForSeconds(classItemDelay);
            }
        }

        private void InitAvatarItem()
        {
            foreach (Transform child in avatarButton.transform)
            {
                Destroy(child.gameObject);
            }

            string avatarId = ChildModel.GetChild(PlayerPrefsModel.CurrentChildId).AvatarId;

            UIAvatarItemView avatarPrefab = null;

            foreach (var avatar in Pool.Avatars)
            {
                if (avatar.Id == avatarId)
                {
                    avatarPrefab = avatar;
                }
            }

            if (avatarPrefab == null)
            {
                avatarPrefab = Pool.Avatars[0];
            }

            UIAvatarItemView avatarInstance = Instantiate(avatarPrefab, avatarButton.transform);
            avatarInstance.Init(UIAvatarItemView.AvatarItemState.Profile);
            (avatarInstance.transform as RectTransform).sizeDelta = Vector2.zero;
        }

        private void SetLocalization()
        {
            starsLable.text = LocalizationManager.GetLocalizationText(StarsKey) + ":";
            readBooksLable.text = LocalizationManager.GetLocalizationText(BooksReadKey) + ":";
            homeworkLable.text = char.ToUpper(LocalizationManager.GetLocalizationText(CompletedHomeworkKey)[0]) + LocalizationManager.GetLocalizationText(CompletedHomeworkKey).Substring(1) + ":";
            readingTimeLable.text = LocalizationManager.GetLocalizationText(ReadingTimeKey) + ":";
            lastHomeworkTitle.text = LocalizationManager.GetLocalizationText(FinishedHomeworksTitleKey);
            mostClickedWordsTitle.text = LocalizationManager.GetLocalizationText(FavoriteWordsKey);
            mostReadBooksTitle.text = LocalizationManager.GetLocalizationText(MyFavoriteBooksKey);
            myClassesTitle.text = LocalizationManager.GetLocalizationText(MyClassesKey);
            connectToClassTitle.text = LocalizationManager.GetLocalizationText(ConnectToClassKey);

            if (isStatsLoaded)
            {
                sorryNoMostClickText.text = LocalizationManager.GetLocalizationText(NoMostClickKey);
                sorryNoMostReadyText.text = LocalizationManager.GetLocalizationText(NoMostReadyKey);

                for (int i = 0; ChildStatsModel.Stats.MostClicked.Length > i; i++)
                {
                    clickWord[i].GetComponent<TextMeshProUGUI>().text = LocalizationManager.GetLocalizationText(ClicksKey);
                }
            }
        }

        private void SetMostClickedWords()
        {
            if (ChildStatsModel.Stats.MostClicked != null && ChildStatsModel.Stats.MostClicked.Length != 0)
            {
                sorryNoMostClickText.gameObject.SetActive(false);
                mostClickedWordsTitle.gameObject.SetActive(true);

                for (int i = 0; ChildStatsModel.Stats.MostClicked.Length > i; i++)
                {
                    if (ChildStatsModel.Stats.MostClicked[i].Word.Length > 0)
                    {
                        string tempWord = char.ToUpper(ChildStatsModel.Stats.MostClicked[i].Word[0]) + ChildStatsModel.Stats.MostClicked[i].Word.Substring(1);
                        objectLine[i].SetActive(true);
                        wordLineExample[i].GetComponent<TextMeshProUGUI>().text = i + 1 + ". " + tempWord.Replace("_", " ");
                        valueLineExample[i].GetComponent<TextMeshProUGUI>().text = ChildStatsModel.Stats.MostClicked[i].Count.ToString();
                    }
                }
            }
            else
            {
                mostClickedWordsContainer.gameObject.SetActive(false);
                mostClickedWordsTitle.gameObject.SetActive(false);
                sorryNoMostClickText.gameObject.SetActive(true);
            }
        }

        private void SetLastHomeworks()
        {
            Dispatcher.Dispatch(EventGlobal.E_GetHomeworks, new HomeworksByStatusRequstModel(Conditions.HomeworkStatus.Checked, 3,
            (works) =>
            {
                StartCoroutine(InitHomeworksScroll(works));
            }, () =>
            {
                Debug.LogError("GetHomeworks server error");
            }));
        }

        private void SetReadingTime()
        {
            string wordHour = LocalizationManager.GetLocalizationText(HourKey);
            string wordMinutes = LocalizationManager.GetLocalizationText(MinuteKey);

            if (hours < 1)
            {
                readingTimeCount.text = residueMinutes + " " + wordMinutes.Substring(0, 3);
            }
            else
            {
                if (residueMinutes > 1)
                {
                    readingTimeCount.text = hours + " " + wordHour.Substring(0, 1) + " " + residueMinutes + " " + wordMinutes.Substring(0, 3);
                }
                else
                {
                    readingTimeCount.text = hours + " " + wordHour.Substring(0, 1);
                }
            }
        }

        private IEnumerator InitHomeworksScroll(Homework[] newHomeworks)
        {
            yield return new WaitForSeconds(1);

            lastHomeworkTitle.gameObject.SetActive(newHomeworks.Length > 0);

            bool chooseMaterial = true;

            for (int i = 0; i < newHomeworks.Length; i++)
            {
                if (i < 3)
                {
                    UIDoneHomeworkItemView item = Instantiate(homeworkItemPrefab, lastHomeworkItemContainer);
                    if (item)
                    {
                        item.SetHomework(newHomeworks[i], chooseMaterial ? material1 : material2);
                        chooseMaterial = !chooseMaterial;
                    }

                    yield return new WaitForSeconds(1);
                }

            }
        }

        private void SetScreenColliderSize()
        {
            BoxCollider2D collider = GetComponent<BoxCollider2D>();

            if (collider)
            {
                collider.size = GetComponent<RectTransform>()?.sizeDelta ?? Vector2.zero;
            }
        }
    }
}