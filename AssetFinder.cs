using UnityEditor;
using UnityEngine;

public static class AssetFinder
{
    public static bool TryGetAsset<T>(string folderPath, out T result) where T : Object
    {
        result = default;

        var filter = $"t:{typeof(T).Name}";
        var guids = AssetDatabase.FindAssets(filter, new[] { folderPath });

        if (guids.Length <= 0)
        {
            Debug.LogWarning($"No asset of type {typeof(T).Name} was found.");
            return false;
        }

        var filePath = AssetDatabase.GUIDToAssetPath(guids[0]);
        result = AssetDatabase.LoadAssetAtPath<T>(filePath);

        return true;
    }
    
    public static bool TryGetAsset<TSearch, TResult>(string folderPath, out TResult result) 
        where TSearch : TResult
        where TResult : Object
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

