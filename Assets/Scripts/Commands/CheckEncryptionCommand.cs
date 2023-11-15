using PFS.Assets.Scripts.Models;
using UnityEngine;

namespace PFS.Assets.Scripts.Commands.Encryption
{
    public class CheckEncryptionCommand : BaseCommand
    {
        [Inject("Straight")]
        public IPlayerPrefsStrategy straightPlayerPrefsStrategy { get; private set; }

        [Inject("Encrypted")]
        public IPlayerPrefsStrategy encryptedPlayerPrefsStrategy { get; private set; }

        public override void Execute()
        {
            Retain();
            Debug.Log("Checking PlayerPrefs encryption.");
            if (PlayerPrefs.HasKey("FirstLoginTimestamp"))
            {
                Debug.Log("Not a first run. Reading Key:");
                PlayerPrefsModel.strategy = encryptedPlayerPrefsStrategy;
                PlayerPrefsModel.UpdateCryptorKey();
            }
            else
            {
                Debug.Log("First run. Creating key.");
                PlayerPrefsModel.strategy = straightPlayerPrefsStrategy;
                PlayerPrefsModel.UpdateToStrategy(encryptedPlayerPrefsStrategy);
            }
            Release();
        }
    }
}