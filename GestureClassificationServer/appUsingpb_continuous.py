import threading
from flask import Flask, request, jsonify
from model import ContinuousClassifier_pb
import json
import csv
import time

app = Flask(__name__)
continuous_classifier = ContinuousClassifier_pb()

with open('model/keypoint_history_classifier/continuous_classifier_label.csv',
              encoding='utf-8-sig') as f:
        continuous_classifier_labels = csv.reader(f)
        continuous_classifier_labels = [row[0] for row in continuous_classifier_labels]        

@app.route('/', methods=['POST'])
#유니티로부터 데이터를 json 형식으로 받음
def receive_data():
    data = request.json
    
    #데이터 처리
    result = process(data)
    
    #유니티로 결과 리턴
    return result, 300
    
def process(data):
    
    toNN = list()
    for ges in data:
        toNN.append(ges)
    if len(toNN) != 24:
        return "yet"

    start = time.time()
    continuous_sign_id = continuous_classifier(toNN)
    end = time.time()
    sec = end-start
    
    print(str(continuous_sign_id) + "   " + str(sec) +" sec")
    
    return continuous_classifier_labels[continuous_sign_id]

def runFlask2():
    app.run('0.0.0.0', port=8000, debug=False, threaded=True)

if __name__ == '__main__':
    t2 = threading.Thread(target=runFlask2)
    t2.start()