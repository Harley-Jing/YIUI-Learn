using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using I2.Loc;
using Sirenix.OdinInspector;
using UnityEngine;
using YIUI.Login;
using YIUIFramework;
using YooAsset;
using Object = UnityEngine.Object;

public class Main : MonoBehaviour
{
    private ResourcePackage package;
    
    private void Awake()
    {
        SingletonMgr.Initialize();
        UIBindHelper.InternalGameGetUIBindVoFunc = YIUICodeGenerated.UIBindProvider.Get;
        YIUILoadDI.LoadAssetFunc = LoadAsset;
        YIUILoadDI.LoadAssetAsyncFunc = LoadAssetAsync;
        YIUILoadDI.ReleaseAction = ReleaseAction;
        
        YooAssets.Initialize();
        package = YooAssets.TryGetPackage("DefaultPackage") ?? YooAssets.CreatePackage("DefaultPackage");
        YooAssets.SetDefaultPackage(package);
    }

    private void Start()
    {
        UniTask.Void(async () =>
        {
            await InitYooAsset();
            await MgrCenter.Inst.Register(CountDownMgr.Inst);
            await MgrCenter.Inst.Register(RedDotMgr.Inst);
            await MgrCenter.Inst.Register(PanelMgr.Inst);
            OpenLogin();
        });
    }

    [Button]
    private void OpenLogin()
    {
        PanelMgr.Inst.OpenPanel<LoginPanel>();
    }

    #region 资源加载
    
    private Dictionary<int, AssetHandle> allAssetHandles = new Dictionary<int, AssetHandle>();

    private void ReleaseAction(int hashCode)
    {
        if (allAssetHandles.Remove(hashCode, out AssetHandle handle))
        {
            handle.Release();
        }
    }

    private async UniTask<(Object, int)> LoadAssetAsync(string arg1, string arg2, Type arg3)
    {
        var handle = package.LoadAssetAsync(arg2, arg3);
        await handle.ToUniTask();
        return LoadAssetHandle(handle);
    }

    private (Object, int) LoadAsset(string arg1, string arg2, Type arg3)
    {
        var handle = package.LoadAssetSync(arg2, arg3);
        return LoadAssetHandle(handle);
    }

    private (Object, int) LoadAssetHandle(AssetHandle handle)
    {
        if (handle.AssetObject != null)
        {
            var hashCode  = handle.GetHashCode();
            allAssetHandles.Add(hashCode, handle);
            return (handle.AssetObject, hashCode);
        }
        else
        {
            handle.Release();
            return (null, 0);
        }
    }

    private async UniTask InitYooAsset()
    {
        var initializationOperation = InitPackage();
        await initializationOperation.ToUniTask();
            
        if (initializationOperation.Status != EOperationStatus.Succeed)
        {
            Debug.LogError("Initialization operation failed: " + initializationOperation.Error);
            return;
        }
            
        var packageVersionOperation = RequestPackageVersionAsync();
        await packageVersionOperation.ToUniTask();
        Debug.Log($"Package版本：{packageVersionOperation.PackageVersion}");

        if (packageVersionOperation.Status != EOperationStatus.Succeed)
        {
            Debug.LogError("Request package version operation failed: " + packageVersionOperation.Error);
            return;
        }

        var packageManifestOperation = UpdatePackageManifestAsync(packageVersionOperation.PackageVersion);
        await packageManifestOperation.ToUniTask();

        if (packageManifestOperation.Status != EOperationStatus.Succeed)
        {
            Debug.LogError("Update package manifest operation failed: " + packageManifestOperation.Error);
            return;
        }

        Debug.Log("Update package manifest success");
    }

    /// <summary>
    /// 初始化资源包的运行模式
    /// </summary>
    /// <param name="packageName"></param>
    /// <returns></returns>
    private InitializationOperation InitPackage()
    {
        InitializeParameters initParameters = null;

        initParameters = new EditorSimulateModeParameters()
        {
            EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(EditorSimulateModeHelper.SimulateBuild("DefaultPackage").PackageRootDirectory),
        };
        
        return package.InitializeAsync(initParameters);
    }

    /// <summary>
    /// 获取资源版本
    /// </summary>
    /// <param name="packageName"></param>
    /// <returns></returns>
    private RequestPackageVersionOperation RequestPackageVersionAsync()
    {
        return package.RequestPackageVersionAsync();
    }

    /// <summary>
    /// 更新资源包清单
    /// </summary>
    /// <param name="packageVersion"></param>
    /// <param name="packageName"></param>
    /// <returns></returns>
    private UpdatePackageManifestOperation UpdatePackageManifestAsync(string packageVersion)
    {
        return package.UpdatePackageManifestAsync(packageVersion);
    }

    #endregion
}