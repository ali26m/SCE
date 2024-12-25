using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class ARCameraCaptureController : MonoBehaviour
{
    public Camera arCamera; // Assign the AR camera

    private Texture2D texture;

    void Start()
    {
        Debug.Log("Started");
        // Start capturing frames periodically
        InvokeRepeating(nameof(CaptureFrameAndSend), 0f, 1f); // Adjust interval as needed
    }

    void CaptureFrameAndSend()
    {
        Debug.Log("before captire");
        // Create a new RenderTexture
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        arCamera.targetTexture = renderTexture;
        
        // Render camera's view into the texture
        arCamera.Render();

        // Create a new Texture2D and read pixels
        texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        Debug.Log("after captire");

        // Save captured image
        Color pixelColor = texture.GetPixel(100, 100); // Get the color of pixel at (1, 1)
        Debug.Log("Pixel Color Value From Unity : " + pixelColor);

        // Cleanup
        arCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        // Send texture to Flask API
        StartCoroutine(SendFrameToAPI(texture));
    }

    IEnumerator SendFrameToAPI(Texture2D frame)
    {
        Debug.Log("Sending frame to API");
        // Encode texture to PNG
        byte[] imageBytes = frame.EncodeToPNG();
        

        WWWForm form = new WWWForm();
        form.AddBinaryData("frame", imageBytes, "frame.jpg", "image/jpeg");

        // Send the POST request
        UnityWebRequest www = UnityWebRequest.Post("http://192.168.1.6:5000", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Image sent successfully!");
            Debug.Log(www.downloadHandler.text);
            GestureResponse gestureResponse = JsonUtility.FromJson<GestureResponse>(responseText);
            Debug.Log("Gesture Detected: " + gestureResponse.gesture);
        }
        else
        {
            Debug.LogError("Error: " + www.error);
        }

        RenderTexture.active = null;
        arCamera.targetTexture = null;
    }

    [System.Serializable]
    public class PixelColorResponse
    {
        public int[] color;
    }
}
