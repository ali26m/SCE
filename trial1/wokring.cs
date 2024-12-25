using System;
using Python.Runtime; // Import Python.NET namespace

class Program
{
    static void Main(string[] args)
    {
        // Set the Python runtime library path
        Runtime.PythonDLL = @"C:\\Users\\AbdulRahman\\AppData\\Local\\Programs\\Python\\Python311\\python311.dll";
        PythonEngine.Initialize(); // Initialize the Python Engine

        using (Py.GIL()) // Ensure thread safety when interacting with Python
        {
            // Import necessary Python libraries
            dynamic cv2 = Py.Import("cv2");
            dynamic pyautogui = Py.Import("pyautogui");
            dynamic mp = Py.Import("mediapipe");
            dynamic np = Py.Import("numpy");
            dynamic time = Py.Import("time");

            // Initialize MediaPipe hands module
            dynamic mp_hands = mp.solutions.hands;
            dynamic hands = mp_hands.Hands(static_image_mode: false, max_num_hands: 1,
                                           min_detection_confidence: 0.5, min_tracking_confidence: 0.5);

            dynamic mp_drawing = mp.solutions.drawing_utils;

            // OpenCV VideoCapture (this will open the webcam)
            dynamic cap = cv2.VideoCapture(0);

            bool isPlaying = false;
            double lastToggleTime = 0;



            while (true)
            {
                // Read frame from webcam
                dynamic frame = cap.read();
                if (!frame[0])
                    break;
                //Console.WriteLine(frame[1]);

                // Convert frame to RGB for MediaPipe
                dynamic image_rgb = cv2.cvtColor(frame[1], cv2.COLOR_BGR2RGB);
                dynamic results = hands.process(image_rgb);

                if (results.multi_hand_landmarks != null)
                {
                    foreach (var hand_landmarks in results.multi_hand_landmarks)
                    {
                        mp_drawing.draw_landmarks(frame[1], hand_landmarks, mp_hands.HAND_CONNECTIONS);

                        // Process hand landmarks and trigger actions
                        dynamic index_tip = hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_TIP].y;
                        dynamic pinky_tip = hand_landmarks.landmark[mp_hands.HandLandmark.PINKY_TIP].y;
                        dynamic middle_tip = hand_landmarks.landmark[mp_hands.HandLandmark.MIDDLE_FINGER_TIP].y;
                        dynamic thumb_tip = hand_landmarks.landmark[mp_hands.HandLandmark.THUMB_TIP].y;
                        dynamic wrist = hand_landmarks.landmark[mp_hands.HandLandmark.WRIST].y;

                        if (skip(hand_landmarks))
                        {
                            skip(hand_landmarks);
                        }
                        else if (drawback(hand_landmarks))
                        {
                            drawback(hand_landmarks);
                        }
                        else if (index_tip < thumb_tip && middle_tip < thumb_tip && pinky_tip > thumb_tip)
                        {
                            playback(hand_landmarks);
                        }
                        else if (index_tip < wrist && pinky_tip < wrist)
                        {
                            volume(hand_landmarks);
                        }
                    }
                }

                // Show the video frame with hand landmarks
                //Console.WriteLine(frame);
                cv2.imshow("Hand Gesture", frame[1]);

                if (cv2.waitKey(1) & 0xFF == 27) // Press 'ESC' to exit
                    break;
            }

            // Release the video capture and close windows
            cap.release();
            cv2.destroyAllWindows();
        }

