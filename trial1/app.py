import cv2
import mediapipe as mp
import numpy as np
import sys
import time

# Initialize MediaPipe Hands
mp_hands = mp.solutions.hands
hands = mp_hands.Hands(static_image_mode=True, max_num_hands=1,
                       min_detection_confidence=0.5, min_tracking_confidence=0.5)
mp_drawing = mp.solutions.drawing_utils

# Define the actions
def calculate_angle(a, b, c):
    a, b, c = np.array(a), np.array(b), np.array(c)
    radians = np.arctan2(c[1] - b[1], c[0] - b[0]) - np.arctan2(a[1] - b[1], a[0] - b[0])
    angle = np.abs(radians * 180.0 / np.pi)
    if angle > 180.0:
        angle = 360 - angle
    return angle

def volume(hand_landmarks):
    index_finger_tip = hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_TIP].y
    index_finger_mid = hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_MCP].y
    wrist = hand_landmarks.landmark[mp_hands.HandLandmark.WRIST].y

    angle = calculate_angle(index_finger_tip, index_finger_mid, wrist)

    if angle == 180.0:
         print("volumeup")
    elif angle == 0.0:
         print("volumedown")

def playback(hand_landmarks):
    global isplaying, last_toggle_time
    
    index_tip = hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_TIP].y
    middle_tip = hand_landmarks.landmark[mp_hands.HandLandmark.MIDDLE_FINGER_TIP].y
    thumb_tip = hand_landmarks.landmark[mp_hands.HandLandmark.THUMB_TIP].y
    
    current_time = time.time()
    if index_tip < thumb_tip and middle_tip < thumb_tip and pinky_tip > thumb_tip and current_time - last_toggle_time > 4:
        isplaying = not isplaying
        print(isplaying)
        last_toggle_time = current_time

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
        print("skip")
        last_toggle_time = current_time

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
        print("drawback")
        last_toggle_time = current_time

# Chatgpt generated code
# def detect_action(hand_landmarks):
#     index_tip = hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_TIP].y
#     pinky_tip = hand_landmarks.landmark[mp_hands.HandLandmark.PINKY_TIP].y
#     middle_tip = hand_landmarks.landmark[mp_hands.HandLandmark.MIDDLE_FINGER_TIP].y
#     thumb_tip = hand_landmarks.landmark[mp_hands.HandLandmark.THUMB_TIP].y
#     wrist = hand_landmarks.landmark[mp_hands.HandLandmark.WRIST].y

#     # Volume Control Gesture
#     if index_tip < wrist and pinky_tip < wrist:
#         return "volume"

#     # Playback Control Gesture
#     if index_tip < thumb_tip and middle_tip < thumb_tip and pinky_tip > thumb_tip:
#         return "playback"

#     # Skip Gesture
#     tips = [
#         hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_TIP],
#         hand_landmarks.landmark[mp_hands.HandLandmark.MIDDLE_FINGER_TIP],
#         hand_landmarks.landmark[mp_hands.HandLandmark.RING_FINGER_TIP],
#         hand_landmarks.landmark[mp_hands.HandLandmark.PINKY_TIP],
#     ]
#     mcps = [
#         hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_MCP],
#         hand_landmarks.landmark[mp_hands.HandLandmark.MIDDLE_FINGER_MCP],
#         hand_landmarks.landmark[mp_hands.HandLandmark.RING_FINGER_MCP],
#         hand_landmarks.landmark[mp_hands.HandLandmark.PINKY_MCP],
#     ]
#     if all(tip.y < mcp.y for tip, mcp in zip(tips, mcps)):
#         return "skip"

#     # Drawback Gesture
#     if all(tip.y > mcp.y for tip, mcp in zip(tips, mcps)):
#         return "drawback"

#     return "unknown"

# Main script entry point
if len(sys.argv) < 2:
    print("Error: No image path provided.")
    sys.exit(1)

image_path = sys.argv[1]

frame = cv2.imread(image_path)

if frame is None:
    print("Error: Could not load image.")
    sys.exit(1)

# Convert the image to RGB
image_rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)

# Process the image with MediaPipe Hands
results = hands.process(image_rgb)

if results.multi_hand_landmarks:
    for hand_landmarks in results.multi_hand_landmarks:
        
        index_tip = hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_TIP].y
        pinky_tip = hand_landmarks.landmark[mp_hands.HandLandmark.PINKY_TIP].y
        middle_tip = hand_landmarks.landmark[mp_hands.HandLandmark.MIDDLE_FINGER_TIP].y
        thumb_tip = hand_landmarks.landmark[mp_hands.HandLandmark.THUMB_TIP].y
        wrist = hand_landmarks.landmark[mp_hands.HandLandmark.WRIST].y

        if skip(hand_landmarks):
            skip(hand_landmarks)

        elif drawback(hand_landmarks):
            drawback(hand_landmarks)

        elif index_tip < thumb_tip and middle_tip < thumb_tip and pinky_tip > thumb_tip:
            playback(hand_landmarks)

        elif index_tip < wrist and pinky_tip < wrist:
            volume(hand_landmarks)
        
        # print(detect_action(hand_landmarks))

        sys.exit(0)

print("no_hand_detected")
sys.exit(0)
