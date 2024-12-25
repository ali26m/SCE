import cv2
import pyautogui
import mediapipe as mp
import numpy as np
import time

isplaying = False
last_toggle_time = 0

#-------------------------------------------------------------------------------------------
def calculate_angle(a,b,c):
    a = np.array(a) # First
    b = np.array(b) # Mid
    c = np.array(c) # End
    
    radians = np.arctan2(c-b, c-b) - np.arctan2(a-b, a-b)
    angle = np.abs(radians*180.0/np.pi)
    
    if angle >180.0:
        angle = 360-angle
        
    return angle

#-------------------------------------------------------------------------------------------
def volume(hand_landmarks):
    index_finger_tip = hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_TIP].y
    index_finger_mid = hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_MCP].y
    wrist = hand_landmarks.landmark[mp_hands.HandLandmark.WRIST].y

    angle = calculate_angle(index_finger_tip, index_finger_mid, wrist)

    if angle == 180.0:
         return 'volumeup'
    elif angle == 0.0:
         return 'volumedown'
            
#-------------------------------------------------------------------------------------------
def playback(hand_landmarks,pinky_tip):
    global isplaying, last_toggle_time
    
    index_tip = hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_TIP].y
    middle_tip = hand_landmarks.landmark[mp_hands.HandLandmark.MIDDLE_FINGER_TIP].y
    thumb_tip = hand_landmarks.landmark[mp_hands.HandLandmark.THUMB_TIP].y
    
    current_time = time.time()
    if index_tip < thumb_tip and middle_tip < thumb_tip and pinky_tip > thumb_tip and current_time - last_toggle_time > 4:
        isplaying = not isplaying
        last_toggle_time = current_time
    print(isplaying)

#-------------------------------------------------------------------------------------------
def skip(hand_landmarks):
    global last_toggle_time
    
    tips = [
        hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_TIP],
        hand_landmarks.landmark[mp_hands.HandLandmark.MIDDLE_FINGER_TIP],
        hand_landmarks.landmark[mp_hands.HandLandmark.RING_FINGER_TIP],
        hand_landmarks.landmark[mp_hands.HandLandmark.PINKY_TIP],
    ]
    mcps = [
        hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_MCP],
        hand_landmarks.landmark[mp_hands.HandLandmark.MIDDLE_FINGER_MCP],
        hand_landmarks.landmark[mp_hands.HandLandmark.RING_FINGER_MCP],
        hand_landmarks.landmark[mp_hands.HandLandmark.PINKY_MCP],
    ]
    
    # Check if each finger tip is above its corresponding MCP joint
    open_fist = all(tip.y < mcp.y for tip, mcp in zip(tips, mcps))
    
    current_time = time.time()
    if open_fist == True and current_time - last_toggle_time > 1:
        print("skipped 5 seconds")
        last_toggle_time = current_time
        
    return open_fist

#-------------------------------------------------------------------------------------------
def drawback(hand_landmarks):
    global last_toggle_time
    
    tips = [
        hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_TIP],
        hand_landmarks.landmark[mp_hands.HandLandmark.MIDDLE_FINGER_TIP],
        hand_landmarks.landmark[mp_hands.HandLandmark.RING_FINGER_TIP],
        hand_landmarks.landmark[mp_hands.HandLandmark.PINKY_TIP],
    ]
    mcps = [
        hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_MCP],
        hand_landmarks.landmark[mp_hands.HandLandmark.MIDDLE_FINGER_MCP],
        hand_landmarks.landmark[mp_hands.HandLandmark.RING_FINGER_MCP],
        hand_landmarks.landmark[mp_hands.HandLandmark.PINKY_MCP],
    ]
    
    # Check if each finger tip is below its corresponding MCP joint (closed fist condition)
    closed_fist = all(tip.y > mcp.y for tip, mcp in zip(tips, mcps))
    
    current_time = time.time()
    if closed_fist == True and current_time - last_toggle_time > 1:
        print("drawbacked 5 seconds")
        last_toggle_time = current_time
    
    return closed_fist

#-------------------------------------------------------------------------------------------

mp_hands = mp.solutions.hands
hands = mp_hands.Hands(static_image_mode=False, max_num_hands=1,
                       min_detection_confidence=0.5, min_tracking_confidence=0.5)

mp_drawing = mp.solutions.drawing_utils

# frame =cv2.imread("C:\\Users\\alihi\\ipynb\\Image Processing\\skip.jpg")
def get_gesture_from_frame(frame):
    # frame = cv2.imread("C:\\Users\\AbdulRahman\\Desktop\\collage PROJECTS\\IMAGE_PROCESSING PROJECT\\image-processing-project\\testCases\\volup.jpg")


    image_rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)

    results = hands.process(image_rgb)

    if results.multi_hand_landmarks:
        for hand_landmarks in results.multi_hand_landmarks:
            mp_drawing.draw_landmarks(
                frame, hand_landmarks, mp_hands.HAND_CONNECTIONS)

            try:

                index_tip = hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_TIP].y
                pinky_tip = hand_landmarks.landmark[mp_hands.HandLandmark.PINKY_TIP].y
                middle_tip = hand_landmarks.landmark[mp_hands.HandLandmark.MIDDLE_FINGER_TIP].y
                thumb_tip = hand_landmarks.landmark[mp_hands.HandLandmark.THUMB_TIP].y
                wrist = hand_landmarks.landmark[mp_hands.HandLandmark.WRIST].y

                if skip(hand_landmarks):
                    return "skip"

                elif drawback(hand_landmarks):
                    return "drawback"

                elif index_tip < thumb_tip and middle_tip < thumb_tip and pinky_tip > thumb_tip:
                    playback(hand_landmarks,pinky_tip)

                elif index_tip < wrist and pinky_tip < wrist:
                    return volume(hand_landmarks)

            except Exception as error:
                print(error)