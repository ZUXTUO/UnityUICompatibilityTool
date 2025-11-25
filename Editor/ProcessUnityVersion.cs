﻿using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ProcessUnityVersion
{
    [MenuItem("Tools/替换UI引用")]
    public static void Process()
    {
        var map = BuildTypeTargetScriptMap();
        ProcessAllPrefabsByType("*.prefab", map);
    }

    [MenuItem("Tools/反向替换UI引用")]
    public static void ReverseProcess()
    {
        var dict = BuildPrefabReplaceDict();
        ProcessAllPrefabs("*.prefab", dict, true);
    }

    private static Dictionary<string, string> BuildPrefabReplaceDict()
    {
        var groups = new List<(List<string> olds, string target)>
        {
            (new List<string> { "m_Script: {fileID: 11500000, guid: dc42784cf147c0c48a680349fa168899, type: 3}" }, "GameGraphicRaycaster"),
            (new List<string> { "m_Script: {fileID: 11500000, guid: 0cd44c1031e13a943bb63640046fad76, type: 3}" }, "GameCanvasScaler"),
            (new List<string> { "m_Script: {fileID: 11500000, guid: 5f7201a12d95ffc409449d95f23cf332, type: 3}",
                                "m_Script: {fileID: 11500000, guid: 880218c1f11e0054597c4282005949df, type: 3}",
                                "m_Script: {fileID: 708705254, guid: f70555f144d8491a825f0804e09c671c, type: 3}" }, "GameText"),
            (new List<string> { "m_Script: {fileID: 11500000, guid: fe87c0e1cc204ed48ad3b37840f39efc, type: 3}" }, "GameImage"),
            (new List<string> { "m_Script: {fileID: 11500000, guid: 4e29b1a8efbd4b44bb3f3716e73f07ff, type: 3}",
                                "m_Script: {fileID: 1392445389, guid: f70555f144d8491a825f0804e09c671c, type: 3}" }, "GameBtn"),
            (new List<string> { "m_Script: {fileID: 11500000, guid: 1344c3c82d62a2a41a3576d8abb8e3ea, type: 3}" }, "GameRawImage"),
            (new List<string> { "m_Script: {fileID: 11500000, guid: 0d0b652f32a2cc243917e4028fa0f046, type: 3}" }, "GameDropdown"),
            (new List<string> { "m_Script: {fileID: 11500000, guid: 1aa08ab6e0800fa44ae55d278d1423e3, type: 3}" }, "GameScrollRect"),
            (new List<string> { "m_Script: {fileID: 11500000, guid: 9085046f02f69544eb97fd06b6048fe2, type: 3}",
                                "m_Script: {fileID: 2109663825, guid: f70555f144d8491a825f0804e09c671c, type: 3}" }, "GameToggle"),
            (new List<string> { "m_Script: {fileID: 11500000, guid: 2a4db7a114972834c8e4117be1d82ba3, type: 3}" }, "GameScrollbar"),
            (new List<string> { "m_Script: {fileID: 11500000, guid: 67db9e8f0e2ae9c40bc1e2b64352a6b4, type: 3}",
                                "m_Script: {fileID: -113659843, guid: f70555f144d8491a825f0804e09c671c, type: 3}" }, "GameSlider"),
            (new List<string> { "m_Script: {fileID: 11500000, guid: 31a19414c41e5ae4aae2af33fee712f6, type: 3}",
                                "m_Script: {fileID: -1200242548, guid: f70555f144d8491a825f0804e09c671c, type: 3}" }, "GameMask"),
            (new List<string> { "m_Script: {fileID: 11500000, guid: 3312d7739989d2b4e91e6319e9a96d76, type: 3}" }, "GameRectMask2D"),
            (new List<string> { "m_Script: {fileID: 11500000, guid: 3245ec927659c4140ac4f8d17403cc18, type: 3}",
                                "m_Script: {fileID: 1741964061, guid: f70555f144d8491a825f0804e09c671c, type: 3}" }, "GameContentSizeFitter"),
            (new List<string> { "m_Script: {fileID: 11500000, guid: 8a8695521f0d02e499659fee002a26c2, type: 3}" }, "GameGridLayoutGroup"),
            (new List<string> { "m_Script: {fileID: 11500000, guid: 76c392e42b5098c458856cdf6ecaaaa1, type: 3}" }, "GameEventSystem"),
            (new List<string> { "m_Script: {fileID: 11500000, guid: 4f231c4fb786f3946a6b90b886c48677, type: 3}" }, "GameStandaloneInputModule"),
            (new List<string> { "m_Script: {fileID: 11500000, guid: e19747de3f5aca642ab2be37e372fb86, type: 3}",
                                "m_Script: {fileID: -900027084, guid: f70555f144d8491a825f0804e09c671c, type: 3}" }, "GameOutline"),
            (new List<string> { "m_Script: {fileID: 11500000, guid: 59f8146938fff824cb5fd77236b75775, type: 3}",
                                "m_Script: {fileID: 1297475563, guid: f70555f144d8491a825f0804e09c671c, type: 3}" }, "GameVerticalLayoutGroup"),
            (new List<string> { "m_Script: {fileID: 11500000, guid: 30649d3a9faa99c48a7b1166b86bf2a0, type: 3}" }, "GameHorizontalLayoutGroup"),
            (new List<string> { "m_Script: {fileID: 11500000, guid: d0b148fe25e99eb48b9724523833bab1, type: 3}" }, "GameEventTrigger"),
            (new List<string> { "m_Script: {fileID: 11500000, guid: 306cc8c2b49d7114eaa3623786fc2126, type: 3}" }, "GameLayoutElement"),
        };

        var dict = new Dictionary<string, string>();
        foreach (var g in groups)
        {
            var newYaml = ResolveScriptYaml(g.target);
            if (string.IsNullOrEmpty(newYaml)) continue;
            foreach (var old in g.olds)
            {
                dict[old] = newYaml;
            }
        }
        return dict;
    }

    private static string ResolveScriptYaml(string className)
    {
        var guids = AssetDatabase.FindAssets(className + " t:MonoScript", new[] { "Assets/UnityUICompatibilityTool/Scripts/UI" });
        if (guids != null && guids.Length > 0)
        {
            var guid = guids[0];
            return $"m_Script: {{fileID: 11500000, guid: {guid}, type: 3}}";
        }
        Debug.LogError("未找到脚本: " + className);
        return null;
    }

    private static List<(System.Type srcType, MonoScript targetScript)> BuildTypeTargetScriptMap()
    {
        var pairs = new List<(System.Type, MonoScript)>();
        AddPair(typeof(GraphicRaycaster), "GameGraphicRaycaster", pairs);
        AddPair(typeof(CanvasScaler), "GameCanvasScaler", pairs);
        AddPair(typeof(Text), "GameText", pairs);
        AddPair(typeof(Image), "GameImage", pairs);
        AddPair(typeof(Button), "GameBtn", pairs);
        AddPair(typeof(RawImage), "GameRawImage", pairs);
        AddPair(typeof(Dropdown), "GameDropdown", pairs);
        AddPair(typeof(ScrollRect), "GameScrollRect", pairs);
        AddPair(typeof(Toggle), "GameToggle", pairs);
        AddPair(typeof(Scrollbar), "GameScrollbar", pairs);
        AddPair(typeof(Slider), "GameSlider", pairs);
        AddPair(typeof(Mask), "GameMask", pairs);
        AddPair(typeof(RectMask2D), "GameRectMask2D", pairs);
        AddPair(typeof(ContentSizeFitter), "GameContentSizeFitter", pairs);
        AddPair(typeof(GridLayoutGroup), "GameGridLayoutGroup", pairs);
        AddPair(typeof(EventSystem), "GameEventSystem", pairs);
        AddPair(typeof(StandaloneInputModule), "GameStandaloneInputModule", pairs);
        AddPair(typeof(Outline), "GameOutline", pairs);
        AddPair(typeof(VerticalLayoutGroup), "GameVerticalLayoutGroup", pairs);
        AddPair(typeof(HorizontalLayoutGroup), "GameHorizontalLayoutGroup", pairs);
        AddPair(typeof(EventTrigger), "GameEventTrigger", pairs);
        AddPair(typeof(LayoutElement), "GameLayoutElement", pairs);
        return pairs;
    }

    private static void AddPair(System.Type srcType, string targetClass, List<(System.Type, MonoScript)> pairs)
    {
        var guids = AssetDatabase.FindAssets(targetClass + " t:MonoScript", new[] { "Assets/UnityUICompatibilityTool/Scripts/UI" });
        if (guids != null && guids.Length > 0)
        {
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            var mono = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            if (mono != null) pairs.Add((srcType, mono));
        }
    }

    private static void ProcessAllPrefabs(string form, Dictionary<string, string> replaceDict, bool reverse = false)
    {
        var resourcesPath = Application.dataPath;
        var absolutePaths = Directory.GetFiles(resourcesPath, form, SearchOption.AllDirectories);
        for (int i = 0; i < absolutePaths.Length; i++)
        {
            foreach (var kvp in replaceDict)
            {
                string oldValue = reverse ? kvp.Value : kvp.Key;
                string newValue = reverse ? kvp.Key : kvp.Value;
                ReplaceValue(absolutePaths[i], oldValue, newValue);
            }
            EditorUtility.DisplayProgressBar("处理预制体……", "处理预制体中……", (float)i / absolutePaths.Length);
        }
        EditorUtility.ClearProgressBar();
    }

    private static void ProcessAllPrefabsByType(string form, List<(System.Type srcType, MonoScript targetScript)> map)
    {
        var resourcesPath = Application.dataPath;
        var absolutePaths = Directory.GetFiles(resourcesPath, form, SearchOption.AllDirectories);
        for (int i = 0; i < absolutePaths.Length; i++)
        {
            var abs = absolutePaths[i];
            var rel = "Assets" + abs.Substring(Application.dataPath.Length);
            var root = PrefabUtility.LoadPrefabContents(rel);
            foreach (var pair in map)
            {
                ReplaceScriptOnComponents(root, pair.srcType, pair.targetScript);
            }
            PrefabUtility.SaveAsPrefabAsset(root, rel);
            PrefabUtility.UnloadPrefabContents(root);
            EditorUtility.DisplayProgressBar("处理预制体……", "处理预制体中……", (float)i / absolutePaths.Length);
        }
        EditorUtility.ClearProgressBar();
    }

    private static void ReplaceScriptOnComponents(GameObject root, System.Type srcType, MonoScript targetScript)
    {
        var components = root.GetComponentsInChildren(srcType, true);
        foreach (var c in components)
        {
            var so = new SerializedObject(c);
            var prop = so.FindProperty("m_Script");
            prop.objectReferenceValue = targetScript;
            so.ApplyModifiedProperties();
        }
    }

    /// <summary>
    /// 替换值
    /// </summary>
    /// <param name="strFilePath">文件路径</param>
    private static void ReplaceValue(string strFilePath, string oldLine, string newLine)
    {
        if (File.Exists(strFilePath))
        {
            string[] lines = File.ReadAllLines(strFilePath);
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Replace(oldLine, newLine);
            }
            File.WriteAllLines(strFilePath, lines);
        }
    }
}
