namespace Backend_AIHost.Services.Docker
{
    public class PyFileGenerator
    {
        public static string GenerateAppPy(string modelName, int maxNetTokens, int port)
        {
            return $$"""
import subprocess
import sys
import threading
from flask import Flask, request, jsonify
from flask_cors import CORS

app = Flask(__name__)
CORS(app, origins=['http://localhost:5173'])

pipe = None
model_ready = False
status_message = "Model jest pobierany..."

def install_and_load_model(model_name):
    global pipe, model_ready, status_message
    try:
        # Instalacja brakujących pakietów w czasie działania kontenera
        subprocess.check_call([sys.executable, "-m", "pip", "install", "--no-cache-dir", "torch", "transformers"])

        from transformers import pipeline
        pipe = pipeline('text-generation', model=model_name)
        model_ready = True
        status_message = "Model gotowy!"
    except Exception as e:
        model_ready = False
        status_message = f"Błąd pobierania modelu: {str(e)}"

# Uruchamiamy w tle po starcie
model_name = "{{modelName}}"
max_tokens = {{maxNetTokens}}
port = {{port}}
threading.Thread(target=install_and_load_model, args=(model_name,), daemon=True).start()

@app.route('/generate', methods=['POST'])
def generate():
    data = request.get_json(force=True)
    prompt = data.get('prompt', '')
    if not prompt:
        return jsonify([{"generated_text": "Prompt is missing"}]), 400
    if not model_ready:
        return jsonify([{"generated_text": status_message}])
    try:
        result = pipe(prompt, max_new_tokens=max_tokens)
        return jsonify(result)
    except Exception as e:
        return jsonify([{"generated_text": str(e)}]), 500

@app.route('/status', methods=['GET'])
def status():
    return jsonify({"model_ready": model_ready, "message": status_message})

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=port, debug=True)

""";
        }

    }
}
