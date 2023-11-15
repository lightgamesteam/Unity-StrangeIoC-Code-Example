using PFS.Assets.Scripts.Views.Avatar;
using UnityEngine;
using Generic = System.Collections.Generic;

namespace PFS.Assets.Scripts.Models.Pool
{
    public class PoolModel
    {
        public static PoolModel Instance { get; private set; }

        //-------images-------------
        public Sprite BookCoverDefault { get; set; }
        public Sprite BookBackgroundDefault { get; set; }
        public Sprite QuizPartDefault { get; set; }

        //-------book elements-------------
        public GameObject BookOutline { get; set; }
        public GameObject BookAnimatedIcon { get; set; }
        public GameObject BookSongIcon { get; set; }

        //-------customization------
        public UIAvatarItemView[] Avatars { get; set; }

        //------simplified-------------
        public Color[] SimplifiedColors { get; set; }

        //------languages-------------
        public Generic.Dictionary<Conditions.Languages, Sprite> LanguagesImages { get; set; }

        public PoolModel()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public Sprite GetLanguageSprite(Conditions.Languages language)
        {
            if (LanguagesImages.ContainsKey(language))
            {
                return LanguagesImages[language];
            }

            return null;
        }
    }
}