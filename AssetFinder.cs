#if UNITY_EDITOR
using Sirenix.Utilities;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class AssetFinder
{
    public static bool TryGetAssets<T>(string folderPath, out T[] results) where T : UnityEngine.Object
    {
        results = Array.Empty<T>();

        var filter = $"t:{typeof(T).Name}";
        var guids = AssetDatabase.FindAssets(filter, new[] { folderPath });

        if (guids.Length <= 0)
        {
            Debug.LogWarning($"No assets of type {typeof(T).Name} were found.");
            return false;
        }

        results = new T[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            var filePath = AssetDatabase.GUIDToAssetPath(guids[i]);
            results[i] = AssetDatabase.LoadAssetAtPath<T>(filePath);
        }

        return true;
    }

    public static bool TryGetAsset<T>(string folderPath, out T result) where T : UnityEngine.Object
    {
        result = default;

        if (!TryGetAssets(folderPath, out T[] results))
            return false;

        result = results[0];
        return true;
    }

    public static bool TryGetAsset<TSearch, TResult>(string folderPath, out TResult result) 
        where TSearch : TResult
        where TResult : UnityEngine.Object
    {
        result = default;

        if (!TryGetAsset(folderPath, out TSearch rawResult))
            return false;

        if (rawResult is not TResult casted)
        {
            Debug.LogWarning(
                $"Asset type mismatch in '{folderPath}'. " +
                $"Expected {typeof(TResult).Name}, but got {rawResult.GetType().Name}.");
            return false;
        }

        result = casted;
        return true;
    }

    //Sprite Assets
    public static Sprite[] GetSprites(string folderPath, string fileName)
    {
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogWarning($"Invalid folder path: {folderPath}");
            return Array.Empty<Sprite>();
        }

        var guids = AssetDatabase.FindAssets("t:Sprite", new[] { folderPath });

        return guids
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(path => Path.GetFileNameWithoutExtension(path).Equals(fileName))
            .SelectMany(path => AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>())
            .ToArray();
    }

    public static Sprite GetSprite(string folderPath, string fileName, int index = 0)
    {
        var sprites = GetSprites(folderPath, fileName);
        return sprites.IsNullOrEmpty() ? null : sprites[index];
    }
}
#endif