        PythonEngine.Shutdown(); // Clean up and shut down the Python engine
    }
    static dynamic skip(dynamic hand_landmarks)
    {
        using (Py.GIL())
        {
            dynamic time = Py.Import("time");
            dynamic mp_hands = Py.Import("mediapipe");
            mp_hands = mp_hands.solutions.hands;
            dynamic tips = new dynamic[]
            {
                hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_TIP],
                hand_landmarks.landmark[mp_hands.HandLandmark.MIDDLE_FINGER_TIP],
                hand_landmarks.landmark[mp_hands.HandLandmark.RING_FINGER_TIP],
                hand_landmarks.landmark[mp_hands.HandLandmark.PINKY_TIP]
            };
            dynamic mcps = new dynamic[]
            {
                hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_MCP],
                hand_landmarks.landmark[mp_hands.HandLandmark.MIDDLE_FINGER_MCP],
                hand_landmarks.landmark[mp_hands.HandLandmark.RING_FINGER_MCP],
                hand_landmarks.landmark[mp_hands.HandLandmark.PINKY_MCP]
            };

            bool open_fist = true;
            foreach (var tip in tips)
            {
                if (tip.y >= mcps[Array.IndexOf(tips, tip)].y)
                {
                    open_fist = false;
                    break;
                }
            }

            if (open_fist)
            {
                dynamic current_time = time.time();
                Console.WriteLine("Skipped 5 seconds");
                return true;
            }

            return false;
        }
    }

    static dynamic drawback(dynamic hand_landmarks)
    {
        using (Py.GIL())
        {
            dynamic time = Py.Import("time");
            dynamic mp_hands = Py.Import("mediapipe");
            mp_hands = mp_hands.solutions.hands;
            dynamic tips = new dynamic[]
            {
                hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_TIP],
                hand_landmarks.landmark[mp_hands.HandLandmark.MIDDLE_FINGER_TIP],
                hand_landmarks.landmark[mp_hands.HandLandmark.RING_FINGER_TIP],
                hand_landmarks.landmark[mp_hands.HandLandmark.PINKY_TIP]
            };
            dynamic mcps = new dynamic[]
            {
                hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_MCP],
                hand_landmarks.landmark[mp_hands.HandLandmark.MIDDLE_FINGER_MCP],
                hand_landmarks.landmark[mp_hands.HandLandmark.RING_FINGER_MCP],
                hand_landmarks.landmark[mp_hands.HandLandmark.PINKY_MCP]
            };

            bool closed_fist = true;
            foreach (var tip in tips)
            {
                if (tip.y <= mcps[Array.IndexOf(tips, tip)].y)
                {
                    closed_fist = false;
                    break;
                }
            }

            if (closed_fist)
            {
                dynamic current_time = time.time();
                Console.WriteLine("Drawbacked 5 seconds");
                return true;
            }

            return false;
        }
    }

    static dynamic playback(dynamic hand_landmarks)
    {
        using (Py.GIL())
        {
            dynamic time = Py.Import("time");
            dynamic mp_hands = Py.Import("mediapipe");
            mp_hands = mp_hands.solutions.hands;
            dynamic index_tip = hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_TIP].y;
            dynamic middle_tip = hand_landmarks.landmark[mp_hands.HandLandmark.MIDDLE_FINGER_TIP].y;
            dynamic thumb_tip = hand_landmarks.landmark[mp_hands.HandLandmark.THUMB_TIP].y;
            dynamic pinky_tip = hand_landmarks.landmark[mp_hands.HandLandmark.PINKY_TIP].y;

            dynamic current_time = time.time();
            if (index_tip < thumb_tip && middle_tip < thumb_tip && pinky_tip > thumb_tip)
            {
                Console.WriteLine("Playback triggered");
                return true;
            }

            return false;
        }
    }

    static dynamic volume(dynamic hand_landmarks)
    {
        using (Py.GIL())
        {
            dynamic pyautogui = Py.Import("pyautogui");
            dynamic np = Py.Import("numpy");
            dynamic mp_hands = Py.Import("mediapipe");
            mp_hands = mp_hands.solutions.hands;
            dynamic index_finger_tip = hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_TIP].y;
            dynamic index_finger_mid = hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_MCP].y;
            dynamic wrist = hand_landmarks.landmark[mp_hands.HandLandmark.WRIST].y;

            dynamic angle = calculate_angle(index_finger_tip, index_finger_mid, wrist);
            if (angle == 180.0)
            {
                pyautogui.press("volumeup");
            }
            else if (angle == 0.0)
            {
                pyautogui.press("volumedown");
            }

            return true;
        }
    }

    static dynamic calculate_angle(dynamic a, dynamic b, dynamic c)
    {
        using (Py.GIL())
        {
            dynamic np = Py.Import("numpy");
            dynamic radians = np.arctan2(c - b, c - b) - np.arctan2(a - b, a - b);
            dynamic angle = np.abs(radians * 180.0 / np.pi);

            if (angle > 180.0)
            {
                angle = 360 - angle;
            }

            return angle;
        }
    }
}
