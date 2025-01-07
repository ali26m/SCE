from flask import Flask, request, jsonify
from PIL import Image
import io
import cv2
import mediapipe as mp
import numpy as np
import setuptools.dist

#-------------------------------------------------------------------------------------------
def calculate_angle(a,b,c):
    
    radians = np.arctan2(c-b, c-b) - np.arctan2(a-b, a-b)
    angle = np.abs(radians*180.0/np.pi)
        
    return angle

#-------------------------------------------------------------------------------------------
def volume(hand_landmarks, index_tip, wrist):
    index_mid = hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_MCP].y

    angle = calculate_angle(index_tip, index_mid, wrist)

    if angle == 180.0:
         return jsonify({'gesture': "volumeup"}), 200
    elif angle == 0.0:
         return jsonify({'gesture': "vloumedown"}), 200
            
#-------------------------------------------------------------------------------------------
def playback(thumb_tip, index_tip, middle_tip, pinky_tip):
        
    if index_tip < thumb_tip and middle_tip < thumb_tip and pinky_tip > thumb_tip:
        return jsonify({'gesture': "playback"}), 200

#-------------------------------------------------------------------------------------------
def skip(hand_landmarks):
    
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
    
    if open_fist == True:
        return open_fist
    
    return False

#-------------------------------------------------------------------------------------------
def drawback(hand_landmarks):
    
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
    
    if closed_fist == True:
        return closed_fist

app = Flask(__name__)

@app.route('/', methods=['GET', 'POST'])
def process_frame():

    mp_hands = mp.solutions.hands
    hands = mp_hands.Hands(static_image_mode=False, max_num_hands=1,
                       min_detection_confidence=0.5, min_tracking_confidence=0.5)


    try:
        if 'frame' not in request.files:
            return jsonify({'error': 'No frame provided in the request'}), 400

        file = request.files['frame']
        image = Image.open(io.BytesIO(file.read()))
        cv_image = cv2.cvtColor(np.array(image), cv2.COLOR_RGB2BGR)

        results = hands.process(cv_image)

        if results.multi_hand_landmarks:
            for hand_landmarks in results.multi_hand_landmarks:
                mp_drawing.draw_landmarks(
                    frame, hand_landmarks, mp_hands.HAND_CONNECTIONS)

                try:

                    thumb_tip = hand_landmarks.landmark[mp_hands.HandLandmark.THUMB_TIP].y
                    index_tip = hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_TIP].y
                    middle_tip = hand_landmarks.landmark[mp_hands.HandLandmark.MIDDLE_FINGER_TIP].y
                    pinky_tip = hand_landmarks.landmark[mp_hands.HandLandmark.PINKY_TIP].y
                    wrist = hand_landmarks.landmark[mp_hands.HandLandmark.WRIST].y

                    if skip(hand_landmarks):
                        return jsonify({'gesture': "skip"}), 200

                    elif drawback(hand_landmarks):
                        return jsonify({'gesture': "drawback"}), 200

                    elif index_tip < thumb_tip and middle_tip < thumb_tip and pinky_tip > thumb_tip:
                        return playback(thumb_tip, index_tip, middle_tip, pinky_tip)

                    elif index_tip < wrist:
                        return volume(hand_landmarks, index_tip, wrist)

                except Exception as error:
                    print(error)
                    return jsonify({'gesture': error}), 200
        
        return jsonify({'gesture': "none"}), 200        
    except Exception as e:
        print(f"Error occurred: {str(e)}")
        return jsonify({'error': str(e)}), 500

app.run(debug=True)