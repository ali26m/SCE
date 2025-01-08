from fastapi import FastAPI, UploadFile, File, HTTPException
from fastapi.responses import JSONResponse
from typing import Union
from PIL import Image
import io
import cv2
import mediapipe as mp
import numpy as np

app = FastAPI()

# Initialize Mediapipe Hands solution
mp_hands = mp.solutions.hands
mp_drawing = mp.solutions.drawing_utils

# -------------------------------------------------------------------------------------------
def calculate_angle(a, b, c):
    radians = np.arctan2(c - b, c - b) - np.arctan2(a - b, a - b)
    angle = np.abs(radians * 180.0 / np.pi)
    return angle

# -------------------------------------------------------------------------------------------
def volume(hand_landmarks, index_tip, wrist):
    index_mid = hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_MCP].y
    angle = calculate_angle(index_tip, index_mid, wrist)

    if angle == 180.0:
        return {"gesture": "volumeup"}
    elif angle == 0.0:
        return {"gesture": "volumedown"}
    return {"gesture": "none"}

# -------------------------------------------------------------------------------------------
def playback(thumb_tip, index_tip, middle_tip, pinky_tip):
    if index_tip < thumb_tip and middle_tip < thumb_tip and pinky_tip > thumb_tip:
        return {"gesture": "playback"}
    return {"gesture": "none"}

# -------------------------------------------------------------------------------------------
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
    open_fist = all(tip.y < mcp.y for tip, mcp in zip(tips, mcps))
    return open_fist

# -------------------------------------------------------------------------------------------
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
    closed_fist = all(tip.y > mcp.y for tip, mcp in zip(tips, mcps))
    return closed_fist

# -------------------------------------------------------------------------------------------
@app.post("/")
@app.get("/")
async def process_frame(frame: UploadFile = File(...)):
    hands = mp_hands.Hands(static_image_mode=False, max_num_hands=1,
                           min_detection_confidence=0.5, min_tracking_confidence=0.5)

    try:
        image = Image.open(io.BytesIO(await frame.read()))
        cv_image = cv2.cvtColor(np.array(image), cv2.COLOR_RGB2BGR)

        results = hands.process(cv_image)

        if results.multi_hand_landmarks:
            for hand_landmarks in results.multi_hand_landmarks:

                thumb_tip = hand_landmarks.landmark[mp_hands.HandLandmark.THUMB_TIP].y
                index_tip = hand_landmarks.landmark[mp_hands.HandLandmark.INDEX_FINGER_TIP].y
                middle_tip = hand_landmarks.landmark[mp_hands.HandLandmark.MIDDLE_FINGER_TIP].y
                pinky_tip = hand_landmarks.landmark[mp_hands.HandLandmark.PINKY_TIP].y
                wrist = hand_landmarks.landmark[mp_hands.HandLandmark.WRIST].y

                if skip(hand_landmarks):
                    return JSONResponse(content={"gesture": "skip"})

                elif drawback(hand_landmarks):
                    return JSONResponse(content={"gesture": "drawback"})

                elif index_tip < thumb_tip and middle_tip < thumb_tip and pinky_tip > thumb_tip:
                    return JSONResponse(content=playback(thumb_tip, index_tip, middle_tip, pinky_tip))

                elif index_tip < wrist:
                    return JSONResponse(content=volume(hand_landmarks, index_tip, wrist))

        return JSONResponse(content={"gesture": "none"})
    except Exception as e:
        return HTTPException(status_code=500, detail=str(e))

# Run with: uvicorn app:app --reload
