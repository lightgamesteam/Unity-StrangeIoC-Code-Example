using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace PFS.Assets.Scripts.Commands.Localization
{
    public class UpdateLocalizationCommand : BaseCommand
    {
        [Inject]
        public IExecutor courutine { get; private set; }

        public override void Execute()
        {
            Retain();
            Debug.Log("Update localization");
            courutine.Execute(SaveLocalization());
        }

        private IEnumerator SaveLocalization()
        {
            string url = "https://doc-00-bs-sheets.googleusercontent.com/export/4kmq2rmgonn2n695hp97caf5hs/jkops87vnc9fqi4djcmnlm8ros/1594224710000/108506411229129043867/108506411229129043867/106Y3i_F7U1OHAR_hGVbhHXiSooJ1O_zr6Oj49c2JRzg?format=csv&id=106Y3i_F7U1OHAR_hGVbhHXiSooJ1O_zr6Oj49c2JRzg&gid=0&dat=AMvwolZce9nsUX2McKOWfp9Y0JaQ0bebV-FlMROJbSW9V8RkEzFWfmW6CVYWw9NqAFO0fiKsUirMfac1BBW9CpdfUUrNrfVkuwPmiG3gQYdtTH5ptlJySZPiVDj0E7nFBYGXjYUdVvH4RrgOHBpY2h0MhGka19lfi0fg-OULC9qg79aAaFsDjiwEPvMo6jv9yR4dMOBf-pcPUzCwGg5l3-534Nn-mJ77_0rL5M2VepBgqTy_uk7KvKOEpy1vbUyUtUbOt6tPNjj-a9hUR3CVoO0nBfsJMEJjtO-YcYHlg8UCEzP3PaCdU-xZgK66Egjf-2Ia9SrHFRPP6FWs-CtKNXO1AbI6HT1PdhZL82KvDAQ9uGGpt508uWxM7Ghw8xXwoe8i0JSXDsq2gZqSp9Uxpcc6HLa8htg8vN_v_nGbmYS_m0F1y6ZYZEEguNDyk57rnjFKzjf6MTOI5kNQNNNiKGYsEJPJOm3euVM7ogdMVFjTXZFCSEbS4CfWmDv7uPnH3AU6crVjp9bDbC8zxZq42X_JOOfc-ICI3x6gs83Z5c3xkCL4ihpUw1NjreyP-dG2bRPBlVm3PwDe1B_Xxt47kHppBvu_OzwM7Z2dCEV2yK4CnWpz3yBTH0rZlQx-k4ozX5aKD4PC4VMr-XuwLWKXjSuLH1fRc5qf59HodVnzlUkf9SAPBMquv-LrftdLAYvD9VYjQOMH_Ihvh9QvbYDJapyaigb6TT-9";
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("Download Localization ERROR " + www.error);
            }
            else
            {
                Debug.Log("GetGlobalAsset_Noerror");
                Debug.Log(www.downloadHandler.text);
                string path = Application.dataPath + "/Resources/Localization/Localization.csv";
                File.WriteAllBytes(path, www.downloadHandler.data);
            }
            Release();
        }
    }
}