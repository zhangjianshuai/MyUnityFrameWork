﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class ResourcesConfigManager 
{
    public const string c_ManifestFileName = "ResourcesManifest";



    public const string c_relyBundleKey = "relyBundles";
    public const string c_bundlesKey = "AssetsBundles";

    public static Dictionary<string, ResourcesConfig> m_relyBundleConfigs;
    public static Dictionary<string, ResourcesConfig> m_bundleConfigs ;

    public static void Initialize()
    {
        ResourcesConfigStruct result = GetResourcesConfig();

        m_relyBundleConfigs = result.relyList;
        m_bundleConfigs = result.bundleList;
    }

    public static ResourcesConfig GetBundleConfig(string bundleName)
    {
        if (m_bundleConfigs == null)
        {
            throw new Exception("RecourcesConfigManager GetBundleConfig : bundleConfigs is null  do you Initialize?");
        }

        if (m_bundleConfigs.ContainsKey(bundleName))
        {
            return m_bundleConfigs[bundleName];
        }
        else
        {
            throw new Exception("RecourcesConfigManager GetBundleConfig : Dont find ->" + bundleName + "<- please check BundleConfig!");
        }
    }

    public static ResourcesConfig GetRelyBundleConfig(string bundleName)
    {
        if (m_relyBundleConfigs == null)
        {
            throw new Exception("ResourcesConfigManager GetRelyBundleConfig Exception: relyBundleConfigs is null do you Initialize?");
        }

        if (m_relyBundleConfigs.ContainsKey(bundleName))
        {
            return m_relyBundleConfigs[bundleName];
        }
        else
        {
            throw new Exception("ResourcesConfigManager GetRelyBundleConfig Exception: Dont find ->" + bundleName + "<- please check BundleConfig!");
        }
    }

    //资源路径数据不依赖任何其他数据
    public static ResourcesConfigStruct GetResourcesConfig()
    {
        string dataJson = "";

        dataJson = ReadResourceConfigContent();

        if (dataJson == "")
        {
            throw new Exception("ResourcesConfig not find " + c_ManifestFileName);
        }
        else
        {
            return AnalysisResourcesConfig2Struct(dataJson);
        }
    }



    public static string ReadResourceConfigContent()
    {
        string dataJson = "";

        if (ResourceManager.m_gameLoadType == ResLoadLocation.Resource)
        {
            dataJson = ResourceIOTool.ReadStringByResource(
                c_ManifestFileName + "." + ConfigManager.c_expandName);
        }
        else
        {
            ResLoadLocation type = ResLoadLocation.Streaming;

            if (RecordManager.GetData(HotUpdateManager.c_HotUpdateRecordName).GetRecord(HotUpdateManager.c_useHotUpdateRecordKey, false))
            {
                type = ResLoadLocation.Persistent;
            }

            dataJson = ResourceIOTool.ReadStringByFile(
                PathTool.GetAbsolutePath(
                     type,
                     c_ManifestFileName + "." + ConfigManager.c_expandName));
        }

        return dataJson;
    }

    public static ResourcesConfigStruct AnalysisResourcesConfig2Struct(string content)
    {
        if (content == null || content =="")
        {
            throw new Exception("ResourcesConfigcontent is null ! ");
        }

        ResourcesConfigStruct result = new ResourcesConfigStruct();

        Dictionary<string, object> data = (Dictionary<string, object>)MiniJSON.Json.Deserialize(content);

        Dictionary<string, object> gameRelyBundles = (Dictionary<string, object>)data[c_relyBundleKey];
        Dictionary<string, object> gameAssetsBundles = (Dictionary<string, object>)data[c_bundlesKey];

        result.relyList = new Dictionary<string, ResourcesConfig>();
        result.bundleList = new Dictionary<string, ResourcesConfig>();
        foreach (object item in gameRelyBundles.Values)
        {
            Dictionary<string, object> tmp = (Dictionary<string, object>)item;

            ResourcesConfig config = new ResourcesConfig();
            config.name = (string)tmp["name"];
            config.path = (string)tmp["path"];
            config.relyPackages = ((string)tmp["relyPackages"]).Split('|');
            config.md5 = (string)tmp["md5"];

            result.relyList.Add(config.name,config);
        }

        foreach (object item in gameAssetsBundles.Values)
        {
            Dictionary<string, object> tmp = (Dictionary<string, object>)item;

            ResourcesConfig config = new ResourcesConfig();
            config.name = (string)tmp["name"];
            config.path = (string)tmp["path"];
            config.relyPackages = ((string)tmp["relyPackages"]).Split('|');
            config.md5 = (string)tmp["md5"];

            result.bundleList.Add(config.name,config);
        }

        return result;
    }

    public static string SerializeResourcesConfig()
    {
        return "";
    }
}

public class ResourcesConfig
{
    public string name;               //名称
    public string path;               //加载相对路径
    public string[] relyPackages;     //依赖包
    public string md5;                //md5
}

public class ResourcesConfigStruct
{
    public Dictionary<string,ResourcesConfig> relyList;
    public Dictionary<string, ResourcesConfig> bundleList;
}

