from flask import Flask, request, jsonify
from PIL import Image
import io
from app import get_gesture_from_frame
from flask_cors import CORS
import cv2
import numpy as np
app = Flask(__name__)
CORS(app)


@app.route('/', methods=['GET', 'POST'])
def process_frame():
    try:
        # Get image data from the request
        if 'frame' not in request.files:
            return jsonify({'error': 'No frame provided in the request'}), 400

        file = request.files['frame']
        image = Image.open(io.BytesIO(file.read()))
        cv_image = cv2.cvtColor(np.array(image), cv2.COLOR_RGB2BGR)
        gesture_result = get_gesture_from_frame(cv_image)
        print(gesture_result)
        print('Image received and processed')
        
        return jsonify({'gesture': gesture_result}), 200
        # print('Image received and processed')

        # # Get the color of pixel (1, 1)
        # pixel_color = image.getpixel((1, 1))

        # print(f'Pixel color at (1, 1): {pixel_color}')

        # return jsonify({'color': pixel_color}), 200
    except Exception as e:
        print(f"Error occurred: {str(e)}")
        return jsonify({'error': str(e)}), 500

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000, debug=True)
