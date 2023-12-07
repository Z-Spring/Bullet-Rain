using System.Collections;
using System.Collections.Generic;
using beginGame;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ResManager : MonoBehaviour
{
    public static ResManager Instance { get; private set; }
    public readonly Dictionary<string, AssetBundle> loadedBundles = new();

    private AssetBundle file;
    private Dictionary<string, string> assetBundlePath;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        string basePath = Application.persistentDataPath + "/AssetBundles";

        assetBundlePath = new()
        {
            {
                "player", basePath + "/player"
            },
            {
                "bullet", basePath + "/bullet"
            },
            {
                "battle_scene_ui", basePath + "/battle_scene_ui"
            },
            {
                "effect", basePath + "/effect"
            },
        };
    }

    private void ResourcesLoad()
    {
    }

    private void OnEnable()
    {
        StartGameMain.InitBattleSceneResources += LoadBattleSceneResources;
    }

//
    private void OnDisable()
    {
        StartGameMain.InitBattleSceneResources -= LoadBattleSceneResources;
    }

    public void LoadStartSceneResources()
    {
        string uiPath = Application.persistentDataPath + "/AssetBundles/ui";
        StartCoroutine(LoadResourcesAsync(uiPath));
    }

   

    private void LoadBattleSceneResources()
    {
        foreach (var path in assetBundlePath.Values)
        {
            StartCoroutine(LoadResourcesAsync(path));
        }

        foreach (var key in loadedBundles.Keys)
        {
            Debug.Log("key: " + key);
        }
    }

    private IEnumerator LoadResourcesAsync(string path)
    {
        string splitPath = path.Split('/')[^1];
        AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(path);
        yield return assetBundleCreateRequest;
        loadedBundles[splitPath] = assetBundleCreateRequest.assetBundle;
    }

    public void LoadResultPanel()
    {
        string resultPanelPath = Application.persistentDataPath + "/AssetBundles/result_ui";
        StartCoroutine(LoadResourcesAsync(resultPanelPath));
    }


    /*private void LoadResources(string path, string resourceName)
    {
        if (file is null)
        {
            file = AssetBundle.LoadFromFile(path);
            LoadedPrefab = file.LoadAsset<GameObject>(resourceName);
        }
        else
        {
            LoadedPrefab = file.LoadAsset<GameObject>(resourceName);
        }
    }*/

    public void UnloadAssetBundle(string bundleName, bool unloadLoadedObjects)
    {
        if (loadedBundles.TryGetValue(bundleName, out AssetBundle assetBundle))
        {
            assetBundle.Unload(unloadLoadedObjects);
            loadedBundles.Remove(bundleName);
        }
    }
}


// ResManager.Instance.LoadPrefab("player");
// Instantite(ResManager.Instance.LoadedPrefab); 