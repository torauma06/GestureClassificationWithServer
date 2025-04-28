import threading
from flask import Flask, request, jsonify
from model import KeyPointClassifier_pb
import json
import csv
import time

app = Flask(__name__)
keypoint_classifier = KeyPointClassifier_pb()

with open('model/keypoint_classifier/keypoint_classifier_label.csv',
              encoding='utf-8-sig') as f:
        keypoint_classifier_labels = csv.reader(f)
        keypoint_classifier_labels = [row[0] for row in keypoint_classifier_labels]        

@app.route('/', methods=['POST'])
#유니티로부터 데이터를 json 형식으로 받음
def receive_data():
    data = request.json

    #데이터 처리
    result = process(data)
    
    #유니티로 결과 리턴
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
    
    print(str(handsign_id) + "   " + str(sec) +" sec")
    
    return keypoint_classifier_labels[handsign_id]

def runFlask1():
    app.run('0.0.0.0', port=5000, debug=False, threaded=True)

if __name__ == '__main__':
    t1 = threading.Thread(target=runFlask1)
    t1.start()