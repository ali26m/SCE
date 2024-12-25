// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.Networking;
// using System.Collections;
// using System.IO;
// using UnityEngine.Video; 

// public class ARCameraCaptureController : MonoBehaviour
// {
//     public Camera arCamera; // Assign the AR camera
//     public RawImage rawImage; // Reference to RawImage in UI
//     public VideoPlayer videoPlayer; // Reference to VideoPlayer
//     private Texture2D texture;

//     void Start()
//     {
//         Debug.Log("Started");
//         // Start capturing frames periodically
//         InvokeRepeating(nameof(CaptureFrameAndSend), 0f, 1f); // Adjust interval as needed
//     }

//     void CaptureFrameAndSend()
//     {
//         Debug.Log("before capture");
//         // Create a new RenderTexture
//         RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
//         arCamera.targetTexture = renderTexture;
        
//         // Render camera's view into the texture
//         arCamera.Render();

//         // Create a new Texture2D and read pixels
//         texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
//         RenderTexture.active = renderTexture;
//         texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
//         texture.Apply();

//         Debug.Log("after capture");

//         // Save captured image
//         Color pixelColor = texture.GetPixel(100, 100);
//         Debug.Log("pixel color from unity " + pixelColor);

//         // Cleanup
//         arCamera.targetTexture = null;
//         RenderTexture.active = null;
//         Destroy(renderTexture);

//         // Send texture to Flask API
//         StartCoroutine(SendFrameToAPI(texture));
//     }

//     IEnumerator SendFrameToAPI(Texture2D frame)
//     {
//         Debug.Log("Sending frame to API");
//         // Encode texture to PNG
//         byte[] imageBytes = frame.EncodeToPNG();
        
//         WWWForm form = new WWWForm();
//         form.AddBinaryData("frame", imageBytes, "frame.jpg", "image/jpeg");

//         // Send the POST request
//         UnityWebRequest www = UnityWebRequest.Post("http://192.168.1.6:5000", form);
//         yield return www.SendWebRequest();

//         if (www.result == UnityWebRequest.Result.Success)
//         {
//             Debug.Log("Image sent successfully!");
//             string responseText = www.downloadHandler.text;
//             Debug.Log("API Response: " + responseText);
//             GestureResponse response = JsonUtility.FromJson<GestureResponse>(responseText);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);
//             Debug.Log("Gesture: " + response.gesture);

//         }
//         else
//         {
//             Debug.LogError("Error: " + www.error);
//         }

//         RenderTexture.active = null;
//         arCamera.targetTexture = null;
//         Destroy(frame);
//     }
//  private void ControlVideo(string gesture)
//     {
//         switch (gesture)
//         {
//             case "play":
//                 if (!videoPlayer.isPlaying)
//                     videoPlayer.Play();
//                 Debug.Log("Video playing");
//                 break;
//             case "pause":
//                 if (videoPlayer.isPlaying)
//                     videoPlayer.Pause();
//                 Debug.Log("Video paused");
//                 break;
//             case "stop":
//                 videoPlayer.Stop();
//                 Debug.Log("Video stopped");
//                 break;
//             case "volumeup":
//                 videoPlayer.SetDirectAudioVolume(0, Mathf.Clamp(videoPlayer.GetDirectAudioVolume(0) + 0.1f, 0, 1)); // Increase volume
//                 Debug.Log("Volume up");
//                 break;
//             case "volumedown":
//                 videoPlayer.SetDirectAudioVolume(0, Mathf.Clamp(videoPlayer.GetDirectAudioVolume(0) - 0.1f, 0, 1)); // Decrease volume
//                 Debug.Log("Volume down");
//                 break;
//             default:
//                 Debug.Log("Unknown gesture");
//                 break;
//         }
//     }
//     [System.Serializable]
//     public class PixelColorResponse
//     {
//         public int[] color;
//     }
// }
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using UnityEngine.Video; 
using TMPro;

public class ARCameraCaptureController : MonoBehaviour
{
    public Camera arCamera; // Assign the AR camera
    public RawImage rawImage; // Reference to RawImage in UI
    public VideoPlayer videoPlayer; // Reference to VideoPlayer
    private float currentVolume = 0.5f; // Initial volume
    private string lastGesture = ""; // To prevent repeated actions for the same gesture
    public TMP_Text textObject; // Reference to the Text component
    private Texture2D texture;

    void Start()
    {
        Debug.Log("Started");
        // Start capturing frames periodically
        InvokeRepeating(nameof(CaptureFrameAndSend), 0f, 1f); // Adjust interval as needed
    }

