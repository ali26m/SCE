from flask import Flask, request, jsonify
from PIL import Image
import io
from flask_cors import CORS

app = Flask(__name__)
CORS(app)


@app.route('/', methods=['GET', 'POST'])
def process_frame():
    print('Request received')
    try:
        # Get image data from the request
        if 'frame' not in request.files:
            return jsonify({'error': 'No frame provided in the request'}), 400

        file = request.files['frame']
        image = Image.open(io.BytesIO(file.read()))
        
        print('Image received and processed')

        # Get the color of pixel (1, 1)
        pixel_color = image.getpixel((1, 1))

        print(f'Pixel color at (1, 1): {pixel_color}')

        return jsonify({'color': pixel_color}), 200
    except Exception as e:
        print(f"Error occurred: {str(e)}")
        return jsonify({'error': str(e)}), 500

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000, debug=True)
