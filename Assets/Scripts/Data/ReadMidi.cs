using UnityEngine;
using UnityEngine.Networking;

public class ReadMidi
{
    public static async Awaitable<byte[]> Load(TuneData tune)
    {
        string path = Application.streamingAssetsPath + "/" + tune.midiFile;

        string url = path.Contains("://") ? path : new System.Uri(path).AbsoluteUri;

        using var request = UnityWebRequest.Get(url);
        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            throw new System.IO.FileNotFoundException(
                $"Could not load MIDI for '{tune.title}' from {url}: {request.error}");

        return request.downloadHandler.data;

    }
}
