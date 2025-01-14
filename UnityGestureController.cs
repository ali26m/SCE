// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.Networking;
// using System.Collections;
// using System.IO;
// using UnityEngine.Video; 
// using TMPro;

// public class ARCameraCaptureController : MonoBehaviour
// {
//     public Camera arCamera; // Assign the AR camera
//     public RawImage rawImage; // Reference to RawImage in UI
//     public VideoPlayer videoPlayer; // Reference to VideoPlayer
//     private float currentVolume = 0.5f; // Initial volume
//     private string lastGesture = ""; // To prevent repeated actions for the same gesture
//     public TMP_Text textObject; // Reference to the Text component
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

//         // ControlVideo("volumedown");

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
//         UnityWebRequest www = UnityWebRequest.Post("http://127.0.0.1:8000/", form);
//         yield return www.SendWebRequest();

//         if (www.result == UnityWebRequest.Result.Success)
//         {
//             Debug.Log("Image sent successfully!");
//             string responseText = www.downloadHandler.text;
//             Debug.Log("API Response: " + responseText);

//             GestureResponse response = JsonUtility.FromJson<GestureResponse>(responseText);

//             // response.gesture = "volumedown"; // For testing
//             ControlVideo(response.gesture);

//             if (response.gesture != lastGesture)
//             {
//                 lastGesture = response.gesture; // Update the last gesture
//                 ControlVideo(response.gesture);
//             }

//         }
//         else
//         {
//             Debug.LogError("Error: " + www.error);
//         }

//         RenderTexture.active = null;
//         arCamera.targetTexture = null;
//         Destroy(frame);
//     }
// //  private void ControlVideo(string gesture)
// //     {   
// //         double targetTime;

// //         switch (gesture)
// //         {
// //             case "playback":
// //                 if (!videoPlayer.isPlaying){
// //                     videoPlayer.Play();
// //                     textObject.text = "Playing";
// //                 }
// //                 else{
// //                     videoPlayer.Pause();
// //                     textObject.text = "Paused";
// //                 }
// //                 break;
            
// //             case "skip":
// //                 targetTime = videoPlayer.time + 5;
// //                 videoPlayer.time = Mathf.Clamp((float)targetTime, 0, (float)videoPlayer.length);
// //                 textObject.text = "Skipped";
// //                 break;

// //             case "drawback":
// //                 targetTime = videoPlayer.time + (-5);
// //                 videoPlayer.time = Mathf.Clamp((float)targetTime, 0, (float)videoPlayer.length);
// //                 textObject.text = "Drawback";
// //                 break;

// //             case "volumeup":
// //                 currentVolume = Mathf.Clamp(currentVolume + 0.1f, 0f, 1f);
// //                 videoPlayer.SetDirectAudioVolume(0, currentVolume);
// //                 textObject.text = "Volume Up";
// //                 break;

// //             case "volumedown":
// //                 currentVolume = Mathf.Clamp(currentVolume + (-0.1f), 0f, 1f);
// //                 videoPlayer.SetDirectAudioVolume(0, currentVolume);
// //                 textObject.text = "Volume Down";
// //                 break;

// //             default:
// //                 textObject.text = "none";
// //                 break;
// //         }
// //     }
// private void ControlVideo(string gesture)
// {   
//     double targetTime;

//     switch (gesture)
//     {
//         case "playback":
//             if (!videoPlayer.isPlaying)
//             {
//                 videoPlayer.Play();
//                 textObject.text = "Playing";
//             }
//             else
//             {
//                 videoPlayer.Pause();
//                 textObject.text = "Paused";
//             }
//             break;

//         case "skip":
//             targetTime = videoPlayer.time + 5;
//             videoPlayer.time = Mathf.Clamp((float)targetTime, 0, (float)videoPlayer.length);
//             textObject.text = "Skipped";
//             break;

//         case "drawback":
//             targetTime = videoPlayer.time + (-5);
//             videoPlayer.time = Mathf.Clamp((float)targetTime, 0, (float)videoPlayer.length);
//             textObject.text = "Drawback";
//             break;

//         case "volumeup":
//             currentVolume = Mathf.Clamp(currentVolume + 0.1f, 0f, 1f);
//             videoPlayer.SetDirectAudioVolume(0, currentVolume);
//             textObject.text = "Volume Up";
//             break;

//         case "volumedown":
//             currentVolume = Mathf.Clamp(currentVolume + (-0.1f), 0f, 1f);
//             videoPlayer.SetDirectAudioVolume(0, currentVolume);
//             textObject.text = "Volume Down";
//             break;

//         default:
//             textObject.text = "none";
//             StartCoroutine(HideTextAfterDelay(2f)); // Hide text after 2 seconds if "none"
//             break;
//     }
// }

// private IEnumerator HideTextAfterDelay(float delay)
// {
//     yield return new WaitForSeconds(delay); // Wait for the specified delay
//     textObject.text = ""; // Clear the text
// }

