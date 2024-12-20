from flask import Flask, request, jsonify
from PIL import Image
import io

app = Flask(__name__)

@app.route('/detect', methods=['POST'])
def process_frame():
    if 'image' not in request.files:
        return jsonify({"error": "No image provided"}), 400

    file = request.files['image']
    image = Image.open(io.BytesIO(file.read()))

    # Dummy processing: Return image size as a response
    width, height = image.size
    response = {"width": width, "height": height}

    return jsonify(response)

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000, debug=True)