    void CaptureFrameAndSend()
    {
        Debug.Log("before capture");
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

        Debug.Log("after capture");

        // Save captured image
        Color pixelColor = texture.GetPixel(100, 100);
        Debug.Log("pixel color from unity " + pixelColor);

        // Cleanup
        arCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        // ControlVideo("volumedown");

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
        UnityWebRequest www = UnityWebRequest.Post("http://192.168.1.30:5000", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Image sent successfully!");
            string responseText = www.downloadHandler.text;
            Debug.Log("API Response: " + responseText);

            GestureResponse response = JsonUtility.FromJson<GestureResponse>(responseText);
            Debug.Log("Gesture: " + response.gesture);

            response.gesture = "volumedown"; // For testing
            ControlVideo(response.gesture);

            // if (response.gesture != lastGesture)
            // {
            //     lastGesture = response.gesture; // Update the last gesture
            //     ControlVideo(response.gesture);
            // }

        }
        else
        {
            Debug.LogError("Error: " + www.error);
        }

        RenderTexture.active = null;
        arCamera.targetTexture = null;
        Destroy(frame);
    }
 private void ControlVideo(string gesture)
    {   
        double targetTime;

        switch (gesture)
        {
            case "playback":
                if (!videoPlayer.isPlaying){
                    videoPlayer.Play();
                    textObject.text = "Playing";
                }
                else{
                    videoPlayer.Pause();
                    textObject.text = "Paused";
                }
                break;
            
            case "skip":
                targetTime = videoPlayer.time + 5;
                videoPlayer.time = Mathf.Clamp((float)targetTime, 0, (float)videoPlayer.length);
                textObject.text = "Skipped";
                break;

            case "drawback":
                targetTime = videoPlayer.time + (-5);
                videoPlayer.time = Mathf.Clamp((float)targetTime, 0, (float)videoPlayer.length);
                textObject.text = "Drawback";
                break;

            case "volumeup":
                currentVolume = Mathf.Clamp(currentVolume + 0.1f, 0f, 1f);
                videoPlayer.SetDirectAudioVolume(0, currentVolume);
                textObject.text = "Volume Up";
                break;

            case "volumedown":
                currentVolume = Mathf.Clamp(currentVolume + (-0.1f), 0f, 1f);
                videoPlayer.SetDirectAudioVolume(0, currentVolume);
                textObject.text = "Volume Down";
                break;

            default:
                textObject.text = "none";
                break;
        }
    }
    [System.Serializable]
    public class PixelColorResponse
    {
        public int[] color;
    }

    [System.Serializable]
    public class GestureResponse
    {
        public string gesture;
    }

}













// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.Networking;
// using System.Collections;
// using System.IO;

// public class ARCameraCaptureController : MonoBehaviour
// {
//     public Camera arCamera; // Assign the AR camera

//     private Texture2D texture;

//     void Start()
//     {
//         Debug.Log("Started");
//         // Start capturing frames periodically
//         InvokeRepeating(nameof(CaptureFrameAndSend), 0f, 1f); // Adjust interval as needed
//     }

//     void CaptureFrameAndSend()
//     {
//         Debug.Log("before capture");
//         // Create a new RenderTexture
//         RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
//         arCamera.targetTexture = renderTexture;
        
//         // Render camera's view into the texture
//         arCamera.Render();

//         // Create a new Texture2D and read pixels
//         texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
//         RenderTexture.active = renderTexture;
//         texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
//         texture.Apply();

//         Debug.Log("after capture");

//         // Save captured image
//         Color pixelColor = texture.GetPixel(100, 100);
//         Debug.Log("pixel color from unity " + pixelColor);

//         // Cleanup
//         arCamera.targetTexture = null;
//         RenderTexture.active = null;
//         Destroy(renderTexture);

//         // Send texture to Flask API
//         StartCoroutine(SendFrameToAPI(texture));
//     }

//     IEnumerator SendFrameToAPI(Texture2D frame)
//     {
//         Debug.Log("Sending frame to API");
//         // Encode texture to PNG
//         byte[] imageBytes = frame.EncodeToPNG();
        
//         WWWForm form = new WWWForm();
//         form.AddBinaryData("frame", imageBytes, "frame.jpg", "image/jpeg");

//         // Send the POST request
//         UnityWebRequest www = UnityWebRequest.Post("http://192.168.1.6:5000", form);
//         yield return www.SendWebRequest();

//         if (www.result == UnityWebRequest.Result.Success)
//         {
//             Debug.Log("Image sent successfully!");
//             Debug.Log("pixel color from api " + www.downloadHandler.text);
//         }
//         else
//         {
//             Debug.LogError("Error: " + www.error);
//         }

//         RenderTexture.active = null;
//         arCamera.targetTexture = null;
//         Destroy(frame);
//     }

//     [System.Serializable]
//     public class PixelColorResponse
//     {
//         public int[] color;
//     }
// }