//     [System.Serializable]
//     public class PixelColorResponse
//     {
//         public int[] color;
//     }

//     [System.Serializable]
//     public class GestureResponse
//     {
//         public string gesture;
//     }

// }
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Networking;
using TMPro;
using System.Threading.Tasks;

public class ARCameraCaptureController : MonoBehaviour
{
    public Camera arCamera; // Assign the AR camera
    public RawImage rawImage; // Reference to RawImage in UI
    public VideoPlayer videoPlayer; // Reference to VideoPlayer
    public TMP_Text textObject; // Reference to the Text component
    private float currentVolume = 0.5f; // Initial volume
    private string lastGesture = ""; // To prevent repeated actions for the same gesture
    private Queue<string> gestureQueue = new Queue<string>(); // Queue for gesture commands
    private RenderTexture reusableRenderTexture;
    private Texture2D reusableTexture;

    void Start()
    {
        // Initialize reusable RenderTexture and Texture2D
        reusableRenderTexture = new RenderTexture(640, 360, 24);
        reusableTexture = new Texture2D(640, 360, TextureFormat.RGB24, false);

        // Start capturing frames periodically
        InvokeRepeating(nameof(CaptureFrameAndSend), 0f, 0.4f); // Adjust interval as needed
    }

    void CaptureFrameAndSend()
    {
        if (arCamera == null)
        {
            Debug.LogError("AR Camera is not assigned!");
            return;
        }

        // Set the camera's target texture
        arCamera.targetTexture = reusableRenderTexture;

        // Render the camera's view
        arCamera.Render();

        // Capture the frame into the reusable Texture2D
        RenderTexture.active = reusableRenderTexture;
        reusableTexture.ReadPixels(new Rect(0, 0, reusableRenderTexture.width, reusableRenderTexture.height), 0, 0);
        reusableTexture.Apply();

        // Clear target texture to avoid warnings
        arCamera.targetTexture = null;
        RenderTexture.active = null;

        // Send the frame to the Flask API asynchronously
        StartCoroutine(SendFrameToAPI(reusableTexture));
    }


    IEnumerator SendFrameToAPI(Texture2D frame)
    {
        Debug.Log("Sending frame to API");

        // Encode texture to PNG
        byte[] imageBytes = frame.EncodeToPNG();
        WWWForm form = new WWWForm();
        form.AddBinaryData("frame", imageBytes, "frame.jpg", "image/jpeg");

        // Send the POST request asynchronously
        UnityWebRequest www = UnityWebRequest.Post("http://192.168.1.30:8000/", form);
        yield return www.SendWebRequest();

        Debug.Log(www.downloadHandler.text);

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Image sent successfully!");

            string responseText = www.downloadHandler.text;
            GestureResponse response = JsonUtility.FromJson<GestureResponse>(responseText);

            if (!string.IsNullOrEmpty(response.gesture))
            {
                lock (gestureQueue)
                {
                    gestureQueue.Enqueue(response.gesture);
                    Debug.Log("Gesture: " + response.gesture);
                }
            }
        }
        else
        {
            Debug.LogError("Error: " + www.error);
            textObject.text = www.error;
        }
    }

    void Update()
    {
        // Process gestures from the queue
        if (gestureQueue.Count > 0)
        {
            string gesture;
            lock (gestureQueue)
            {
                gesture = gestureQueue.Dequeue();
            }
            ControlVideo(gesture);
        }
    }

    private void ControlVideo(string gesture)
    {
        double targetTime;

        // Check gesture and perform corresponding actions
        switch (gesture)
        {
            case "playback":
                if (!videoPlayer.isPlaying)
                {
                    videoPlayer.Play();
                    textObject.text = "Playing";
                }
                else
                {
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
                targetTime = videoPlayer.time - 5;
                videoPlayer.time = Mathf.Clamp((float)targetTime, 0, (float)videoPlayer.length);
                textObject.text = "Drawback";
                break;

            case "volumeup":
                currentVolume = Mathf.Clamp(currentVolume + 0.1f, 0f, 1f);
                videoPlayer.SetDirectAudioVolume(0, currentVolume);
                textObject.text = "Volume Up";
                break;

            case "volumedown":
                currentVolume = Mathf.Clamp(currentVolume - 0.1f, 0f, 1f);
                videoPlayer.SetDirectAudioVolume(0, currentVolume);
                textObject.text = "Volume Down";
                break;

            default:
                textObject.text = "none";
                StartCoroutine(HideTextAfterDelay(2f)); // Hide text after 2 seconds if "none"
                break;
        }
    }

    private IEnumerator HideTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        textObject.text = ""; // Clear the text
    }

    private void OnDestroy()
    {
        // Cleanup reusable resources
        if (reusableRenderTexture != null) reusableRenderTexture.Release();
        if (reusableTexture != null) Destroy(reusableTexture);
    }

    [System.Serializable]
    public class GestureResponse
    {
        public string gesture;
    }
}
