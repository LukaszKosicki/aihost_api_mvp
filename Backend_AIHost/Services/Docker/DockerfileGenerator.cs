using Microsoft.AspNetCore.Mvc.ModelBinding;
using Org.BouncyCastle.Asn1.Mozilla;

namespace Backend_AIHost.Services.Docker
{
    public class DockerfileGenerator
    {
        public static string GenerateDockerfileWithoutPyFile(string modelName, int port, int maxNetTokens = 50)
        {
            return $$"""
FROM python:3.10-slim

WORKDIR /app

# Instalacja tylko lekkich zależności
RUN pip install --no-cache-dir flask flask-cors

# Skopiuj plik aplikacji
COPY app.py /app/

# Wystaw port
EXPOSE {{port}}

# Uruchom aplikację
CMD ["python", "app.py"]
""";
        }


        public static string GenerateDockerfileForTextGenerationModel(string modelName, int port, int maxNetTokens = 50)
        {
            return $$"""
FROM python:3.10-slim

WORKDIR /app

# Instalacja zależności
RUN pip install --no-cache-dir torch transformers flask flask-cors
RUN python -c "from transformers import pipeline; pipeline('text-generation', model='{{modelName}}')"
# Tworzymy plik app.py bez błędów składniowych
RUN echo "\ 
from transformers import pipeline\n\
from flask import Flask, request, jsonify\n\
from flask_cors import CORS\n\
import logging\n\
import time\n\
\n\
app = Flask(__name__)\n\
CORS(app, origins=['http://localhost:5173'])\n\
pipe = pipeline('text-generation', model='{{modelName}}')\n\
\n\
@app.route('/generate', methods=['POST'])\n\
def generate():\n\
    try:\n\
        data = request.get_json(force=True)\n\
        prompt = data.get('prompt', '')\n\
        if not prompt:\n\
            return jsonify({'error': 'Prompt is missing'}), 400\n\
        result = pipe(prompt, max_new_tokens={{maxNetTokens}})\n\
        return jsonify(result)\n\
    except Exception as e:\n\
        return jsonify({'error': str(e)}), 500\n\
\n\
if __name__ == '__main__':\n\
    app.run(host='0.0.0.0', port={{port}}, debug=True)\n\
" > app.py

EXPOSE {{port}}

CMD ["python", "app.py"]

""";
        }
    }
}
