from flask import Flask, request, jsonify
from model import KeyPointClassifier
import json
import csv
import time

app = Flask(__name__)
keypoint_classifier = KeyPointClassifier()

with open('model/keypoint_classifier/keypoint_classifier_label.csv',
              encoding='utf-8-sig') as f:
        keypoint_classifier_labels = csv.reader(f)
        keypoint_classifier_labels = [row[0] for row in keypoint_classifier_labels]        

"""
@app.route('/', methods=['GET'])
def send_data(id):
    print(keypoint_classifier_labels[id])
    return jsonify(keypoint_classifier_labels[id])
"""

@app.route('/', methods=['POST'])
def receive_data():
    data = request.json
    #print("i got data")
    result = process(data)
    return result, 200
    
def process(data):
    coordinates = [handpoint['coordinate'] for handpoint in data['handpoints']]
    toNN = list()
    for coord in coordinates:
        toNN.append(coord['x'])
        toNN.append(coord['y'])
        toNN.append(coord['z'])
    
    start = time.time()
    handsign_id = keypoint_classifier(toNN)
    end = time.time()
    sec = end-start
    print( str(handsign_id) + "   " + str(sec) +" sec")
    
    return keypoint_classifier_labels[handsign_id]

if __name__ == '__main__':
    app.run('0.0.0.0', port=5000, debug=True)