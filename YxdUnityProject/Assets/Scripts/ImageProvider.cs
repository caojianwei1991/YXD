/*using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ImageProvider : MonoBehaviour
{
    static ImageProvider instance;
    static readonly string RemoteImageCachePath = DirectoryTool.persistentDataPath + "/ImageCache/";

    public static ImageProvider Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject();
                instance = go.AddComponent<ImageProvider>();
                DontDestroyOnLoad(go);

                // 如果缓存目录不存在则创建
                if (!Directory.Exists(RemoteImageCachePath))
                {
                    Directory.CreateDirectory(RemoteImageCachePath);
                }
            }

            return instance;
        }
    }

    /// <summary>
    /// 为可拉取缓存的图片显示框设置默认图片和远程图片地址
    /// </summary>
    public void SetRemoteImage(UITexture texture, string imageUrl)
    {
        if (imageUrl == null)
        {
            return;
        }
        //判断是否是第一次加载这张图片
        //图片文件名采用的是原URL的哈希码
        string targetPath = RemoteImageCachePath + imageUrl.GetHashCode();
        if (!File.Exists(targetPath))
        {
            //如果之前不存在缓存图片,下载图片
            StartCoroutine(DownloadImage(texture, imageUrl, targetPath));
        }
        //存在的，直接异步本地加载图片
        else
        {
            StartCoroutine(LoadCachedImage(texture, targetPath));
        }
    }

    /// <summary>
    /// 为图片显示框设置随StreamAssets分发的图片
    /// </summary>
    /// <param name="localImagePath">图片相对于StreamAssets目录的路径</param>
    public void SetLocalImage(UITexture texture, string localImagePath)
    {
        StartCoroutine(LoadLocalImage(texture, localImagePath));
    }

    /// <summary>
    /// 为图片显示框设置本地存储的图片
    /// </summary>
    /// <param name="cachedImagePath">图片在本地存储的路径</param>
    public void SetCachedImage(UITexture texture, string cachedImagePath)
    {
        StartCoroutine(LoadCachedImage(texture, cachedImagePath));
    }

    //第一次加载图片，这个URL对应的文件不存在，那么我们就去原URL下载图片
    IEnumerator DownloadImage(UITexture texture, string url, string targetPath)
    {
        Logger.Log("Image URL = " + url);

        WWW www = new WWW(url);
        yield return www;

        // 防止空图片造成额外BUG风险
        if (www.bytes.Length < 8) {
            Logger.Log("Directly load failed: image data is empty or too small");
            yield break;
        }

        byte[] imageBytes;
        Texture2D imageTexture;
        // 判断图片是否为GIF
        if (www.bytes[0] == 0x47 && www.bytes[1] == 0x49 && www.bytes[2] == 0x46)
        {
            int loop, w, h;
            List<UniGif.GifTexture> gifTexList = new List<UniGif.GifTexture>();
            gifTexList = UniGif.GetTextureList(www.bytes, out loop, out w, out h);
            if (gifTexList != null && gifTexList.Count > 0) {
                imageBytes = gifTexList[0].texture2d.EncodeToPNG();
                imageTexture = gifTexList[0].texture2d;
                Logger.Log("GIF decoding finished.");
            } else {
                Logger.Log("GIF decoding failed.");
                yield break;
            }
        }
        else
        {
            imageBytes = www.bytes;
            imageTexture = www.texture;
        }

        try
        {
            // 保存图片到ImageCache
            File.WriteAllBytes(targetPath, imageBytes);
        }
        catch (System.Exception ex) { Debug.Log(ex.Message); }

        imageBytes = null;

        // 将获取到的数据设置到texture，这里需要注意texture对应的GameObject有可能已经被Destroy掉
        if (texture != null)
        {
            texture.mainTexture = imageTexture;
        }
        
        Logger.Log("Directly load finished");
    }

    //从缓存文件夹读取已经存在的图片方法
    IEnumerator LoadCachedImage(UITexture texture, string cachedPath)
    {
        string filePath = @"file://" + cachedPath;

        WWW www = new WWW(filePath);
        yield return www;

        // 直接贴图，这里需要注意texture对应的GameObject有可能已经被Destroy掉
        if (texture != null)
        {
            texture.mainTexture = www.texture;
        }
    }

    /// <summary>
    /// 加载存储在Unity中StreamingAssets中的文件
    /// </summary>
    /// <param name="filePath">图片相对于StreamAssets目录的路径</param>
    /// <returns></returns>
    IEnumerator LoadLocalImage(UITexture texture, string localPath)
    {
        WWW www = new WWW(DirectoryTool.LocalPathToStreamingAssetsPath(localPath));
        yield return www;

        // 直接贴图，这里需要注意texture对应的GameObject有可能已经被Destroy掉
        if (texture != null)
        {
            texture.mainTexture = www.texture;
        }
    }
}
*/