using PFS.Assets.Scripts.Models.Pool;
using PFS.Assets.Scripts.Views.Avatar;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PFS.Assets.Scripts.Views.Pool
{
    public class PoolObjectsView : BaseView
    {
        [Inject] public PoolModel Pool { get; set; }

        [Header("Sprites")]
        public Sprite bookCoverDefault;
        public Sprite bookBackgroundDefault;
        public Sprite quizPartDefault;

        [Header("Book")]
        public GameObject bookItem;
        public GameObject booksGridType1;
        public GameObject booksGridType2;

        [Header("Book elements")]
        public GameObject bookOutline;
        public GameObject bookAnimatedIcon;
        public GameObject bookSongIcon;

        [Header("Loader")]
        public GameObject loader;
        public GameObject booksGridLoader;

        [Header("Customization")]
        public UIAvatarItemView[] avatars;
        public GameObject[] customization;

        [Header("Body color"), Tooltip("For customization")]
        public Color[] bodyColor;

        [Header("Chars color"), Tooltip("For flashcards")]
        public Color[] charColors;

        [Header("Simplified colors"), Tooltip("For books")]
        public Color[] simplifiedColors = new Color[8];

        [Header("Languages images"), SerializeField]
        public PoolLanguage[] languagesImages;

        public void LoadView()
        {
            SetPool();
        }

        public void RemoveView()
        {

        }

        private void SetPool()
        {
            //-------images-------------
            Pool.BookCoverDefault = bookCoverDefault;
            Pool.BookBackgroundDefault = bookBackgroundDefault;
            Pool.QuizPartDefault = quizPartDefault;

            //-------book elements-------------
            Pool.BookOutline = bookOutline;
            Pool.BookAnimatedIcon = bookAnimatedIcon;
            Pool.BookSongIcon = bookSongIcon;

            //-------customization------
            Pool.Avatars = avatars;

            //------simplified-------------
            Pool.SimplifiedColors = simplifiedColors;

            //------languages-------------
            Pool.LanguagesImages = new Dictionary<Conditions.Languages, Sprite>();
            foreach (var item in languagesImages)
            {
                Pool.LanguagesImages.Add(item.language, item.sprite);
            }
        }

        [Serializable]
        public struct PoolLanguage
        {
            public Conditions.Languages language;
            public Sprite sprite;
        }
    }
}