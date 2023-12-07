using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace beginGame
{
    public class BeginGame : MonoBehaviour
    {
        public static BeginGame Instance { get; private set; }
        public Button beginGameButton;
        public readonly Dictionary<string, GameObject> startUIDict = new();
        public readonly Dictionary<string, GameObject> battleSceneUIDict = new();
        public TextMeshProUGUI processText;

        // private AsyncOperationHandle<IList<GameObject>> handle;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Addressables.ClearResourceLocators();
            Addressables.ClearDependencyCacheAsync("ui");
            // loadoo();
            LoadStartSceneResources2();
            // StartCoroutine(DownloadAndLoadAssets());
            beginGameButton.onClick.AddListener(LoadStartGameScene);
        }

        private IEnumerator DownloadAndLoadAssets()
        {
            // Step 1: Download dependencies
            AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync("ui");
            while (!downloadHandle.IsDone)
            {
                processText.text = "Process: " + downloadHandle.GetDownloadStatus().Percent * 100 + "%";
                yield return null;
            }

            yield return downloadHandle;

            if (downloadHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("Failed to download dependencies!");
                yield break;
            }

            Addressables.LoadAssetsAsync<GameObject>("ui", null).Completed += loadHandle =>
            {
                if (loadHandle.Status != AsyncOperationStatus.Succeeded)
                {
                    foreach (var ui in loadHandle.Result)
                    {
                        print("ui.name: " + ui.name);
                        startUIDict.Add(ui.name, ui);
                    }
                }
                else
                {
                    print("failed");
                    
                }
            };
        }

        private void LoadStartSceneResources2()
        {
            StartCoroutine(loadStartSceneResources());
        }

        private IEnumerator loadStartSceneResources()
        {
            AsyncOperationHandle handle = Addressables.DownloadDependenciesAsync("ui");
            // yield return handle;
            while (!handle.IsDone)
            {
                processText.text = "Process: " + handle.GetDownloadStatus().Percent * 100 + "%";
                print("handle.PercentComplete: " + handle.PercentComplete);
                yield return null;
            }

            handle.Completed += operation => { print("operation.Status: " + operation.Status); };
            
            Addressables.LoadAssetsAsync<GameObject>("ui", null).Completed += asyncOperationHandle =>
            {
                foreach (var ui in asyncOperationHandle.Result)
                {
                    print("ui.name: " + ui.name);
                    startUIDict.Add(ui.name, ui);
                }
            };
        }

        private void loadoo()
        {
            //     .Completed += handle =>
            // {
            //     foreach (var ui in handle.Result)
            //     {
            //         print("ui.name: " + ui.name);
            //         startUIDict.Add(ui.name, ui);
            //     }
            // };
            StartCoroutine(star());
        }

        private IEnumerator star()
        {
            var handle = Addressables.LoadAssetsAsync<GameObject>("ui", null);
            while (!handle.IsDone)
            {
                processText.text = "Process: " + handle.GetDownloadStatus().Percent * 100 + "%";
                yield return null;
            }

            handle.Completed += operation =>
            {
                foreach (var ui in operation.Result)
                {
                    print("ui.name: " + ui.name);
                    startUIDict.Add(ui.name, ui);
                }
            };
        }


        private void LoadStartGameScene()
        {
            SceneManager.LoadScene(1);
        }
    }
}